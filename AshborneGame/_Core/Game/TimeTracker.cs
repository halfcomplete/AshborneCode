using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
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
    public sealed class TimeTracker
    {
        private bool _tickRunning;
        private CancellationTokenSource _tickCancellation;
        private Task _tickTask;
        private DateTime _lastTickTime;

        private Location? _currentLocation;
        private double _secondsSinceLastHourAdvance = 0;
        private int _ticksInCurrentLocation;
        // Tracks the total number of ticks spent in the current location, starting from when the player last entered it.
        private int _totalTicksInCurrentLocation;
        private List<LocationTimeTrigger> _locationTimeTriggers = new();
        private int _tickInterval;
        private readonly int _ticksRequiredToAdvanceHour = 60; // 60 ticks = 1 in-game hour; if tick interval is 1 second, then 1 in-game hour = 1 real minute

        /// <summary>
        /// Total in-game hours passed since the start of the game.
        /// Defaults to 6 (Dawn of Day 1).
        /// </summary>
        public int TotalInGameHours { get; private set; } = 6;

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

        private void Tick()
        {
            var now = DateTime.UtcNow;
            var delta = now - _lastTickTime;
            _lastTickTime = now;

            
            TickLocationTimeTracking(delta);

            TickQuestTimeTracking(delta);
        }

        private void TickLocationTimeTracking(TimeSpan delta)
        {
            // Ensure the player stayed in the same location during this tick
            if (GameContext.Player.CurrentLocation == _currentLocation)
            {
                var secondsSinceLastTick = delta.TotalSeconds;

                _secondsSinceLastHourAdvance += secondsSinceLastTick;

                // Check if the seconds since the last hour advance are enough to advance in-game time by at least one hour
                int hoursPassed = (int)Math.Floor(_secondsSinceLastHourAdvance / (_ticksRequiredToAdvanceHour * _tickInterval));

                if (hoursPassed > 0)
                {
                    AdvanceTime(hoursPassed);
                    _totalTicksInCurrentLocation += hoursPassed * _ticksRequiredToAdvanceHour; // Increment total ticks in location based on hours passed
                    _secondsSinceLastHourAdvance = 0; // Reset the counter after advancing time

                    // Because we advanced time, check if any location time triggers should fire
                    foreach (var trigger in _locationTimeTriggers)
                    {
                        if (trigger.CheckTrigger(_currentLocation, _totalTicksInCurrentLocation))
                        {
                            EventBus.Publish(trigger.EventToRaise);
                        }
                    }
                }
            }
        }

        private void TickQuestTimeTracking(TimeSpan delta)
        {
            foreach (var quest in _quests)
            {
                if (quest.Status == QuestStatus.InProgress)
                {
                    quest.TickQuestTime(delta, this);
                    switch (quest.Status)
                    {
                        case QuestStatus.Completed:
                            IOService.Output.DisplayDebugMessage($"[GameStateManager] Quest Completed: {quest.Name} (ID: {quest.ID})", Globals.Enums.ConsoleMessageTypes.INFO);
                            break;
                        case QuestStatus.Failed:
                            IOService.Output.DisplayDebugMessage($"[GameStateManager] Quest Failed: {quest.Name} (ID: {quest.ID})", Globals.Enums.ConsoleMessageTypes.WARNING);
                            break;
                    }
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
        public TimeSpan GetLiveTimeInCurrentLocation()
        {
            return _realTimeInCurrentLocation;
        }

        public void OnPlayerEnterLocation(Location location)
        {
            if (_currentLocation != null)
            {
                if (!_locationDurations.ContainsKey(_currentLocation))
                    _locationDurations[_currentLocation] = TimeSpan.Zero;

                _locationDurations[_currentLocation] += _realTimeInCurrentLocation;
            }

            _currentLocation = location;
            _locationEnteredTime = DateTime.UtcNow;
            _realTimeInCurrentLocation = TimeSpan.Zero;

            if (!_locationDurations.ContainsKey(_currentLocation))
                _locationDurations[_currentLocation] = TimeSpan.Zero;

            if (GameContext.Player == null)
            {
                throw new InvalidOperationException("GameContext.Player is null. Cannot set current location.");
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location), "Location cannot be null.");
            }

            if (GameContext.Player.CurrentScene == null)
            {
                throw new InvalidOperationException("GameContext.Player.CurrentScene is null. Cannot set current location.");
            }

            if (GameContext.Player.CurrentScene.Locations == null)
            {
                throw new InvalidOperationException("GameContext.Player.CurrentScene.Locations is null. Cannot set current location.");
            }

            if (!GameContext.Player.CurrentScene.Locations.Contains(_currentLocation))
            {
                // We've moved to a new scene
                // Increment the scene number
                if (!TryDecrementCounter(StateKeys.Counters.Player.CurrentSceneNo))
                {
                    SetCounter(StateKeys.Counters.Player.CurrentSceneNo, 0);
                }
                // Change the player's scene
                GameContext.Player.MoveTo(GameContext.Player.CurrentLocation.Scene);
            }
        }

        public Location? CurrentLocation => _currentLocation;

        public void AddLocationTimeTrigger(LocationTimeTrigger trigger)
        {
            _locationTimeTriggers.Add(trigger);
        }

        #endregion Location Time Tracking
    }
}
