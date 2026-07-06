using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using System.Net.Mail;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;

namespace AshborneGame._Core._Player
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player : ISentientEntity
    {
        private InstanceID _instanceID = new();
        private DefinitionID _definitionID = new("Player");

        public PsychologicalState PsychologicalState { get; }

        public Scene CurrentScene { get; private set; }

        /// <summary>
        /// Gets the player's current location.
        /// </summary>
        public Location CurrentLocation { get; private set; }

        public Location? PreviousLocation { get; private set; } = null;

        /// <summary>
        /// Gets the player's inventory.
        /// </summary>
        public Inventory Inventory { get; }

        /// <summary>
        /// Gets or sets the inventory of the container that is currently opened.
        /// </summary>
        public Inventory? OpenedInventory { get; set; }

        public BOCSObject? CurrentNPCInteraction { get; set; } = null;

        public BOCSObject? CurrentMask { get; set; } = null;

        // TODO: maybe make more type-safe
        /// <summary>
        /// Gets or sets the items equipped by the player, either on the hand, offhand, head, body, or feet.
        /// </summary>
        public Dictionary<string, BOCSObject?> EquippedItems { get; set; } = new Dictionary<string, BOCSObject?>()
        {
            { "face", null },
            { "hand", null },
            { "offhand", null },
            { "head", null },
            { "body", null },
            { "feet", null }
        };

        /// <summary>
        /// Gets or sets the player's visibility level.
        /// </summary>
        public int Visibility { get; private set; } = 5;

        public StatCollection Stats { get; } = new StatCollection();

        private readonly string _name;

        public Player()
        {
            _name = "Hero"; // Default name
            var descriptor = new LocationNameAdapter("test location");
            // Use unique ID to avoid registry collisions in tests
            // HACK: fix the def id
            CurrentLocation = new Location(
                new DefinitionID("Locations.test-location-" + Guid.NewGuid().ToString("N")[..8]),
                descriptor,
                new(
                    new LookDescription(),
                    new VisitDescription("You enter a new place.", "You are here again.", "You have been here many times."),
                    new SensoryDescription("A generic location.", "You hear ambient sounds.")),
                new(),
                new(),
                new(),
                new()
            );
            CurrentScene = new Scene(new DefinitionID("TestScene"), "Test Scene", [CurrentLocation]);

            GameContext.LocationRegistry.RegisterScene(CurrentScene);
            GameContext.LocationRegistry.RegisterLocation(CurrentLocation);

            Inventory = new Inventory();
            PsychologicalState = new(_definitionID);
		}

		/// <summary>
		/// Initialises a new instance of the Player class with a default name.
		/// </summary>
		/// <param name="startingLocation">The location where the player starts.</param>
		/// <exception cref="ArgumentNullException">Thrown when startingLocation is null.</exception>
		public Player(Location startingLocation)
        {
            _name = "Hero"; // Default name
            CurrentLocation = startingLocation ?? throw new ArgumentNullException(nameof(startingLocation));
            CurrentScene = new Scene(new DefinitionID("TestScene"), "Test Scene", [CurrentLocation]);

            GameContext.LocationRegistry.RegisterScene(CurrentScene);
            GameContext.LocationRegistry.RegisterLocation(CurrentLocation);
            Inventory = new Inventory();
            PsychologicalState = new(_definitionID);
        }

        public Player(string name)
        {
            _name = name;
            var descriptor = new LocationNameAdapter("Placeholder");
            // Use unique ID to avoid registry collisions in tests
            Inventory = new Inventory();
            PsychologicalState = new(_definitionID);
        }

        /// <summary>
        /// Initialises a new instance of the Player class with a custom name.
        /// </summary>
        /// <param name="name">The name of the player.</param>
        /// <param name="startingLocation">The location where the player starts.</param>
        /// <exception cref="ArgumentNullException">Thrown when name or startingLocation is null.</exception>
        public Player(string name, Location startingLocation)
        {
            _name = name ?? throw new ArgumentNullException(nameof(name));
            CurrentLocation = startingLocation ?? throw new ArgumentNullException(nameof(startingLocation));
            CurrentScene = new Scene(new DefinitionID("TestScene"), "Test Location Group");
            CurrentScene.AddLocation(CurrentLocation);
            GameContext.LocationRegistry.RegisterScene(CurrentScene);
            GameContext.LocationRegistry.RegisterLocation(CurrentLocation);
            Inventory = new Inventory();
            PsychologicalState = new(_definitionID);
        }

        /// <summary>
        /// Moves the player to the specified location.
        ///
        /// This method is the final step of player movement and is intended to be
        /// called only by <see cref="MovementService"/> after the destination has
        /// been validated and resolved.
        ///
        /// Responsibilities:
        /// <list type="bullet">
        /// <item><description>Updates the player's current and previous locations.</description></item>
        /// <item><description>Notifies the game state that the player entered a new location.</description></item>
        /// <item><description>Increments the visit count.</description></item>
        /// <item><description>Publishes movement-related events.</description></item>
        /// <item><description>Displays the location description.</description></item>
        /// </list>
        /// </summary>
        /// <param name="newLocation">The destination location.</param>
        /// <exception cref="ArgumentNullException">
        /// Thrown if <paramref name="newLocation"/> is null.
        /// </exception>
        public async Task MoveTo(Location newLocation)
        {
            ArgumentNullException.ThrowIfNull(newLocation);

            PreviousLocation = CurrentLocation;
            CurrentLocation = newLocation;

            GameContext.GameState.OnPlayerEnterLocation(newLocation);

            CurrentLocation.VisitCount++;

            await IOService.Output.DisplayDebugMessage(
                $"Player moved to {CurrentLocation.Name.DisplayName}. Visit count: {CurrentLocation.VisitCount}");

            bool sceneMatch =
                newLocation.Scene?.DisplayName == "Ossaneth's Domain";

            bool locationMatch =
                newLocation.Name.ReferenceName == "Eye Platform";

            bool thresholdReached =
                CurrentLocation.VisitCount == 3;

            if (sceneMatch && locationMatch && thresholdReached)
            {
                EventBus.Publish(
                    new GameEvents.OssanethsDomain.EyePlatformVisitThresholdEvent(
                        GameContext.TimeTracker.TotalInGameHours,
                        newLocation.Name.ReferenceName,
                        CurrentLocation.VisitCount));
            }

            if (!(sceneMatch && locationMatch && thresholdReached))
            {
                await IOService.Output.WriteNonDialogueLine(
                    CurrentLocation.GetDescription(this, GameContext.GameState));
            }
        }

        /// <summary>
        /// Instantly relocates the player without incrementing visit counts or
        /// triggering movement events.
        ///
        /// Intended for scripted sequences, loading saves and developer tools.
        /// </summary>
        public async Task ForceMoveTo(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            PreviousLocation = CurrentLocation;
            CurrentLocation = location;

            GameContext.GameState.OnPlayerEnterLocation(location);

            await IOService.Output.WriteNonDialogueLine(
                $"You are back at {location.Name.DisplayName}.\n");
        }

        public async Task MoveTo(Scene scene)
        {
            ArgumentNullException.ThrowIfNull(scene);

            CurrentScene = scene;

            if (!OperatingSystem.IsBrowser())
            {
                await IOService.Output.WriteNonDialogueLine(scene.GetHeader());
            }
        }

        public async Task SetupMoveTo(Location location, Scene scene, bool displayDescription = true)
        {
            ArgumentNullException.ThrowIfNull(location);
            ArgumentNullException.ThrowIfNull(scene);

            CurrentScene = scene;
            CurrentLocation = location;

            GameContext.GameState.OnPlayerEnterLocation(location);

            location.VisitCount = 1;

            if (!OperatingSystem.IsBrowser())
            {
                await IOService.Output.WriteNonDialogueLine(scene.GetHeader());
            }

            if (displayDescription)
            {
                await IOService.Output.WriteNonDialogueLine(
                    location.GetDescription(this, GameContext.GameState));
            }
        }

        // TODO: should not access equippable internal state
        // TODO: remove async
        public async void EquipItem(BOCSObject item, string slot)
        {
            (bool hasEquippableBehaviour, var equippableBehaviour) = await item.TryGetBehaviour<IEquippable>();
            if (hasEquippableBehaviour && equippableBehaviour != null)
            {
                await IOService.Output.DisplayDebugMessage($"Attempting to equip {item} on player's {slot}.", ConsoleMessageTypes.INFO);
                if (!equippableBehaviour.EquippableSlots.Contains(slot.ToLower()))
                {
                    throw new ArgumentException($"Item cannot be equipped in the {slot} slot. Valid slots are: {string.Join(", ", equippableBehaviour.EquippableSlots)}");
                }
                else if (EquippedItems.TryGetValue(slot.ToLower(), out var _item) && _item != null)
                {
                    // If the slot is already occupied, unequip the current item
                    UnequipItem(slot);
                    await IOService.Output.WriteNonDialogueLine($"You unequip the {_item.Name} from your {slot}.");

                    // Then equip the item in the specified slot
                    EquippedItems[slot.ToLower()] = item;
                    await IOService.Output.WriteNonDialogueLine($"You equip the {item.Name} on your {slot}.");
                }
                else
                {
                    // Equip the item in the specified slot
                    EquippedItems[slot.ToLower()] = item;
                    await IOService.Output.WriteNonDialogueLine($"You equip the {item.Name} on your {slot}.");
                }
            }
            else
            {
                throw new InvalidOperationException("Item is not equippable.");
            }
        }

        public void UnequipItem(string slot)
        {
            if (!EquippedItems.ContainsKey(slot.ToLower()))
            {
                throw new ArgumentException($"Invalid equipment slot: {slot}. Valid slots are: {string.Join(", ", EquippedItems.Keys)}");
            }
            EquippedItems[slot.ToLower()] = null;
        }

        // TODO: remove async void, check logic
        public async void Attack(BOCSObject npc)
        {
            (bool hasAttackableBehaviour, var attackableBehaviour) = await npc.TryGetBehaviour<ICanBeAttacked>();
            if (hasAttackableBehaviour && attackableBehaviour != null)
            {
                float damage = 0;
                var (baseStrength, bonusStrength, totalStrength) = Stats.GetStat(PlayerStatType.Strength);
                (bool hasDamageBehaviour, var damageBehaviour) = await npc.TryGetBehaviour<ICanDamage>();
                if (EquippedItems.TryGetValue("hand", out var weapon) && weapon != null && hasDamageBehaviour && damageBehaviour != null)
                {
                    damage = (float)(damageBehaviour.BaseDamage + totalStrength * 0.5); // Example damage calculation
                    await IOService.Output.WriteNonDialogueLine($"You attack {npc.Name} with {weapon.Name} for {damage} damage.");
                }
                else
                {
                    // If no weapon is equipped, use bare hands
                    damage = totalStrength;
                    await IOService.Output.WriteNonDialogueLine($"You attack {npc.Name} with your bare hands for {damage} damage.");
                }
                attackableBehaviour.Attacked(damage);
                await IOService.Output.WriteNonDialogueLine($"You attack {npc.Name} for {damage} damage. {npc.Name} now has {attackableBehaviour.Health} health left.");
            }
            else
            {
                await IOService.Output.WriteNonDialogueLine($"{npc.Name} cannot be attacked.");
            }
        }

        /// <summary>
        /// Sets a game variable for the player.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="value">The value to set.</param>
        public async void SetVariable(string variableName, int value)
        {
            switch (variableName.ToLower())
            {
                case "sight":
                    Visibility = value;
                    break;
                default:
                    await IOService.Output.WriteNonDialogueLine($"Variable '{variableName}' is not recognized.");
                    break;
            }
        }

        public async void ChangeHealth(int amount)
        {
            (int baseHealth, _, _) = Stats.GetStat(PlayerStatType.Health);
            (_, _, int totalMaxHealth) = Stats.GetStat(PlayerStatType.MaxHealth);
            Stats.SetBase(PlayerStatType.Health, Math.Clamp(baseHealth + amount, 0, totalMaxHealth));
            await IOService.Output.WriteNonDialogueLine($"You have been healed by {amount} points.");
        }

        public async void SetHealth(int amount)
        {
            (int baseHealth, _, _) = Stats.GetStat(PlayerStatType.Health);
            (_, _, int totalMaxHealth) = Stats.GetStat(PlayerStatType.MaxHealth);
            Stats.SetBase(PlayerStatType.Health, Math.Clamp(baseHealth, 0, totalMaxHealth));
            await IOService.Output.WriteNonDialogueLine($"Your health has been set to {amount} points.");
        }
    }
}

