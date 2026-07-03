using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.LocationManagement
{
    /// <summary>
    /// Registry for managing and retrieving locations by their unique identifiers.
    /// </summary>
    public static class LocationRegistry
    {
        private static readonly Dictionary<DefinitionID, ILocation> _locationsByID = [];

        public static void RegisterLocation(ILocation location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (_locationsByID.ContainsKey(location.DefinitionID))
                throw new InvalidOperationException($"Location with ID '{location.DefinitionID}' is already registered. The Location's name is '{location.Name.ReferenceName}'.");

            _locationsByID[location.DefinitionID] = location;
        }

        public static bool GetLocationByID(DefinitionID id, out ILocation? location)
        {
            if (string.IsNullOrWhiteSpace(id.Value.ToString()))
                throw new ArgumentException("ID cannot be null or whitespace.", nameof(id));

            return _locationsByID.TryGetValue(id, out location);
        }

        /// <summary>
        /// Returns all registered location IDs. Useful for error messages and debugging.
        /// </summary>
        public static IEnumerable<DefinitionID> GetAllLocationIds()
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