using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.QuestManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game
{
    public class TimeTracker
    {
        /// <summary>
        /// One in-game hour passes every 60 ticks. At the default 1000 ms tick interval, one in-game hour equals one real minute.
        /// </summary>
        public const int TicksPerHour = 60;

        private bool _tickRunning;
        private CancellationTokenSource _tickCancellation;
        private Task _tickTask;

        private Location? _currentLocation;
        private int _ticksSinceLastHourAdvance;
        private int _ticksInCurrentLocation;
        // Tracks the total number of ticks spent in the current location, starting from when the player last entered it.
        private int _totalTicksInCurrentLocation;
        private Dictionary<Location, int> _locationDurations = new Dictionary<Location, int>();
        private List<TimeBasedTrigger> _locationTimeTriggers = new();

        // Default to 1000ms per tick
        private int _tickInterval = 1000;

        private readonly QuestTracker _questTracker;

        /// <summary>
        /// Total in-game hours passed since the start of the game.
        /// Defaults to 6 (Dawn of Day 1).
        /// </summary>
        public int TotalInGameHours { get; private set; } = 6;

        public TimeTracker(QuestTracker questTracker)
        {
            _questTracker = questTracker;
        }

        public Globals.Enums.TimeOfDay CurrentTimeOfDay => CalculateTimeOfDay(TotalInGameHours);

        /// <summary>
        /// Advances in-game time by a discrete number of hours.
        /// </summary>
        public void AdvanceTime(int hoursToAdd)
        {
            if (hoursToAdd > 0)
            {
                TotalInGameHours += hoursToAdd;
                IOService.Output.DisplayDebugMessage($"[Time] Advanced {hoursToAdd} hours. Current time: {CurrentTimeOfDay} (Hour of Day: {TotalInGameHours % 24})", Globals.Enums.ConsoleMessageTypes.INFO);
            }
        }

        /// <summary>
        /// Advances time to the next occurrence of the specified TimeOfDay.
        /// </summary>
        public void AdvanceToTimeOfDay(Globals.Enums.TimeOfDay targetTime)
        {
            int currentHourOfDay = TotalInGameHours % 24;
            int targetHour = GetHourForTimeOfDay(targetTime);

            int hoursToAdd = targetHour - currentHourOfDay;
            if (hoursToAdd <= 0)
            {
                hoursToAdd += 24; // Move to the next day's occurrence
            }

            AdvanceTime(hoursToAdd);
        }

        private Globals.Enums.TimeOfDay CalculateTimeOfDay(int totalHours)
        {
            int hourOfDay = totalHours % 24;

            return hourOfDay switch
            {
                >= 5 and < 8 => Globals.Enums.TimeOfDay.Dawn,
                >= 8 and < 12 => Globals.Enums.TimeOfDay.Morning,
                >= 12 and < 14 => Globals.Enums.TimeOfDay.Midday,
                >= 14 and < 18 => Globals.Enums.TimeOfDay.Afternoon,
                >= 18 and < 20 => Globals.Enums.TimeOfDay.Dusk,
                >= 20 and < 24 => Globals.Enums.TimeOfDay.Night,
                _ => Globals.Enums.TimeOfDay.Midnight // 0 to 4
            };
        }

        private int GetHourForTimeOfDay(Globals.Enums.TimeOfDay time)
        {
            return time switch
            {
                Globals.Enums.TimeOfDay.Dawn => 6,
                Globals.Enums.TimeOfDay.Morning => 9,
                Globals.Enums.TimeOfDay.Midday => 12,
                Globals.Enums.TimeOfDay.Afternoon => 15,
                Globals.Enums.TimeOfDay.Dusk => 18,
                Globals.Enums.TimeOfDay.Night => 21,
                Globals.Enums.TimeOfDay.Midnight => 0,
                _ => 6
            };
        }

        #region Ticking

        public void Tick()
        {
            // For simplicity, we assume each tick represents 1 second of real time.
            // TODO: To support variable tick intervals, pass the actual time elapsed since the last tick as a parameter and use that instead of a fixed value.
            _ticksSinceLastHourAdvance++;

            int hoursPassed = _ticksSinceLastHourAdvance / TicksPerHour;
            if (hoursPassed > 0)
            {
                _ticksSinceLastHourAdvance -= hoursPassed * TicksPerHour;
                AdvanceTime(hoursPassed);
            }

            EventBus.Publish(new GameEvents.System.TickEvent(TotalInGameHours, hoursPassed));

            TickLocationTimeTracking();
            _questTracker.TickQuestTimeTracking(hoursPassed);
        }

        private void TickLocationTimeTracking()
        {
            if (GameContext.Player.CurrentLocation != _currentLocation || _currentLocation == null)
                return;

            _ticksInCurrentLocation++;
            _totalTicksInCurrentLocation++;

            foreach (var trigger in _locationTimeTriggers)
            {
                if (trigger.CheckTrigger(_currentLocation.DefinitionID, _totalTicksInCurrentLocation))
                {
                    trigger.Effect?.Invoke();
                    EventBus.Publish(trigger.EventToRaise);
                }
            }
        }

        

        /// <summary>
        /// Begins the tick loop; every tickIntervalMs milliseconds, the Tick() method will be called to update time tracking and trigger checks.
        /// </summary>
        /// <param name="tickIntervalMs">The time, in milliseconds, to wait between each Tick(). Defaults at 1000.</param>
        public void StartTickLoop(int tickIntervalMs = 1000)
        {
            _tickInterval = tickIntervalMs;
            if (_tickRunning)
                return;

            _tickRunning = true;
            _tickCancellation = new CancellationTokenSource();
            var token = _tickCancellation.Token;

            _tickTask = Task.Run(async () =>
            {
                while (!token.IsCancellationRequested)
                {
                    try
                    {
                        Tick();
                        await Task.Delay(tickIntervalMs, token);
                    }
                    catch (TaskCanceledException)
                    {
                        // Normal shutdown
                    }
                    catch (Exception ex)
                    {
                        // Log or handle unexpected errors
                        Console.WriteLine($"[Tick Error] {ex.Message}");
                    }
                }

                _tickRunning = false;
            });
        }

        public async Task StopTickLoop()
        {
            if (_tickCancellation == null || !_tickRunning)
                return;

            _tickCancellation.Cancel();
            if (_tickTask != null)
                await _tickTask;
            _tickCancellation.Dispose();
            _tickCancellation = null;
            _tickTask = null;
        }

        #endregion Tick

        #region Location Time Tracking
        public void OnPlayerEnterLocation(Location location)
        {
            if (_currentLocation != null)
            {
                if (!_locationDurations.ContainsKey(_currentLocation))
                    _locationDurations[_currentLocation] = 0;

                _locationDurations[_currentLocation] += _ticksInCurrentLocation;
            }

            _currentLocation = location;
            _ticksInCurrentLocation = 0;
            _totalTicksInCurrentLocation = 0;

            if (!_locationDurations.ContainsKey(_currentLocation))
                _locationDurations[_currentLocation] = 0;
        }

        public Location? CurrentLocation => _currentLocation;

        public void AddLocationTimeTrigger(TimeBasedTrigger trigger)
        {
            _locationTimeTriggers.Add(trigger);
        }

        #endregion Location Time Tracking
    }
}
