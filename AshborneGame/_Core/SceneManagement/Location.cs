using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;
using System.Numerics;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Represents a location in the game world.
    /// </summary>
    public class Location : IDescribable
    {
        /// <summary>
        /// Gets the unique identifier of the location.
        /// </summary>
        public string Id { get; }

        /// <summary>
        /// Gets the name of the location.
        /// </summary>
        public virtual string Name { get; protected set; }

        /// <summary>
        /// Gets the description of the location.
        /// </summary>
        public virtual string Description { get; protected set; }

        /// <summary>
        /// Gets the dictionary of exits from this location.
        /// </summary>
        public IReadOnlyDictionary<string, Location> Exits => _exits;

        /// <summary>
        /// Gets the list of sublocations in this location.
        /// </summary>
        public IReadOnlyList<Sublocation> Sublocations => _sublocations;

        private readonly Dictionary<string, Location> _exits;
        private readonly List<Sublocation> _sublocations;
        private readonly int _minimumVisibility;

        /// <summary>
        /// Initializes a new instance of the Scene class.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        /// <param name="description">The description of the location.</param>
        /// <exception cref="ArgumentNullException">Thrown when name or description is null.</exception>
        public Location(string name, string description)
        {
            Id = Guid.NewGuid().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            _exits = new Dictionary<string, Location>();
            _sublocations = new List<Sublocation>();
            _minimumVisibility = 5; // Default visibility level
        }

        /// <summary>
        /// Initializes a new instance of the Scene class with a custom minimum visibility level.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        /// <param name="description">The description of the location.</param>
        /// <param name="minimumVisibility">The minimum visibility level required to see this location.</param>
        public Location(string name, string description, int minimumVisibility)
            : this(name, description)
        {
            _minimumVisibility = minimumVisibility;
        }

        /// <summary>
        /// Adds an exit to another location.
        /// </summary>
        /// <param name="direction">The direction of the exit.</param>
        /// <param name="location">The location that can be reached through this exit.</param>
        /// <exception cref="ArgumentNullException">Thrown when direction or location is null.</exception>
        public void AddExit(string direction, Location location)
        {
            if (string.IsNullOrEmpty(direction))
            {
                throw new ArgumentNullException(nameof(direction));
            }

            if (location == null)
            {
                throw new ArgumentNullException(nameof(location));
            }

            if (!_exits.ContainsKey(direction))
            {
                _exits[direction] = location;
            }
        }

        /// <summary>
        /// Adds a sublocation to this location.
        /// </summary>
        /// <param name="sublocation">The sublocation to add.</param>
        /// <exception cref="ArgumentNullException">Thrown when sublocation is null or already exists.</exception>
        public void AddSublocation(Sublocation sublocation)
        {
            if (sublocation == null)
            {
                throw new ArgumentNullException(nameof(sublocation));
            }

            if (_sublocations.Contains(sublocation))
            {
                throw new ArgumentException("Subscene already exists in this location.", nameof(sublocation));
            }

            _sublocations.Add(sublocation);
        }

        public string GetExits(Player player)
        {
            string exitString = string.Empty;
            bool _areThereHiddenExits = false;
            if (_exits.Count == 0)
            {
                return exitString;
            }
            exitString += "You can go:\n";
            foreach (var _exit in _exits)
            {
                if (_exit.Value.CanPlayerSeeExit(player))
                {
                    exitString += $"- {_exit.Key} to {_exit.Value.Name}\n";
                }
                else
                {
                    _areThereHiddenExits = true;
                }
            }

            if (_areThereHiddenExits)
            {
                exitString += "It's awfully dark though. You could be missing something.\n";
            }

            return exitString;
        }

        public string GetSublocations(Player player)
        {
            string sublocationString = string.Empty;

            if (_sublocations.Count == 0)
            {
                return "";
            }

            bool areAllSublocationsHidden = _sublocations.All(s => !s.CanPlayerSeeSublocation(player));
            if (areAllSublocationsHidden)
            {
                sublocationString += "You can't see much. If only it was brighter.\n";
                return "";
            }

            bool areAnySublocationsHidden = false;
            sublocationString += "You notice some other things nearby:\n";
            foreach (var sublocation in _sublocations)
            {
                if (sublocation.CanPlayerSeeSublocation(player))
                {
                    sublocationString += $"- a {sublocation.Name}\n";
                }
                else
                {
                    areAnySublocationsHidden = true;
                }
            }

            if (areAnySublocationsHidden)
            {
                sublocationString += "It's awfully dark though. You could be missing something.";
            }

            return sublocationString;
        }

        /// <summary>
        /// Determines if the player can see this location as an exit.
        /// </summary>
        /// <returns>True if the player can see this location; otherwise, false.</returns>
        public bool CanPlayerSeeExit(Player player)
        {
            return player.Visibility >= _minimumVisibility;
        }

        public string GetDescription(Player player)
        {
            string contextualDescription = Description;
            if (player.EquippedItems.Any(s => s.Value != null && s.Value.Name.Equals("torch", StringComparison.OrdinalIgnoreCase)))
            {
                contextualDescription += $". It is barely lit by your torch.";
            }
            return $"You are at {Name}. {contextualDescription}";
        }

        public string GetFullDescription(Player player)
        {
            string description = GetDescription(player);
            if (_exits.Count > 0)
            {
                description += "\n" + GetExits(player);
            }
            if (_sublocations.Count > 0)
            {
                description += "\n" + GetSublocations(player);
            }
            return description;
        }
    }
}
