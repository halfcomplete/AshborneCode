using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Represents a sublocation within a location, focused on a specific object.
    /// </summary>
    public class Sublocation : ILocation
    {
        /// <summary>
        /// The parent location containing this sublocation.
        /// </summary>
        public Location ParentLocation { get; }

        /// <summary>
        /// The object that is at this sublocation.
        /// </summary>
        public BOCSGameObject GameObject { get; }

        /// <summary>
        /// Dictionary of exits from this sublocation. Only 'back' is supported, which returns to the parent location.
        /// </summary>
        public Dictionary<string, Location> Exits { get; }

        /// <summary>
        /// Dictionary of custom commands for this sublocation.
        /// </summary>
        public Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; } = new();

        /// <summary>
        /// Adds custom commands to this sublocation.
        /// </summary>
        public void AddCustomCommand(List<string> commands, Func<string> messageFunc, Action effect)
        {
            foreach (var command in commands)
            {
                CustomCommands[command] = (messageFunc, effect);
            }
        }

        /// <summary>
        /// Removes a custom command by its string key.
        /// </summary>
        public void RemoveCustomCommand(string command)
        {
            if (CustomCommands.ContainsKey(command))
                CustomCommands.Remove(command);
        }

        /// <summary>
        /// Unique identifier for the sublocation.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Flexible naming and parsing for the sublocation.
        /// </summary>
        public LocationDescriptor Name { get; }

        /// <summary>
        /// Narrative profile for all types of descriptions.
        /// </summary>
        public LocationNarrativeProfile Descriptions { get; }

        /// <summary>
        /// Creates a new Sublocation.
        /// </summary>
        /// <param name="parent">Parent location.</param>
        /// <param name="focusObject">Object this sublocation focuses on.</param>
        /// <param name="name">LocationDescriptor for naming and parsing.</param>
        /// <param name="desc">Narrative profile for descriptions.</param>
        /// <param name="id">Unique identifier.</param>
        public Sublocation(Location parent, GameObject focusObject, LocationDescriptor name, LocationNarrativeProfile desc, string id)
        {
            ParentLocation = parent;
            GameObject = focusObject;
            Name = name;
            Descriptions = desc;
            ID = id;
            Exits = new Dictionary<string, Location> { { "back", parent } };
        }

        /// <summary>
        /// Returns the appropriate description for the player and state.
        /// </summary>
        public string GetDescription(Player player, GameStateManager state) => Descriptions.GetDescription(player, state);

        /// <summary>
        /// Returns the positional name (e.g., "in the Dusty Armoury").
        /// </summary>
        public string GetPositionalName() => $"{Name.PositionalPrefix} {Name.DisplayName}";
    }
}
