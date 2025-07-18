using System.Collections.Generic;
using System.Text;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
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
        public Scene Scene { get; set; }

        /// <summary>
        /// Flexible naming and parsing for the location.
        /// </summary>
        public LocationIdentifier Name { get; }

        /// <summary>
        /// Composer class for managing descriptions.
        /// </summary>
        public DescriptionComposer DescriptionComposer { get; private set; }

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

        public int VisitCount { get; private set; } = 0;

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
        /// <param name="composer">DescriptionComposer to combine descriptions.</param>
        /// <param name="id">Unique identifier.</param>
        public Location(LocationIdentifier name, DescriptionComposer composer, string id)
        {
            Name = name;
            DescriptionComposer = composer;
            ID = id;
        }

        public Location(LocationIdentifier name, string id)
            : this(name, new DescriptionComposer(), id)
        {
        }

        public Location()
        {
            Scene = new Scene("default_scene", "Default Scene");
            Name = new LocationIdentifier("Default Location");
            DescriptionComposer = new DescriptionComposer(
                "You see a generic location.",
                new FadingDescription("You enter a new place.", "You are here again.", "You have been here many times."),
                new SensoryDescription("A generic location.", "You hear ambient sounds."));
            ID = "default_location";
        }

        /// <summary>
        /// Returns the appropriate description for the player and state.
        /// </summary>
        public string GetDescription(Player player, GameStateManager state) => DescriptionComposer.GetDescription(player, state);

        public void SetDescriptionComposer(DescriptionComposer composer)
        {
            DescriptionComposer = composer ?? throw new ArgumentNullException(nameof(composer), "Description composer cannot be null.");
        }

        public string GetExits()
        {
            var sb = new StringBuilder();
            if (Exits.Count == 0)
            {
                sb.AppendLine("\nThere are no exits from here.");
                if (Sublocations.Count == 0)
                {
                    sb.Append(" There is also nothing of note here.");
                }
                else
                {
                    sb.Append(" However, you can go to:");
                }
            }
            else
            {
                sb.AppendLine("\nFrom here you can leave:");
                foreach (var exit in Exits)
                {
                    if (DirectionConstants.CardinalDirections.Contains(exit.Key))
                    {
                        sb.AppendLine($"- {exit.Key} to {exit.Value.Name}");
                    }
                }
            }

            foreach (var sublocation in Sublocations)
            {
                sb.AppendLine($"- {sublocation.Name}");
            }

            foreach (var @object in GameObjects)
            {
                sb.AppendLine($"- {@object.Name}");
            }

            return sb.ToString();
        }

        public void IncrementVisitCount()
        {
            VisitCount++;
        }
    }
}
