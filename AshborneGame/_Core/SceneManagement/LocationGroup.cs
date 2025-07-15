using System.Collections.Generic;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Groups Locations into a shared region/scene (e.g., "Tower of Awakening").
    /// Used for organization, mask/system logic, saving state, and ambient region tracking.
    /// </summary>
    public class LocationGroup
    {
        /// <summary>
        /// Unique identifier for the group.
        /// </summary>
        public string ID { get; }

        /// <summary>
        /// Display name for the group (e.g., "Tower of Awakening").
        /// </summary>
        public string DisplayName { get; }

        /// <summary>
        /// List of locations in this group.
        /// </summary>
        public List<Location> Locations { get; } = new();

        /// <summary>
        /// Creates a new LocationGroup.
        /// </summary>
        /// <param name="id">Unique identifier.</param>
        /// <param name="displayName">Display name.</param>
        public LocationGroup(string id, string displayName)
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
    }
}
