
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.SceneManagement;
using Ink.Runtime;
using System.ComponentModel.DataAnnotations;
using System.Diagnostics;

namespace AshborneGame._Core.Game
{
    /// <summary>
    /// Centralised state manager for tracking global flags, counters, and custom variables.
    /// Ideal for quest logic, event triggers, and persistent world state.
    /// </summary>
    public class GameStateManager
    {
        public Dictionary<string, bool> Flags { get; private set; } = new();
        public Dictionary<string, int> Counters { get; private set; } = new();
        public Dictionary<string, string> Labels { get; private set; } = new();
        public Dictionary<string, object> Variables { get; private set; } = new();
        public Dictionary<string, Item> Masks { get; private set; } = new();
        private Player _player;

        private readonly Dictionary<Location, TimeSpan> _locationDurations = new();
        private Location? _currentLocation;
        private DateTime _locationEnteredTime;
        private TimeSpan _realTimeInCurrentLocation = TimeSpan.Zero;
        private DateTime _lastTickTime = DateTime.UtcNow;
        private List<LocationTimeTrigger> _locationTimeTriggers = new();

        private CancellationTokenSource? _tickCancellation;
        private Task? _tickTask;
        private bool _tickRunning = false;

        public GameStateManager(Player player)
        {
            _player = player;
        }

        public void InitialiseMasks(Dictionary<string, Item> masks)
        {
            Masks = masks;
        }

        public void Tick()
        {
            var now = DateTime.UtcNow;
            var delta = now - _lastTickTime;
            _lastTickTime = now;

            if (_currentLocation != null)
                _realTimeInCurrentLocation += delta;

            foreach (var trigger in _locationTimeTriggers)
            {
                if (trigger.CheckTrigger(GameContext.Player.CurrentLocation, _realTimeInCurrentLocation))
                {
                    // Call the event
                    EventBus.Call(trigger.EventToRaise);

                    // Execute optional effect (e.g. dialogue)
                    trigger.Effect?.Invoke();
                }
            }

        }

        public void StartTickLoop(int tickIntervalMs = 1000)
        {
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

        public void StopTickLoop()
        {
            if (_tickCancellation == null || !_tickRunning)
                return;

            _tickCancellation.Cancel();
            _tickTask?.Wait(); // Optional: wait for cleanup
            _tickCancellation.Dispose();
            _tickCancellation = null;
            _tickTask = null;
        }


        // ----------- FLAGS (True/False binary states) -----------

        public void SetFlag(string key, bool value) => Flags[key] = value;

        /// <summary>
        /// Gets the value of the provided flag name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A bool? value, true if the flag is true, false if the flag is false, null if the flag doesn't exist.</returns>
        public bool TryGetFlag(string key, out bool value) 
        {
            return Flags.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets whether the flag exists.
        /// </summary>
        public bool HasFlag(string key) => Flags.ContainsKey(key);

        /// <summary>
        /// Removes the flag.
        /// </summary>
        public void RemoveFlag(string key) => Flags.Remove(key);

        /// <summary>
        /// Toggles a flag
        /// 
        /// 
        /// If it was true, this sets it to false, else it's set to true.
        /// </summary>
        /// <returns>True if the flag is now true. False if the flag is now false. Null if the flag doesn't exist.</returns>
        public bool? TryToggleFlag(string key)
        {
            if (Flags.ContainsKey(key))
            {
                Flags[key] = !Flags[key];
                return true;
            }
            else
                return null;
        }

        // ----------- COUNTERS (Integers with mutations) -----------

        public void SetCounter(string key, int value) => Counters[key] = value;

        /// <summary>
        /// Gets the value of a counter.
        /// </summary>
        /// <returns>True if it was successful, false if not. Out integer value.</returns>
        public bool TryGetCounter(string key, out int value)
        {
            return Counters.TryGetValue(key, out value);
        }


        /// <summary>
        /// Increments a counter by amount.
        /// </summary>
        /// <returns>True if it was successful. False otherwise.</returns>
        public bool TryIncrementCounter(string key, int amount = 1)
        {
            if (!TryGetCounter(key, out var baseValue))
            {
                return false;
            }
            Counters[key] = (int)baseValue + amount;
            return true;
        }

        /// <summary>
        /// Decrements a counter by an amount. Sets it to 0 if it goes below 0.
        /// </summary>
        /// <returns>True if it was successful. False otherwise.</returns>
        public bool TryDecrementCounter(string key, int amount = 1)
        {
            if (!TryGetCounter(key, out var baseValue))
            {
                return false;
            }
            Counters[key] = (int)baseValue - amount;
            if (Counters[key] < 0) Counters[key] = 0;
            return true;
        }

        public bool HasCounter(string key) => Counters.ContainsKey(key);

        public bool RemoveCounter(string key) => Counters.Remove(key);

        // ----------- LABELS (String storage) ----------

        public void SetLabel(string key, string value) => Labels[key] = value;
        public string? TryGetLabel(string key)
        {
            if (Labels.TryGetValue(key, out string? value))
                return value;
            else
                return null;
        }

        public bool HasLabel(string key) => Labels.Keys.Contains(key);

        public bool RemoveLabel(string key) => Labels.Remove(key);

        // ----------- VARIABLES (Generic object storage) -----------

        public void SetVariable(string key, object value) => Variables[key] = value;

        /// <summary>
        /// Attempts to get the value of the variable.
        /// </summary>
        /// <returns>The value if successful, null otherwise.</returns>
        public object? TryGetVariable(string key) =>
            Variables.TryGetValue(key, out var value) ? value : null;

        /// <summary>
        /// Attempts to get the value of variable provided it is the target 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="key"></param>
        /// <returns></returns>
        public T? GetVariable<T>(string key)
        {
            if (Variables.TryGetValue(key, out var value) && value is T typedValue)
                return typedValue;
            return default;
        }

        public bool HasVariable(string key) => Variables.ContainsKey(key);

        public void RemoveVariable(string key) => Variables.Remove(key);

        // ----------- MASKS -----------
        public void GivePlayerMask(string maskName)
        {
            _player.Inventory.AddItem(Masks[maskName]);
        }

        public bool TryTakePlayerMask(string maskName)
        {
            if (_player.Inventory.GetItem(maskName) != null)
            {
                _player.Inventory.RemoveItem(Masks[maskName]);
                if (PlayerWearingMask(maskName)) _player.EquippedItems["face"] = null;
                return true;
            }
            else
            {
                return false;
            }
        }

        public void ForcePlayerMask(string maskName)
        {
            _player.EquippedItems["face"] = null;
            _player.EquippedItems["face"] = Masks[maskName];
            _player.Inventory.AddItem(Masks[maskName]);

            if (!OperatingSystem.IsBrowser())
            {
                return;
            }

            if (maskName == MaskNameConstants.Ossaneth) GameContext.InkRunner.StartOssanethTimer.Invoke();
            else GameContext.InkRunner.StopOssanethTimer.Invoke();
        }

        public bool PlayerHasMask(string maskName) => _player.Inventory.Slots.Any(s => s.Item.Name == maskName);

        public bool PlayerWearingMask(string maskName)
        {
            return _player.EquippedItems["face"] != null && _player.EquippedItems["face"]!.Name == maskName;
        }

        // ----------- TIME TRACKING -----------
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
                if (!TryDecrementCounter(GameStateKeyConstants.Counters.Player.CurrentSceneNo))
                {
                    SetCounter(GameStateKeyConstants.Counters.Player.CurrentSceneNo, 0);
                }
                // Change the player's scene
                GameContext.Player.MoveTo(GameContext.Player.CurrentLocation.Scene);
            }

            // Check if this is a Dreamspace location and track it
            CheckDreamspaceLocationProgress(location);
        }

