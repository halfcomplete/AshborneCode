using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Globals.Interfaces
{
    /// <summary>
    /// Shared interface for Location and Sublocation.
    /// </summary>
    public interface ILocation
    {
        /// <summary>
        /// Unique identifier for the location.
        /// </summary>
        string ID { get; }

        /// <summary>
        /// Flexible naming and parsing for the location.
        /// </summary>
        LocationNameAdapter Name { get; }

        /// <summary>
        /// Narrative profile for all types of descriptions.
        /// </summary>
        DescriptionComposer DescriptionComposer { get; }

        /// <summary>
        /// Dictionary of exits from this location. Keys are directions/keywords, values are Locations.
        /// </summary>
        Dictionary<string, Location> Exits { get; }

        /// <summary>
        /// Dictionary of custom commands for this location.
        /// </summary>
        Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; }

        int VisitCount { get; set; }

        /// <summary>
        /// Returns the appropriate description for the player and state.
        /// </summary>
        string GetDescription(Player player, GameStateManager state);

        /// <summary>
        /// Adds an exit to another location.
        /// </summary>
        public void AddExit(string direction, Location location)
        {
            if (!Exits.ContainsKey(direction))
                Exits[direction] = location;
        }

        /// <summary>
        /// Removes an exit by direction.
        /// </summary>
        public void RemoveExit(string direction)
        {
            if (Exits.ContainsKey(direction))
                Exits.Remove(direction);
        }

        /// <summary>
        /// Adds custom commands to this location.
        /// </summary>
        void AddCustomCommand(CustomCommandPhrasing phrasings, Func<string> messageFunc, Action effect);

        /// <summary>
        /// Removes a custom command by its string key.
        /// </summary>
        void RemoveCustomCommand(string command);

        public void IncrementVisitCount()
        {
            VisitCount++;
        }

        public string GetExits();
    }
}
