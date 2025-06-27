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
        /// Gets the name of the location (i.e. throne room). Used to say $"You are {PositionalPrefix} {Name}."
        /// </summary>
        public virtual string Name { get; private set; }

        public virtual string ReferenceName { get; private set; }

        /// <summary>
        /// The base description of the location (i.e.
        /// </summary>
        public virtual string Description { get; private set; }

        /// <summary>
        /// Additional description, used when looking around or if it's the first time the player travels to that location (i.e. "It's an endless ocean of black water frozen 
        /// </summary>
        public virtual string AdditionalDescription { get; private set; }

        /// <summary>
        /// Where the player is when they travel to that location (i.e. "at the", "in front of the"). Used to say $"You are {PositionalPrefix} {Name}."
        /// </summary>
        public virtual string PositionalPrefix { get; private set; }

        public virtual Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; } = new();


        /// <summary>
        /// Gets the dictionary of exits from this location.
        /// </summary>
        public IReadOnlyDictionary<string, Location> Exits => _exits;

        /// <summary>
        /// Gets the list of sublocations in this location.
        /// </summary>
        public IReadOnlyList<Sublocation> Sublocations => _sublocations;

        public List<(Func<GameStateManager, bool> condition, string description)> Conditions { get; } = new();

        public int TimesVisited { get; set; } = 0;

        private readonly Dictionary<string, Location> _exits;
        private readonly List<Sublocation> _sublocations;
        private readonly int _minimumVisibility;


        public Location(string name, string description)
        {
            Id = Guid.NewGuid().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            PositionalPrefix = "test prefix";
            ReferenceName = name;
            _exits = new Dictionary<string, Location>();
            _sublocations = new List<Sublocation>();
            _minimumVisibility = 5; // Default visibility level
        }

        /// <summary>
        /// Initializes a new instance of the Scene class.
        /// </summary>
        /// <param name="name">The name of the location.</param>
        /// <param name="description">The description of the location.</param>
        /// <exception cref="ArgumentNullException">Thrown when name or description is null.</exception>
        public Location(string name, string description, string positionalPrefix, string referenceName, string additionalDescription = "something goes here")
        {
            Id = Guid.NewGuid().ToString();
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            PositionalPrefix = positionalPrefix;
            ReferenceName = referenceName;
            AdditionalDescription = additionalDescription;
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
            sublocationString += "You notice some other things nearby:";
            foreach (var sublocation in _sublocations)
            {
                if (sublocation.CanPlayerSeeSublocation(player))
                {
                    sublocationString += $"\n- a {sublocation.Name}";
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

        public void AddCustomCommand(List<string> commands, Func<string> messageFunc, Action effect)
        {
            foreach (var command in commands)
            {
                CustomCommands[command] = (messageFunc, effect);
            }
        }


        /// <summary>
        /// Determines if the player can see this location as an exit.
        /// </summary>
        /// <returns>True if the player can see this location; otherwise, false.</returns>
        public bool CanPlayerSeeExit(Player player)
        {
            return player.Visibility >= _minimumVisibility;
        }

        public void AddCondition(Func<GameStateManager, bool> condition, string description)
        {
            Conditions.Add((condition, description));
        }

        public virtual string GetDescription(Player player, GameStateManager state)
        {
            string description;

            if (TimesVisited > 1)
            {
                description = $"You are back, {PositionalPrefix} {ReferenceName}. ";
            }
            else
            {
                description = $"You are {PositionalPrefix} {ReferenceName}. ";
            }

            foreach (var (condition, desc) in Conditions)
            {
                if (condition(state))
                    return description + desc;
            }

            return description + Description;
        }

        public string GetFullDescription(Player player)
        {
            string description = GetDescription(player, GameContext.GameState);
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
