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
        private AmbientDescription? _ambientDescription;

        private int _ticksSinceReset;
        private int _hoursPassedSinceReset;
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

        public AmbientTimeManager()
        {
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
                _isActive = _ambientDescription != null && _ambientDescription.FromDuration.Count > 0;
                _isPaused = false;
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

                if (_ambientDescription.FromDuration.TryGetValue(_ticksSinceReset, out var desc))
                {
                    _isInputPaused = true;
                    _isPaused = true;
                    descriptionToTrigger = desc;
                }
            }

            if (descriptionToTrigger != null)
            {
                OnInputPaused?.Invoke();
                _ = TriggerAmbientDescriptionAsync(descriptionToTrigger);
            }
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
