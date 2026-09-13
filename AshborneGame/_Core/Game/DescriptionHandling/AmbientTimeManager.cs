using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    /// <summary>
    /// Tracks idle time in a location using game ticks (1 tick = 1 second; 60 ticks = 1 in-game hour)
    /// and outputs ambient descriptions when <see cref="AmbientDescription.FromDuration"/> thresholds are reached.
    /// </summary>
    public class AmbientTimeManager : IDisposable
    {
        private const int RandomDelayMinimumTicks = 15;
        private const int RandomDelayMaximumTicks = 25;
        private const int MinimumAmbientSpacingTicks = 5;

        private AmbientDescription? _ambientDescription;
        private readonly Random _random;
        private readonly HashSet<int> _triggeredFixedTimes = new();
        private readonly List<string> _unusedRandomTimedDescriptions = new();

        private int _ticksSinceReset;
        private int _hoursPassedSinceReset;
        private double? _nextRandomTimedDescriptionAt;
        private double _randomDelayMultiplier;
        private int? _lastAmbientDescriptionTick;
        private bool _isActive;
        private bool _isPaused;
        private bool _isTypewriterActive;
        private bool _isInputPaused;
        private readonly object _lock = new();
        private readonly EventToken _tickSubscription;

        public event Action<string>? OnAmbientDescriptionTriggered;
        public event Func<string, Task>? OnAmbientDescriptionTriggeredAsync;
        public event Action? OnInputPaused;
        public event Action? OnInputResumed;

        public int TicksSinceReset
        {
            get { lock (_lock) { return _ticksSinceReset; } }
        }

        public int HoursPassedSinceReset
        {
            get { lock (_lock) { return _hoursPassedSinceReset; } }
        }

        public AmbientTimeManager(Random? random = null)
        {
            _random = random ?? new Random();
            _tickSubscription = EventBus.Subscribe<GameEvents.System.TickEvent>(OnTick);
        }

        /// <summary>
        /// Call when the player enters a location that may have timed ambient descriptions.
        /// </summary>
        public void OnEnterLocation(Location location)
        {
            lock (_lock)
            {
                _ambientDescription = location.DescriptionComposer.Ambient;
                _ticksSinceReset = 0;
                _hoursPassedSinceReset = 0;
                _triggeredFixedTimes.Clear();
                _unusedRandomTimedDescriptions.Clear();
                if (_ambientDescription != null)
                {
                    _unusedRandomTimedDescriptions.AddRange(_ambientDescription.FromRandomTimeBased);
                }

                _randomDelayMultiplier = 1;
                _nextRandomTimedDescriptionAt = GetNextRandomTimedDescriptionAt();
                _lastAmbientDescriptionTick = null;
                _isActive = _ambientDescription != null &&
                    (_ambientDescription.FromDuration.Count > 0 || _unusedRandomTimedDescriptions.Count > 0);
                _isPaused = false;
                _isTypewriterActive = false;
                _isInputPaused = false;
            }
        }

        /// <summary>
        /// Call when the player leaves the active location.
        /// </summary>
        public void OnExitLocation()
        {
            lock (_lock)
            {
                _isActive = false;
                _ambientDescription = null;
                _ticksSinceReset = 0;
                _hoursPassedSinceReset = 0;
                _triggeredFixedTimes.Clear();
                _unusedRandomTimedDescriptions.Clear();
                _nextRandomTimedDescriptionAt = null;
                _lastAmbientDescriptionTick = null;
            }
        }

        /// <summary>
        /// Call when the player inputs a command while ambient tracking is active.
        /// </summary>
        public void OnPlayerCommandInput()
        {
            lock (_lock)
            {
                if (!_isActive)
                {
                    return;
                }

                _ticksSinceReset = 0;
                _hoursPassedSinceReset = 0;
                if (_unusedRandomTimedDescriptions.Count > 0)
                {
                    _nextRandomTimedDescriptionAt = GetNextRandomTimedDescriptionAt();
                }
            }
        }

        /// <summary>
        /// Call when typewriter output starts.
        /// </summary>
        public void OnTypewriterStart()
        {
            lock (_lock)
            {
                _isTypewriterActive = true;
                _isPaused = true;
            }
        }

        /// <summary>
        /// Call when typewriter output completes.
        /// </summary>
        public void OnTypewriterComplete()
        {
            lock (_lock)
            {
                _isTypewriterActive = false;
                if (!_isInputPaused)
                {
                    _isPaused = false;
                }
            }
        }

        /// <summary>
        /// Call when ambient description output is complete.
        /// </summary>
        public void OnAmbientDescriptionComplete()
        {
            lock (_lock)
            {
                _isInputPaused = false;
                _isPaused = false;
                OnInputResumed?.Invoke();
            }
        }

        public void Dispose()
        {
            _tickSubscription.Dispose();
        }

        private void OnTick(GameEvents.System.TickEvent tickEvent)
        {
            string? descriptionToTrigger = null;

            lock (_lock)
            {
                if (!_isActive || _isPaused || _isTypewriterActive || _isInputPaused || _ambientDescription == null)
                {
                    return;
                }

                _ticksSinceReset++;
                _hoursPassedSinceReset += tickEvent.HoursPassed;

                if (_ambientDescription.FromDuration.TryGetValue(_ticksSinceReset, out var fixedDescription) &&
                    !_triggeredFixedTimes.Contains(_ticksSinceReset) &&
                    CanTriggerAtCurrentTick())
                {
                    _triggeredFixedTimes.Add(_ticksSinceReset);
                    _isInputPaused = true;
                    _isPaused = true;
                    _lastAmbientDescriptionTick = _ticksSinceReset;
                    descriptionToTrigger = fixedDescription;
                }
                else if (CanTriggerRandomTimedDescription())
                {
                    descriptionToTrigger = TakeRandomTimedDescription();
                    _isInputPaused = true;
                    _isPaused = true;
                    _lastAmbientDescriptionTick = _ticksSinceReset;
                }
            }

            if (descriptionToTrigger != null)
            {
                OnInputPaused?.Invoke();
                _ = TriggerAmbientDescriptionAsync(descriptionToTrigger);
            }
        }

        private bool CanTriggerRandomTimedDescription()
        {
            if (_unusedRandomTimedDescriptions.Count == 0 ||
                !_nextRandomTimedDescriptionAt.HasValue ||
                _ticksSinceReset < _nextRandomTimedDescriptionAt.Value ||
                !CanTriggerAtCurrentTick())
            {
                return false;
            }

            int? nextFixedTime = _ambientDescription!.FromDuration.Keys
                .Where(time => !_triggeredFixedTimes.Contains(time) && time >= _ticksSinceReset)
                .OrderBy(time => time)
                .FirstOrDefault();

            if (nextFixedTime.HasValue && nextFixedTime.Value - _ticksSinceReset < MinimumAmbientSpacingTicks)
            {
                _nextRandomTimedDescriptionAt = nextFixedTime.Value + MinimumAmbientSpacingTicks;
                return false;
            }

            return true;
        }

        private bool CanTriggerAtCurrentTick()
        {
            return !_lastAmbientDescriptionTick.HasValue ||
                _ticksSinceReset - _lastAmbientDescriptionTick.Value >= MinimumAmbientSpacingTicks;
        }

        private string TakeRandomTimedDescription()
        {
            int index = _random.Next(_unusedRandomTimedDescriptions.Count);
            string description = _unusedRandomTimedDescriptions[index];
            _unusedRandomTimedDescriptions.RemoveAll(value => value == description);
            _randomDelayMultiplier *= 1.5;
            _nextRandomTimedDescriptionAt = _ticksSinceReset + GetRandomDelayTicks();
            return description;
        }

        private double GetNextRandomTimedDescriptionAt()
        {
            return GetRandomDelayTicks();
        }

        private double GetRandomDelayTicks()
        {
            return _random.Next(RandomDelayMinimumTicks, RandomDelayMaximumTicks + 1) * _randomDelayMultiplier;
        }

        private async Task TriggerAmbientDescriptionAsync(string description)
        {
            try
            {
                if (OnAmbientDescriptionTriggeredAsync != null)
                {
                    await OnAmbientDescriptionTriggeredAsync(description);
                }
                else if (OnAmbientDescriptionTriggered != null)
                {
                    OnAmbientDescriptionTriggered(description);
                }
                else
                {
                    await IOService.Output.WriteNonDialogueLine(description);
                    OnAmbientDescriptionComplete();
                }
            }
            catch
            {
                OnAmbientDescriptionComplete();
                throw;
            }
        }
    }
}
