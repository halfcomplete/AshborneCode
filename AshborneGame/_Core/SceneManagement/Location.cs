using System.Collections.Generic;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Represents a location in the game world, supporting rich environmental storytelling and dynamic scene control.
    /// </summary>
    public class Location : ILocation
    {
        /// <summary>
        /// Unique identifier for the location.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// The group this location belongs to, if any.
        /// </summary>
        public LocationGroup? Group { get; set; }

        /// <summary>
        /// Flexible naming and parsing for the location.
        /// </summary>
        public LocationDescriptor Name { get; }

        /// <summary>
        /// Narrative profile for all types of descriptions.
        /// </summary>
        public LocationNarrativeProfile Descriptions { get; }

        /// <summary>
        /// Game objects present in this location.
        /// </summary>
        public List<GameObject> GameObjects { get; } = new();

        /// <summary>
        /// Sublocations within this location.
        /// </summary>
        public List<Sublocation> Sublocations { get; } = new();

        /// <summary>
        /// Dictionary of exits from this location. Keys are directions/keywords, values are Locations.
        /// </summary>
        public Dictionary<string, Location> Exits { get; } = new();

        /// <summary>
        /// Dictionary of custom commands for this location.
        /// </summary>
        public Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; } = new();

        /// <summary>
        /// Adds custom commands to this location.
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
        /// Adds a sublocation to this location.
        /// </summary>
        public void AddSublocation(Sublocation sublocation)
        {
            if (!Sublocations.Contains(sublocation))
                Sublocations.Add(sublocation);
        }

        /// <summary>
        /// Creates a new Location.
        /// </summary>
        /// <param name="name">LocationDescriptor for naming and parsing.</param>
        /// <param name="descriptions">Narrative profile for descriptions.</param>
        /// <param name="id">Unique identifier.</param>
        public Location(LocationDescriptor name, LocationNarrativeProfile descriptions, string id)
        {
            Name = name;
            Descriptions = descriptions;
            ID = id;
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
