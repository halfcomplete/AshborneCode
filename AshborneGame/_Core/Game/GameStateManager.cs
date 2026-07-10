
using AshborneGame._Core._Player;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.QuestManagement;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.SaveSystem.Data;
using AshborneGame._Core.SaveSystem.Serialisation;

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
        public Dictionary<string, BOCSObject> Masks { get; private set; } = new();

        public TimeTracker TimeTracker { get; private set; }

        private Player _player;

        private List<Quest> _quests = new List<Quest>();

        public GameStateManager(Player player)
        {
            _player = player;
        }

        public void InitialiseMasks(Dictionary<string, BOCSObject> masks)
        {
            Masks = masks;
        }

        

        #region Flags
        public void SetFlag(GameStateKey<bool> key, bool value) => Flags[key] = value;

        /// <summary>
        /// Gets the value of the provided flag name.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>A bool? value, true if the flag is true, false if the flag is false, null if the flag doesn't exist.</returns>
        public bool TryGetFlag(GameStateKey<bool> key, out bool value) 
        {
            return Flags.TryGetValue(key, out value);
        }

        /// <summary>
        /// Gets whether the flag exists.
        /// </summary>
        public bool HasFlag(GameStateKey<bool> key) => Flags.ContainsKey(key);

        /// <summary>
        /// Removes the flag.
        /// </summary>
        public void RemoveFlag(GameStateKey<bool> key) => Flags.Remove(key);

        /// <summary>
        /// Toggles a flag
        /// 
        /// 
        /// If it was true, this sets it to false, else it's set to true.
        /// </summary>
        /// <returns>True if the flag is now true. False if the flag is now false. Null if the flag doesn't exist.</returns>
        public bool? TryToggleFlag(GameStateKey<bool> key)
        {
            if (Flags.ContainsKey(key))
            {
                Flags[key] = !Flags[key];
                return true;
            }
            else
                return null;
        }

        #endregion

        #region Counters
        public void SetCounter(GameStateKey<int> key, int value) => Counters[key] = value;

        /// <summary>
        /// Gets the value of a counter.
        /// </summary>
        /// <returns>True if it was successful, false if not. Out integer value.</returns>
        public bool TryGetCounter(GameStateKey<int> key, out int value)
        {
            return Counters.TryGetValue(key, out value);
        }


        /// <summary>
        /// Increments a counter by amount.
        /// </summary>
        /// <returns>True if it was successful. False otherwise.</returns>
        public bool TryIncrementCounter(GameStateKey<int> key, int amount = 1)
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
        public bool TryDecrementCounter(GameStateKey<int> key, int amount = 1)
        {
            if (!TryGetCounter(key, out var baseValue))
            {
                return false;
            }
            Counters[key] = (int)baseValue - amount;
            if (Counters[key] < 0) Counters[key] = 0;
            return true;
        }

        public bool HasCounter(GameStateKey<int> key) => Counters.ContainsKey(key);

        public bool RemoveCounter(GameStateKey<int> key) => Counters.Remove(key);

        #endregion
        
        #region Labels
        public void SetLabel(GameStateKey<string> key, string value) => Labels[key] = value;
        public string? TryGetLabel(GameStateKey<string> key)
        {
            if (Labels.TryGetValue(key, out string? value))
                return value;
            else
                return null;
        }

        public bool HasLabel(GameStateKey<string> key) => Labels.Keys.Contains(key);

        public bool RemoveLabel(GameStateKey<string> key) => Labels.Remove(key);

        #endregion
        
        #region Masks
        public bool TryGivePlayerMask(string maskName)
        {
            try
            {
                _player.Inventory.TryAddItem(Masks[maskName]);
                return true;
            }
            catch (KeyNotFoundException e)
            {
                IOService.Output.DisplayDebugMessage("[GameStateManager] Attempted to give player a mask that doesn't exist in Masks dictionary: " + maskName + ". StackTrace: " + e.StackTrace, Globals.Enums.ConsoleMessageTypes.ERROR);
                return false;
            }
        }

        public bool TryTakePlayerMask(string maskName)
        {
            if (_player.Inventory.GetItem(maskName) != null)
            {
                _player.Inventory.TryRemoveItem(Masks[maskName]);
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
            Console.WriteLine("[GameStateManager] Forcing player to wear mask:" + maskName);
            Console.WriteLine("Masks:" + string.Join(", ", Masks.Keys));
            _player.EquipItem(Masks[maskName], "face");
            _player.Inventory.TryAddItem(Masks[maskName]);

            if (!OperatingSystem.IsBrowser())
            {
                return;
            }

            if (maskName == MaskNameConstants.Ossaneth)
            {
                Console.WriteLine("[GameStateManager] Player is wearing Ossaneth mask. Starting Ossaneth timer.");
                GameContext.InkRunner.StartOssanethTimer.Invoke();
            }
            else GameContext.InkRunner.StopOssanethTimer.Invoke();
        }

        public bool PlayerHasMask(string maskName) => _player.Inventory.Slots.Any(s => s.Item.Name.Matches(maskName));

        public bool PlayerWearingMask(string maskName)
        {
            return _player.EquippedItems["face"] != null && _player.EquippedItems["face"]!.Name.Matches(maskName);
        }

        #endregion


        public void OnPlayerEnterLocation(Location location)
        {
            

            GameContext.TimeTracker.OnPlayerEnterLocation(location);
            GameContext.AmbientTimeManager?.OnEnterLocation(location);

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

            if (!GameContext.Player.CurrentScene.Locations.Contains(location))
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


        #region Location Visit Tracking
        /// <summary>
        /// Gets the visit count for a location by its ID.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <exception cref="KeyNotFoundException">Thrown when the given location ID is not found in the LocationRegistry.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the location with the given ID is null in the LocationRegistry.</exception>
        public int GetLocationVisitCount(DefinitionID id)
        {
            if (!GameContext.LocationRegistry.TryGetLocationByDefinitionID(id, out var location))
            {
                var knownIds = string.Join(", ", GameContext.LocationRegistry.GetLocationIDs());
                throw new KeyNotFoundException($"Location with ID '{id.Value}' not found in LocationRegistry. Known location IDs: [{knownIds}]");
            }

            if (location == null)
            {
                throw new InvalidOperationException($"Location with ID '{id.Value}' is null in LocationRegistry.");
            }

            return location.VisitCount;
        }

        /// <summary>
        /// Increments the visit count for a location by its ID.
        /// </summary>
        /// <param name="id">The ID of the location.</param>
        /// <returns>The new visit count after incrementing.</returns>
        /// <exception cref="KeyNotFoundException">Thrown when the given location ID is not found in the LocationRegistry.</exception>
        /// <exception cref="InvalidOperationException">Thrown when the location with the given ID is null in the LocationRegistry.</exception>
        public int IncrementLocationVisitCount(DefinitionID id)
        {
            if (!GameContext.LocationRegistry.TryGetLocationByDefinitionID(id, out var location))
            {
                var knownIds = string.Join(", ", GameContext.LocationRegistry.GetLocationIDs());
                throw new KeyNotFoundException($"Location with ID '{id.Value}' not found in LocationRegistry. Known location IDs: [{knownIds}]");
            }

            if (location == null)
            {
                throw new InvalidOperationException($"Location with ID '{id.Value}' is null in LocationRegistry.");
            }

            location.VisitCount += 1;
            return location.VisitCount;
        }

        #endregion

        #region Utilities
        public void ClearAll()
        {
            Flags.Clear();
            Counters.Clear();
            Labels.Clear();
            Masks.Clear();
        }

        public override string ToString()
        {
            return $"[Flags: {Flags.Count}, Counters: {Counters.Count}, Labels: {Labels.Count}, Masks: {Masks.Count}]";
        }
        #endregion
    
        
        public GameStateSaveData GetSaveData()
        {
            Dictionary<string, InstanceID> masks = new();

            foreach (var kvp in Masks)
            {
                masks[kvp.Key] = kvp.Value.InstanceID;
            }

            return new GameStateSaveData
            {
                Flags = new Dictionary<string, bool>(Flags),
                Counters = new Dictionary<string, int>(Counters),
                Labels = new Dictionary<string, string>(Labels),
                Masks = masks,
                TimeTracker = TimeTracker.GetSaveData()
            };
        }

        public void LoadSaveData(GameStateSaveData data, SaveLoadContext context)
        {
            Flags = new Dictionary<string, bool>(data.Flags);
            Counters = new Dictionary<string, int>(data.Counters);
            Labels = new Dictionary<string, string>(data.Labels);
            Masks.Clear();
            foreach (var kvp in data.Masks)
            {
                if (context.InstanceRegistry.TryGet(kvp.Value, out var obj) && obj != null)
                {
                    Masks[kvp.Key] = obj;
                }
                else
                {
                    throw new InvalidOperationException($"Failed to load mask '{kvp.Key}' with InstanceID '{kvp.Value}'. Object not found in InstanceRegistry.");
                }
            }
            TimeTracker = TimeTracker.LoadFromSaveData(data.TimeTracker, GameContext.LocationRegistry);
        }
    }
}
