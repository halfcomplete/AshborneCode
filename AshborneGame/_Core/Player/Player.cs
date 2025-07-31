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
using AshborneGame._Core.SceneManagement;
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
            var descriptor = new LocationIdentifier("test location");
            CurrentLocation = LocationFactory.CreateLocation(
                new Location(descriptor, descriptor.DisplayName),
                new LookDescription(),
                new FadingDescription("You enter a new place.", "You are here again.", "You have been here many times."),
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
            var descriptor = new LocationIdentifier("Placeholder");
            CurrentLocation = LocationFactory.CreateLocation(
                new Location(descriptor, descriptor.DisplayName),
                new LookDescription(),
                new FadingDescription("You enter a placeholder place.", "You are at the placeholder place again.", "You have been here many times."),
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
        public void MoveTo(Location newLocation)
        {
            if (CurrentSublocation == null)
            {
                // If the player is not in a sublocation
                GameContext.GameState.OnPlayerEnterLocation(newLocation);
            }
            CurrentLocation = newLocation;
            CurrentSublocation = null;
            

            IOService.Output.WriteLine(newLocation.GetDescription(this, GameContext.GameState), OutputConstants.DefaultTypeSpeed);
            CurrentLocation.VisitCount++;
        }

        /// <summary>
        /// Same as MoveTo, but prints 'You are back at {location name}.'
        public void ForceMoveTo(Location newLocation)
        {
            if (CurrentSublocation == null)
            {
                // If the player is not in a sublocation
                GameContext.GameState.OnPlayerEnterLocation(newLocation);
            }
            CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            CurrentSublocation = null;
            

            IOService.Output.WriteLine($"You are back at {CurrentLocation.Name.DisplayName}.\n\n", OutputConstants.DefaultTypeSpeed);
            CurrentLocation.VisitCount++;
        }

        public void MoveTo(Sublocation newSublocation)
        {
            CurrentSublocation = newSublocation;
            CurrentSublocation.VisitCount = 0;
            IOService.Output.WriteLine(newSublocation.GetDescription(this, GameContext.GameState), OutputConstants.DefaultTypeSpeed);
        }

        public void MoveTo(Scene newScene)
        {
            CurrentScene = newScene;
            if (OperatingSystem.IsBrowser())
            {
                // In Blazor, don't write the scene header to the console
                // Instead, the scene header is displayed in the UI
                return;
            }
            IOService.Output.WriteLine(newScene.GetHeader());
        }

        public void SetupMoveTo(Location newLocation, Scene newScene)
        {
            Console.WriteLine($"Set up move to location: {newLocation.Name} in scene: {newScene.DisplayName}");
            CurrentScene = newScene;
            CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            CurrentSublocation = null;
            GameContext.GameState.OnPlayerEnterLocation(newLocation);
            if (OperatingSystem.IsBrowser())
            {
                // In Blazor, don't write the scene header to the console
                // Instead, the scene header is displayed in the UI
                return;
            }
            
            IOService.Output.WriteLine(newScene.GetHeader());
            CurrentLocation.VisitCount++;
        }

        /// <summary>
        /// Moves the player based on parsed input.
        /// </summary>
        /// <param name="parsedInput">The parsed input containing the direction or location to move to.</param>
        public bool TryMoveTo(List<string> parsedInput)
        {
            if (parsedInput == null || parsedInput.Count == 0)
            {
                throw new ArgumentException("Parsed input cannot be null or empty.", nameof(parsedInput));
            }

            string place = string.Join(" ", parsedInput).ToLower().Trim();

            IOService.Output.DisplayDebugMessage("Move to... " + place, ConsoleMessageTypes.INFO);

            if (DirectionConstants.CardinalDirections.Contains(place))
            {
                // If the place is a direction, handle it as such
                return TryMoveDirectionally(place);
            }
            else if (CurrentLocation.Name.Matches(place))
            {
                IOService.Output.WriteLine("You can't move there because you are already there.");
                return true;
            }
            else if (CurrentSublocation != null && CurrentSublocation.Name.Matches(place))
            {
                IOService.Output.WriteLine("You can't move there because you are already there.");
                return true;
            }
            else
            {
                // Try to move to a sublocation
                return TryMoveToSublocation(place);
            }
        }

        private bool TryMoveDirectionally(string direction)
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
                IOService.Output.DisplayFailMessage("You can't go that way from here.");
                return false;
            }
            else
            {
                if (CurrentLocation.Exits.TryGetValue(direction, out var newLocation))
                {
                    MoveTo(newLocation);
                    return true;
                }
                IOService.Output.DisplayFailMessage("You can't go that way.");
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

        public void EquipItem(Item item, string slot)
        {
            if (item.TryGetBehaviour<IEquippable>(out var equippableBehaviour) && equippableBehaviour.EquipInfo.IsEquippable)
            {
                IOService.Output.DisplayDebugMessage($"Attempting to equip {item} on player's {slot}.", ConsoleMessageTypes.INFO);
                if (!equippableBehaviour.EquipInfo.BodyParts.Contains(slot.ToLower()))
                {
                    throw new ArgumentException($"Item cannot be equipped in the {slot} slot. Valid slots are: {string.Join(", ", equippableBehaviour.EquipInfo.BodyParts)}");
                }
                else if (EquippedItems.TryGetValue(slot.ToLower(), out var _item) && _item != null)
                {
                    // If the slot is already occupied, unequip the current item
                    UnequipItem(EquippedItems[slot.ToLower()]!, slot);
                    IOService.Output.WriteLine($"You unequip the {_item.Name} from your {slot}.");

                    // Then equip the item in the specified slot
                    EquippedItems[slot.ToLower()] = item;
                    IOService.Output.WriteLine($"You equip the {item.Name} on your {slot}.");
                }
                else
                {
                    // Equip the item in the specified slot
                    EquippedItems[slot.ToLower()] = item;
                    IOService.Output.WriteLine($"You equip the {item.Name} on your {slot}.");
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

        public void Attack(NPC npc)
        {
            if (npc.TryGetBehaviour<ICanBeAttacked>(out var attackableBehaviour))
            {
                float damage = 0;
                var (baseStrength, bonusStrength, totalStrength) = Stats.GetStat(PlayerStatType.Strength);
                if (EquippedItems.TryGetValue("hand", out var weapon) && weapon != null && weapon.TryGetBehaviour<ICanDamage>(out var damageBehaviour))
                {
                    damage = (float)(damageBehaviour.BaseDamage + totalStrength * 0.5); // Example damage calculation
                    IOService.Output.WriteLine($"You attack {npc.Name} with {weapon.Name} for {damage} damage.");
                }
                else
                {
                    // If no weapon is equipped, use bare hands
                    damage = totalStrength;
                    IOService.Output.WriteLine($"You attack {npc.Name} with your bare hands for {damage} damage.");
                }
                attackableBehaviour.Attacked(damage);
                IOService.Output.WriteLine($"You attack {npc.Name} for {damage} damage. {npc.Name} now has {attackableBehaviour.Health} health left.");
            }
            else
            {
                IOService.Output.WriteLine($"{npc.Name} cannot be attacked.");
            }
        }

        /// <summary>
        /// Sets a game variable for the player.
        /// </summary>
        /// <param name="variableName">The name of the variable to set.</param>
        /// <param name="value">The value to set.</param>
        public void SetVariable(string variableName, int value)
        {
            switch (variableName.ToLower())
            {
                case "sight":
                    Visibility = value;
                    break;
                default:
                    IOService.Output.WriteLine($"Variable '{variableName}' is not recognized.");
                    break;
            }
        }

        public void ChangeHealth(int amount)
        {
            (int baseHealth, _, _) = Stats.GetStat(PlayerStatType.Health);
            (_, _, int totalMaxHealth) = Stats.GetStat(PlayerStatType.MaxHealth);
            Stats.SetBase(PlayerStatType.Health, Math.Clamp(baseHealth + amount, 0, totalMaxHealth));
            IOService.Output.WriteLine($"You have been healed by {amount} points.");
        }

        public void SetHealth(int amount)
        {
            (int baseHealth, _, _) = Stats.GetStat(PlayerStatType.Health);
            (_, _, int totalMaxHealth) = Stats.GetStat(PlayerStatType.MaxHealth);
            Stats.SetBase(PlayerStatType.Health, Math.Clamp(baseHealth, 0, totalMaxHealth));
            IOService.Output.WriteLine($"Your health has been set to {amount} points.");
        }
    }
}

