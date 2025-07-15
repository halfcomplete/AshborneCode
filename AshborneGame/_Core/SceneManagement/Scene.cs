using AshborneGame._Core.Game;
using System.Collections.Generic;
using System.Text;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Groups Locations into a shared scene (e.g., "Tower of Awakening").
    /// Used for organization, mask/system logic, saving state, and ambient region tracking.
    /// </summary>
    public class Scene
    {
        /// <summary>
        /// Unique identifier for the scene.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Display name for the scene (e.g., "Tower of Awakening").
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// List of locations in this scene.
        /// </summary>
        public List<Location> Locations { get; } = new();

        /// <summary>
        /// Creates a new Scene.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="displayName">Display name.</param>
        /// <param name="act">The act number of the scene.</param>
        public Scene(string id, string displayName)
        {
            ID = id;
            DisplayName = displayName;
        }

        /// <summary>
        /// Adds a location to this group and sets its Group property.
        /// </summary>
        /// <param name="location">The location to add.</param>
        public void AddLocation(Location location)
        {
            Locations.Add(location);
            location.Group = this;
        }

        public string GetHeader()
        {
            StringBuilder header = new StringBuilder();
            header.AppendLine("ACT 1"); // Placeholder
            if (!GameContext.GameState.TryGetCounter("player.current_scene_no", out int sceneNo))
            {
                throw new Exception("Get counter 'player.current_scene_no' in Scene.cs failed. Counter does not exist.");
            }
            header.AppendLine($"SCENE {sceneNo}");
            header.AppendLine($"--------");
            header.AppendLine(DisplayName);
            return header.ToString();
        }
    }
}
