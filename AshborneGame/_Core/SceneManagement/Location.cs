using System.Collections.Generic;
using System.Text;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Represents a location in the game world, supporting rich environmental storytelling and dynamic scene control.
    /// </summary>
    public class Location : ILocation
    {
        /// <summary>
        /// Unique identifier for the location. Uses a slug-based format derived from the location name.
        /// Format: "Locations.{normalized-name}" (e.g., "Locations.eye-platform")
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// The group this location belongs to, if any.
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// Flexible naming and parsing for the location.
        /// </summary>
        public LocationNameAdapter Name { get; }

        /// <summary>
        /// Composer class for managing descriptions.
        /// </summary>
        public DescriptionComposer DescriptionComposer { get; private set; }

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

        public int VisitCount { get; set; } = 0;

        /// <summary>
        /// Creates a new Location with a given name, DescriptionComposer, and explicit ID.
        /// </summary>
        /// <param name="name">LocationDescriptor for naming and parsing.</param>
        /// <param name="composer">DescriptionComposer to combine descriptions.</param>
        /// <param name="id">Explicit ID for the location. If null, auto-generates from name.</param>
        public Location(LocationNameAdapter name, DescriptionComposer composer, string? id = null)
        {
            Name = name;
            DescriptionComposer = composer;
            ID = id ?? SlugIdService.GenerateSlugId(name.ReferenceName, "location");
        }

        /// <summary>
        /// Creates a new Location with a given name and a placeholder DescriptionComposer.
        /// </summary>
        /// <param name="name">LocationDescriptor for naming and parsing.</param>
        /// <param name="id">Optional explicit ID. If null, auto-generates from name.</param>
        public Location(LocationNameAdapter name, string? id = null)
            : this(name, new DescriptionComposer(), id)
        {
        }

        public Location()
        {
            Scene = new Scene("default_scene", "Default Scene");
            Name = new LocationNameAdapter("Default Location");
            ID = SlugIdService.GenerateSlugId(Name.ReferenceName, "location");
            DescriptionComposer = new DescriptionComposer(
                new LookDescription(),
                new VisitDescription("You enter a new place.", "You are here again.", "You have been here many times."),
                new SensoryDescription("A generic location.", "You hear ambient sounds."));
        }

        /// <summary>
        /// Adds custom commands to this location.
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
        /// Adds a sublocation to this location.
        /// </summary>
        public void AddSublocation(Sublocation sublocation)
        {
            if (!Sublocations.Contains(sublocation))
                Sublocations.Add(sublocation);
        }

        /// <summary>
        /// Removes a sublocation from this location. If the player is in that sublocation, they are moved back to the parent location.
        /// </summary>
        public void RemoveSublocation(Sublocation sublocation)
        {
            if (Sublocations.Contains(sublocation))
                Sublocations.Remove(sublocation);
            if (GameContext.Player.CurrentSublocation == sublocation)
            {
                GameContext.Player.ForceMoveTo(this);
            }
        }

        /// <summary>
        /// Returns the appropriate description for the player and state.
        /// </summary>
        public string GetDescription(Player player, GameStateManager state)
        {
            StringBuilder description = new StringBuilder();

            try
            {
                description.AppendLine(DescriptionComposer.GetDescription(player, state));
            }
            catch (InvalidOperationException e)
            {
                // FAST FAIL FOR NOW
                throw new InvalidOperationException($"Location.cs: DescriptionComposer for location '{Name.ReferenceName}' is not properly set up. {e.Message}");
            }

            description.AppendLine(GetExits());

            return description.ToString();
        }

        public string GetLookDescription(Player player, GameStateManager state) => DescriptionComposer.GetLookDescription(player, state);

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
                sb.AppendLine("From here you can go:");
                foreach (var exit in Exits)
                {
                    if (DirectionConstants.CardinalDirections.Contains(exit.Key))
                    {
                        sb.AppendLine($"- {exit.Key} to {exit.Value.Name.DisplayName}");
                    }
                }

                if (Sublocations.Count > 0)
                {
                    sb.AppendLine("\n You can also go to:");
                }
            }

            foreach (var sublocation in Sublocations)
            {
                sb.AppendLine($"- {sublocation.Name.DisplayName}");
            }

            return sb.ToString();
        }
    }
}
