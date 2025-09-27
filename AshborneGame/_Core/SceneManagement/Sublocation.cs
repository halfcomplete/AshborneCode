using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using System.Text;

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
        public BOCSGameObject FocusObject { get; }

        public string ShortenedPositionalPhrase { get; init; }

        public string ShortRefDesc { get; init; }

        public DescriptionComposer DescriptionComposer { get; private set; }

        /// <summary>
        /// Dictionary of exits from this sublocation. Only 'back' is supported, which returns to the parent location.
        /// </summary>
        public Dictionary<string, Location> Exits { get; }

        /// <summary>
        /// Dictionary of custom commands for this sublocation.
        /// </summary>
        public Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; } = new();

        public int VisitCount { get; set; } = 0;

        /// <summary>
        /// Adds custom commands to this sublocation.
        /// </summary>
        public void AddCustomCommand(CustomCommandPhrasing phrasings, Func<string> messageFunc, Action effect)
        {
            foreach (var command in phrasings.Phrases)
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
        public LocationIdentifier Name { get; }

        /// <summary>
        /// Creates a new Sublocation.
        /// </summary>
        /// <param name="parent">Parent location.</param>
        /// <param name="focusObject">Object this sublocation focuses on.</param>
        /// <param name="name">LocationDescriptor for naming and parsing.</param>
        /// <param name="desc">Narrative profile for descriptions.</param>
        /// <param name="id">Unique identifier.</param>
        /// <param name="shortenedPositionalPhrase">From OutputConstants, where the sublocation is relative to the parent location.</param>
        /// <param name="shortRefDesc">Used when listing the sublocations in the parent location.</param>
        public Sublocation(Location parent, BOCSGameObject focusObject, LocationIdentifier name, DescriptionComposer desc, string id, string shortenedPositionalPhrase, string shortRefDesc)
        {
            ParentLocation = parent;
            FocusObject = focusObject;
            Name = name;
            DescriptionComposer = desc;
            ID = id;
            Exits = new Dictionary<string, Location> { { "back", parent } };
            ShortenedPositionalPhrase = shortenedPositionalPhrase;
            ShortRefDesc = shortRefDesc;
        }

        /// <summary>
        /// Returns the appropriate description for the player and state.
        /// </summary>
        public string GetDescription(Player player, GameStateManager state)
        {
            StringBuilder description = new StringBuilder();
            description.AppendLine(DescriptionComposer.GetDescription(player, state));
            description.AppendLine(GetExits());

            return description.ToString();
        }

        public string GetExits()
        {
            var sb = new StringBuilder();
            if (Exits.Count == 0)
            {
                sb.AppendLine("\nThere are no exits from here.");
            }
            else
            {
                sb.AppendLine("From here you can go:");
                foreach (var exit in Exits)
                {
                    if (DirectionConstants.CardinalDirections.Contains(exit.Key))
                    {
                        sb.AppendLine($"- {exit.Key} to {exit.Value.Name.DisplayName}");
                    }
                }
            }

            return sb.ToString();
        }
    }
}
