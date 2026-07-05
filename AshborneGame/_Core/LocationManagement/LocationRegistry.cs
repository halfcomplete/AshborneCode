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
        private readonly Dictionary<InstanceID, DefinitionID> _byInstanceID = [];

        private readonly Dictionary<DefinitionID, Scene> _scenes = [];

        public void RegisterLocation(Location location)
        {
            ArgumentNullException.ThrowIfNull(location);

            if (_byDefinitionID.ContainsKey(location.DefinitionID))
            {
                throw new InvalidOperationException($"Location with DefinitionID '{location.DefinitionID}' is already registered. The Location's name is '{location.Name.ReferenceName}'.");
            }

            _byDefinitionID[location.DefinitionID] = location;
            _byInstanceID[location.InstanceID] = location.DefinitionID;
        }

        public void RegisterScene(Scene scene)
        {
            ArgumentNullException.ThrowIfNull(scene);

            if (_scenes.ContainsKey(scene.DefinitionID))
            {
                throw new InvalidOperationException($"Scene with DefinitionID '{scene.DefinitionID}' is already registered. The Scene's name is '{scene.DisplayName}'.");
            }

            _scenes[scene.DefinitionID] = scene;
        }

        public bool TryGetLocationByDefinitionID(DefinitionID id, out Location? location)
        {
            if (string.IsNullOrWhiteSpace(id.Value.ToString()))
            {
                throw new ArgumentException("DefinitionID cannot be null or whitespace.", nameof(id));
            }

            return _byDefinitionID.TryGetValue(id, out location);
        }

        public bool TryGetLocationByInstanceID(InstanceID id, out Location? location)
        {
            if (string.IsNullOrWhiteSpace(id.Value.ToString()))
            {
                throw new ArgumentException("InstanceID cannot be null or whitespace.", nameof(id));
            }

            if (!_byInstanceID.TryGetValue(id, out var locationID))
            {
                // because this is a TryGet, return false
                location = null;
                return false;
            }

            if (!_byDefinitionID.TryGetValue(locationID, out location))
            {
                // This shouldn't happen unless something has fundamentally gone wrong with the location registration
                // Therefore throw an error
                // TODO: maybe not throw an error? idk
                throw new ArgumentException("DefinitionID not mapped to a Location.", nameof(locationID));
            }

            return true;
        }

        public bool TryGetSceneByDefinitionID(DefinitionID id, out Scene? scene)
        {
            if (string.IsNullOrWhiteSpace(id.Value.ToString()))
            {
                throw new ArgumentException("DefinitionID cannot be null or whitespace.", nameof(id));
            }

            return _scenes.TryGetValue(id, out scene);
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

        public IReadOnlyList<DefinitionID> GetSceneIDs()
        {
            return _scenes.Keys.ToImmutableList();
        }

        public IReadOnlyList<Scene> GetScenes()
        {
            return _scenes.Values.ToImmutableList();
        }
    }
}