        private void CheckDreamspaceLocationProgress(Location location)
        {
            // Only track locations in Ossaneth's Domain (Dreamspace)
            if (location.Scene?.DisplayName != "Ossaneth's Domain")
                return;

            // Skip Eye Platform as specified
            if (location.Name.ReferenceName == "Eye Platform")
                return;

            // Set flag for this specific location being visited
            string locationFlagKey = $"Flags.Player.Actions.In.OssanethDreamspace_Visited{location.Name.ReferenceName.Replace(" ", "")}";
            SetFlag(locationFlagKey, true);

            // Count visited Dreamspace locations (excluding Eye Platform)
            int visitedCount = 0;
            var dreamspaceLocations = new[] { "Hall of Mirrors", "Chamber of Cycles", "Temple of the Bound", "Throne Room" };
            
            foreach (var locName in dreamspaceLocations)
            {
                string flagKey = $"Flags.Player.Actions.In.OssanethDreamspace_Visited{locName.Replace(" ", "")}";
                if (TryGetFlag(flagKey, out bool visited) && visited)
                {
                    visitedCount++;
                }
            }

            // Check if we've reached the threshold (2 locations excluding Eye Platform)
            if (visitedCount >= 2)
            {
                // Check if we've already triggered the outro to avoid duplicates
                if (!TryGetFlag("Flags.Player.Actions.In.OssanethDreamspace_OutroTriggered", out bool outroTriggered) || !outroTriggered)
                {
                    SetFlag("Flags.Player.Actions.In.OssanethDreamspace_OutroTriggered", true);
                    
                    // Publish event for the outro dialogue
                    var outroEvent = new GameEvent("player.dreamspace.outro.triggered", new Dictionary<string, object>
                    {
                        { "visited_count", visitedCount },
                        { "location_name", location.Name.ReferenceName }
                    });
                    EventBus.Call(outroEvent);
                }
            }
        }


        public Location? CurrentLocation => _currentLocation;

        public void AddLocationTimeTrigger(LocationTimeTrigger trigger)
        {
            _locationTimeTriggers.Add(trigger);
        }

        // ----------- UTILITIES -----------

        public void ClearAll()
        {
            Flags.Clear();
            Counters.Clear();
            Variables.Clear();
        }

        public override string ToString()
        {
            return $"[Flags: {Flags.Count}, Counters: {Counters.Count}, Variables: {Variables.Count}]";
        }
    }
}
