using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.SceneManagement
{
    /// <summary>
    /// Registry for managing and retrieving locations by their unique identifiers.
    /// </summary>
    public static class LocationRegistry
    {
        private static readonly Dictionary<string, ILocation> _locationsByID = [];

        public static void RegisterLocation(ILocation location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (_locationsByID.ContainsKey(location.ID))
                throw new InvalidOperationException($"Location with ID '{location.ID}' is already registered. The Location's name is '{location.Name.ReferenceName}'.");

            _locationsByID[location.ID] = location;
        }

        public static bool GetLocationByID(string id, out ILocation? location)
        {
            if (string.IsNullOrWhiteSpace(id))
                throw new ArgumentException("ID cannot be null or whitespace.", nameof(id));

            return _locationsByID.TryGetValue(id, out location);
        }

        /// <summary>
        /// Returns all registered location IDs. Useful for error messages and debugging.
        /// </summary>
        public static IEnumerable<string> GetAllLocationIds()
        {
            return _locationsByID.Keys;
        }

        /// <summary>
        /// Clears all registered locations. Used for test isolation between tests.
        /// </summary>
        public static void Clear()
        {
            _locationsByID.Clear();
        }
    }
}