using AshborneGame._Core.Data.BOCS.ItemSystem;
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

namespace AshborneGame._Core._Player
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
        public Scene CurrentScene { get; private set; }

        /// <summary>
        /// Gets the player's current location.
        /// </summary>
        public Location CurrentLocation { get; private set; }

        public Location? PreviousLocation { get; private set; } = null;

        public Sublocation? CurrentSublocation { get; private set; } = null;

        /// <summary>
        /// Gets the player's inventory.
        /// </summary>
        public Inventory Inventory { get; }

        /// <summary>
        /// Gets or sets the inventory of the container that is currently opened.
        /// </summary>
        public Inventory? OpenedInventory { get; set; }

        public NPC? CurrentNPCInteraction { get; set; } = null;

        public Item? CurrentMask { get; set; } = null;


        /// <summary>
        /// Gets or sets the items equipped by the player, either on the hand, offhand, head, body, or feet.
        /// </summary>
        public Dictionary<string, Item?> EquippedItems { get; set; } = new Dictionary<string, Item?>()
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
            CurrentLocation = LocationFactory.CreateLocation(
                new Location(descriptor, "Locations.test-location-" + Guid.NewGuid().ToString("N")[..8]),
                new LookDescription(),
                new VisitDescription("You enter a new place.", "You are here again.", "You have been here many times."),
                new SensoryDescription("A generic location.", "You hear ambient sounds.")
            );
            CurrentScene = new Scene("test location group", "Test Location Group");
            CurrentScene.AddLocation(CurrentLocation);
            Inventory = new Inventory();
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
            CurrentScene = new Scene("test location group", "Test Location Group");
            CurrentScene.AddLocation(CurrentLocation);
            Inventory = new Inventory();
        }

        public Player(string name)
        {
            _name = name;
            var descriptor = new LocationNameAdapter("Placeholder");
            // Use unique ID to avoid registry collisions in tests
            CurrentLocation = LocationFactory.CreateLocation(
                new Location(descriptor, "Locations.placeholder-" + Guid.NewGuid().ToString("N")[..8]),
                new LookDescription(),
                new VisitDescription("You enter a placeholder place.", "You are at the placeholder place again.", "You have been here many times."),
                new SensoryDescription("A placeholder location.", "You hear placeholder sounds.")
            );
            CurrentScene = new Scene("Placeholder", "Placeholder");
            CurrentScene.AddLocation(CurrentLocation);
            Inventory = new Inventory();
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
            CurrentScene = new Scene("test location group", "Test Location Group");
            CurrentScene.AddLocation(CurrentLocation);
            Inventory = new Inventory();
        }

        /// <summary>
        /// Moves the player to a location. Sets sublocation to null.
        /// </summary>
        /// <param name="newLocation">The sublocation to move to.</param>
        /// <exception cref="ArgumentNullException">Thrown when newLocation is null.</exception>
        public async void MoveTo(Location newLocation)
        {
            if (CurrentSublocation == null)
            {
                GameContext.GameState.OnPlayerEnterLocation(newLocation);
            }
            PreviousLocation = CurrentLocation;
            CurrentLocation = newLocation;
            CurrentSublocation = null;

            // Increment visit count BEFORE description
            CurrentLocation.VisitCount++;
            Console.WriteLine($"Player moved to {CurrentLocation.Name.DisplayName}. Visit count: {CurrentLocation.VisitCount}");

            // Fire event if Ossaneth Domain and visit count == 4
            bool sceneMatch = newLocation.Scene != null && newLocation.Scene.DisplayName == "Ossaneth's Domain";
            bool nameMatch = newLocation.Name.ReferenceName == "Eye Platform";
            bool visitMatch = CurrentLocation.VisitCount == 3;
            Console.WriteLine($"[MoveTo] Location={newLocation.Name.DisplayName} Scene={(newLocation.Scene?.DisplayName ?? "<null>")} VisitCount={CurrentLocation.VisitCount} sceneMatch={sceneMatch} nameMatch={nameMatch} visitMatch={visitMatch}", ConsoleMessageTypes.INFO);
            if (sceneMatch && nameMatch && visitMatch)
            {
                var evt = new GameEvents.OssanethsDomain.EyePlatformVisitThresholdEvent(newLocation.Name.ReferenceName, CurrentLocation.VisitCount);
                EventBus.Publish(evt);
                await IOService.Output.DisplayDebugMessage($"[MoveTo] Fired EyePlatformVisitThresholdEvent", ConsoleMessageTypes.INFO);
            }

            // Suppress description on the special 4th Eye Platform visit (outro) so dialogue leads
            if (!(sceneMatch && nameMatch && visitMatch))
            {
                await IOService.Output.WriteNonDialogueLine(newLocation.GetDescription(this, GameContext.GameState));
            }
        }

        /// <summary>
        /// Same as MoveTo, but prints 'You are back at {location name}.'
        /// Does NOT increment visit count (force move).
        /// </summary>
        public async void ForceMoveTo(Location newLocation)
        {
            if (CurrentSublocation == null)
            {
                // If the player is not in a sublocation
                GameContext.GameState.OnPlayerEnterLocation(newLocation);
            }
            CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            CurrentSublocation = null;
            
            // Force move does NOT increment visit count
            
            await IOService.Output.WriteNonDialogueLine($"You are back at {CurrentLocation.Name.DisplayName}.\n\n");
        }

        public async void MoveTo(Sublocation newSublocation)
        {
            CurrentSublocation = newSublocation;
            
            // Increment visit count for sublocation movement BEFORE description
            CurrentSublocation.VisitCount++;

            // Get description AFTER incrementing visit count
            await IOService.Output.WriteNonDialogueLine(newSublocation.GetDescription(this, GameContext.GameState));
        }

        public async void MoveTo(Scene newScene)
        {
            CurrentScene = newScene;
            if (OperatingSystem.IsBrowser())
            {
                // In Blazor, don't write the scene header to the console
                // Instead, the scene header is displayed in the UI
                return;
            }
            await IOService.Output.WriteNonDialogueLine(newScene.GetHeader());
        }

        public async void SetupMoveTo(Location newLocation, Scene newScene, bool displayDescription = true)
        {
            await IOService.Output.DisplayDebugMessage($"Set up move to location: {newLocation.Name} in scene: {newScene.DisplayName}");
            CurrentScene = newScene;
            CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            CurrentSublocation = null;
            GameContext.GameState.OnPlayerEnterLocation(newLocation);
            
            // Setup move sets visit count to 1 (first visit)
            CurrentLocation.VisitCount = 1;

            if (displayDescription) await IOService.Output.WriteNonDialogueLine(CurrentLocation.GetDescription(GameContext.Player, GameContext.GameState));

            // In Blazor, don't write the scene header to the console
            // Instead, the scene header is displayed in the UI
            if (OperatingSystem.IsBrowser())
            {
                return;
            }
            await IOService.Output.WriteNonDialogueLine(newScene.GetHeader());
        }

        /// <summary>
        /// Moves the player based on parsed input.
        /// </summary>
        /// <param name="parsedInput">The parsed input containing the direction or location to move to.</param>
        public async Task<bool> TryMoveTo(List<string> parsedInput)
        {
            if (parsedInput == null || parsedInput.Count == 0)
            {
                throw new ArgumentException("Parsed input cannot be null or empty.", nameof(parsedInput));
            }

            string place = string.Join(" ", parsedInput).ToLower().Trim();

            await IOService.Output.DisplayDebugMessage("Move to... " + place, ConsoleMessageTypes.INFO);

            // Directional movement
            if (DirectionConstants.CardinalDirections.Contains(place) || place.Equals(DirectionConstants.Back))
            {
                return await TryMoveDirectionally(place);
            }

            // Sublocation movement
            if (CurrentLocation.Sublocations.Any(s => s.Name.Matches(place)))
            {
                var sublocation = CurrentLocation.Sublocations.First(s => s.Name.Matches(place));
                MoveTo(sublocation);
                return true;
            }

            // Location movement (within current scene)
            if (CurrentScene.Locations.Any(l => l.Name.Matches(place)))
            {
                var location = CurrentScene.Locations.First(l => l.Name.Matches(place));
                if (location == CurrentLocation)
                {
                    await IOService.Output.WriteNonDialogueLine(location.GetDescription(this, GameContext.GameState));
                    return true;
                }
                MoveTo(location);
                return true;
            }

            // Already at location/sublocation
            if (CurrentLocation.Name.Matches(place))
            {
                await IOService.Output.WriteNonDialogueLine(CurrentLocation.GetDescription(this, GameContext.GameState));
                return true;
            }
            if (CurrentSublocation != null && CurrentSublocation.Name.Matches(place))
            {
                await IOService.Output.WriteNonDialogueLine(CurrentSublocation.GetDescription(this, GameContext.GameState));
                return true;
            }

            // If nothing matches, try custom sublocation movement
            return TryMoveToSublocation(place);
        }

        private async Task<bool> TryMoveDirectionally(string direction)
        {
            if (CurrentSublocation != null)
            {
                // Only 'back' is supported in sublocations
                // TODO: Implement 'back' functionality for locations, which returns the player to the previous location
                if (direction == "back" && CurrentSublocation.Exits.ContainsKey("back"))
                {
                    MoveTo(CurrentSublocation.Exits["back"]);
                    return true;
                }
                await IOService.Output.DisplayFailMessage("You can't go that way from here.");
                return false;
            }
            else
            {
                if (direction == "back")
                {
                    if (PreviousLocation != null)
                    {
                        MoveTo(PreviousLocation);
                        return true;
                    }
                }
                if (CurrentLocation.Exits.TryGetValue(direction, out var newLocation))
                {
                    MoveTo(newLocation);
                    return true;
                }
                // Provide debug info on available exits to aid troubleshooting
                var available = string.Join(", ", CurrentLocation.Exits.Keys);
                await IOService.Output.DisplayDebugMessage($"No exit '{direction}' from {CurrentLocation.Name.DisplayName}. Available: [{available}]", ConsoleMessageTypes.WARNING);
                await IOService.Output.DisplayFailMessage("You can't go that way.");
                return false;
            }
        }

        private bool TryMoveToSublocation(string place)
        {
            // Get the sublocation from sublocation list in the current location by name
            var sublocation = CurrentLocation.Sublocations
                .FirstOrDefault(s => s.Name.Matches(place));

            if (sublocation != null)
            {
                MoveTo(sublocation);
                return true;
            }

            return false;
        }

        public async void EquipItem(Item item, string slot)
        {
            (bool hasEquippableBehaviour, var equippableBehaviour) = await item.TryGetBehaviour<IEquippable>();
            if (hasEquippableBehaviour && equippableBehaviour.EquipInfo.IsEquippable)
            {
                await IOService.Output.DisplayDebugMessage($"Attempting to equip {item} on player's {slot}.", ConsoleMessageTypes.INFO);
                if (!equippableBehaviour.EquipInfo.BodyParts.Contains(slot.ToLower()))
                {
                    throw new ArgumentException($"Item cannot be equipped in the {slot} slot. Valid slots are: {string.Join(", ", equippableBehaviour.EquipInfo.BodyParts)}");
                }
                else if (EquippedItems.TryGetValue(slot.ToLower(), out var _item) && _item != null)
                {
                    // If the slot is already occupied, unequip the current item
                    UnequipItem(EquippedItems[slot.ToLower()]!, slot);
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

        public void UnequipItem(Item item, string slot)
        {
            if (!EquippedItems.ContainsKey(slot.ToLower()))
            {
                throw new ArgumentException($"Invalid equipment slot: {slot}. Valid slots are: {string.Join(", ", EquippedItems.Keys)}");
            }
            EquippedItems[slot.ToLower()] = null;
        }

        public async void Attack(NPC npc)
        {
            (bool hasAttackableBehaviour, var attackableBehaviour) = await npc.TryGetBehaviour<ICanBeAttacked>();
            if (hasAttackableBehaviour)
            {
                float damage = 0;
                var (baseStrength, bonusStrength, totalStrength) = Stats.GetStat(PlayerStatType.Strength);
                (bool hasDamageBehaviour, var damageBehaviour) = await npc.TryGetBehaviour<ICanDamage>();
                if (EquippedItems.TryGetValue("hand", out var weapon) && weapon != null && hasDamageBehaviour)
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

