using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Interfaces;
using System.Collections.Immutable;

namespace AshborneGame._Core.LocationManagement
{
    /// <summary>
    /// Registry for managing and retrieving locations by their unique identifiers.
    /// </summary>
    public class LocationRegistry : ILocationRegistry
    {
        private readonly Dictionary<DefinitionID, Location> _byDefinitionID = [];
        private readonly Dictionary<InstanceID, Location> _byInstanceID = [];

        public void RegisterLocation(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (_byDefinitionID.ContainsKey(location.DefinitionID))
                throw new InvalidOperationException($"Location with ID '{location.DefinitionID}' is already registered. The Location's name is '{location.Name.ReferenceName}'.");

            _byDefinitionID[location.DefinitionID] = location;
        }

        public bool TryGetLocationByDefinitionID(DefinitionID id, out Location? location)
        {
            if (string.IsNullOrWhiteSpace(id.Value.ToString()))
                throw new ArgumentException("ID cannot be null or whitespace.", nameof(id));

            return _byDefinitionID.TryGetValue(id, out location);
        }

        public bool TryGetLocationByInstanceID(InstanceID id, out Location? location)
        {
            if (string.IsNullOrWhiteSpace(id.Value.ToString()))
                throw new ArgumentException("ID cannot be null or whitespace.", nameof(id));

            return _byInstanceID.TryGetValue(id, out location);
        }

        /// <summary>
        /// Returns all registered location IDs. Useful for error messages and debugging.
        /// </summary>
        public IReadOnlyList<DefinitionID> GetLocationIDs()
        {
            return _byDefinitionID.Keys.ToImmutableList();
        }

        public IReadOnlyList<Location> GetLocations()
        {
            return _byDefinitionID.Values.ToImmutableList();
        }
    }
}