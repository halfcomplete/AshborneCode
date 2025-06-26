using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

namespace AshborneGame._Core._Player
{
    /// <summary>
    /// Represents a player in the game.
    /// </summary>
    public class Player
    {
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
        private static readonly string[] _directions = { "north", "south", "east", "west" };

        public Player()
        {
            _name = "Hero"; // Default name
            CurrentLocation = new Location("test location", "A test location for testing.");
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
            Inventory = new Inventory();
        }

        public Player(string name)
        {
            _name = name;
            CurrentLocation = new Location("Placeholder", "Placeholder", 5);
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
            startingLocation.TimesVisited = 1;
            Inventory = new Inventory();
        }

        /// <summary>
        /// Moves the player to a sublocation.
        /// </summary>
        /// <param name="newLocation">The sublocation to move to.</param>
        /// <exception cref="ArgumentNullException">Thrown when newLocation is null.</exception>
        public void MoveTo(Sublocation newLocation)
        {
            CurrentSublocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            newLocation.TimesVisited += 1;
            IOService.Output.WriteLine(CurrentSublocation.GetFullDescription(this));
        }

        /// <summary>
        /// Moves the player to a location.
        /// </summary>
        /// <param name="newLocation">The location to move to.</param>
        /// <exception cref="ArgumentNullException">Thrown when newLocation is null.</exception>
        public void MoveTo(Location newLocation)
        {
            CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            CurrentSublocation = null;
            newLocation.TimesVisited += 1;
            IOService.Output.WriteLine(CurrentLocation.GetFullDescription(this));
        }

        public void SetupMoveTo(Location newLocation)
        {
            CurrentLocation = newLocation ?? throw new ArgumentNullException(nameof(newLocation));
            CurrentSublocation = null;
            newLocation.TimesVisited = 1;
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

            if (_directions.Contains(place))
            {
                // If the place is a direction, handle it as such
                return TryMoveDirectionally(place);
            }
            else if (CurrentLocation.Exits.Values.Any(s => s.Name.Equals(place, StringComparison.OrdinalIgnoreCase)))
            {
                // If the place is the name of an exit then move to that exit
                MoveTo(CurrentLocation.Exits.Values.First(s => s.Name.Equals(place, StringComparison.OrdinalIgnoreCase)));
                return true;
            }
            else if (parsedInput[0].Equals("back", StringComparison.OrdinalIgnoreCase))
            {
                MoveTo(CurrentLocation);
                return true;
            }
            else if (parsedInput[0].Equals("through", StringComparison.OrdinalIgnoreCase) && GameContext.Player.CurrentSublocation != null && GameContext.Player.CurrentSublocation.Object.GetAllBehaviours<IInteractable>().Any(b => b is OpenCloseBehaviour openClose && openClose.IsOpen))
            {
                // If the player inputted "go through" and we are in a sublocation, the object of which is openable
                if (GameContext.Player.CurrentSublocation.Object.TryGetBehaviour<IExit>(out var exit))
                {
                    // If that object is a pathway to another location
                    MoveTo(exit.Location); 
                    return true;
                }
                return false;
            }
            else if (place == CurrentLocation.Name || place == (CurrentSublocation != null ? CurrentSublocation.Name : string.Empty))
            {
                IOService.Output.WriteLine("You can't move there because you are already there.");
                return true;
            }
            else
            {
                // Else the place is a sublocation
                return TryMoveToSublocation(place);
            }
        }

        private bool TryMoveDirectionally(string direction)
        {
            if (CurrentLocation.Exits.TryGetValue(direction, out Location? newLocation))
            {
                MoveTo(newLocation);
                return true;
            }
            else
            {
                return false;
            }
        }

        private bool TryMoveToSublocation(string place)
        {
            // Get the sublocation from sublocation list in the current location by name
            var sublocation = CurrentLocation.Sublocations
                .FirstOrDefault(s => s.Name.Equals(place, StringComparison.OrdinalIgnoreCase));

            // Because .FirstOrDefault can return null, check if sublocation is not null
            // If it is not null, move to that sublocation (that means it exists in the current location)
            // If it is null, throw an exception
            if (sublocation != null)
            {
                MoveTo(sublocation);
                return true;
            }
            else
            {
                return false;
            }
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
                var (baseStrength, bonusStrength, totalStrength) = Stats.GetStat(PlayerStatTypes.Strength);
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
            (int baseHealth, _, _) = Stats.GetStat(PlayerStatTypes.Health);
            (_, _, int totalMaxHealth) = Stats.GetStat(PlayerStatTypes.MaxHealth);
            Stats.SetBase(PlayerStatTypes.Health, Math.Clamp(baseHealth + amount, 0, totalMaxHealth));
            IOService.Output.WriteLine($"You have been healed by {amount} points.");
        }

        public void SetHealth(int amount)
        {
            (int baseHealth, _, _) = Stats.GetStat(PlayerStatTypes.Health);
            (_, _, int totalMaxHealth) = Stats.GetStat(PlayerStatTypes.MaxHealth);
            Stats.SetBase(PlayerStatTypes.Health, Math.Clamp(baseHealth, 0, totalMaxHealth));
            IOService.Output.WriteLine($"Your health has been set to {amount} points.");
        }
    }
}
