using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions.Registries;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Serialisation
{
    public sealed class SaveLoadContext
    {
        private IInstanceRegistry _instanceRegistry;
        private Dictionary<InstanceID, List<BehaviourSaveData>> _instanceToBehaviourMap;
        public ILocationRegistry LocationRegistry { get; }

        public SaveLoadContext(IInstanceRegistry instanceRegistry, ILocationRegistry locationRegistry)
        {
            _instanceRegistry = instanceRegistry;
            LocationRegistry = locationRegistry;
        }

        // Save: object -> ID
        public InstanceID ToId(BOCSObject obj) => obj.InstanceID;
        public DefinitionID ToId(Location loc) => loc.DefinitionID;

        // Load phase 2 only: ID -> object
        public BOCSObject ResolveObject(InstanceID id)
        {
            if (!_instanceRegistry.TryGet(id, out var obj))
            {
                throw new InvalidOperationException($"[SaveLoadContext]: Failed to resolve object with InstanceID {id}.");
            }

            return obj;
        }

        public Location ResolveLocation(DefinitionID id)
        {
            if (!LocationRegistry.TryGetLocationByDefinitionID(id, out var loc))
            {
                throw new InvalidOperationException($"[SaveLoadContext]: Failed to resolve location with DefinitionID {id}.");
            }

            return loc;
        }

        public T? TryResolveObject<T>(InstanceID? id) where T : class
        {
            if (id == null)
            {
                return null;
            }

            if (!_instanceRegistry.TryGet(id.Value, out var obj))
            {
                return null;
            }

            return obj as T;
        }

        public void Register(BOCSObject bocsObject, List<BehaviourSaveData> behaviours)
        {
            _instanceRegistry.Register(bocsObject);
            _instanceToBehaviourMap.Add(bocsObject.InstanceID, behaviours);
        }

        public BOCSObject? Get(InstanceID instanceID)
        {
            return _instanceRegistry.Get(instanceID);
        }

        public bool TryGet(InstanceID instanceID, out BOCSObject? bocsObject)
        {
            return _instanceRegistry.TryGet(instanceID, out bocsObject);
        }

        public IEnumerable<BOCSObject> GetAll()
        {
            return _instanceRegistry.GetAll();
        }

        public bool TryGetBehaviourSaveData(InstanceID instanceID, out List<BehaviourSaveData>? behaviours)
        {
            return _instanceToBehaviourMap.TryGetValue(instanceID, out behaviours);
        }
    }
}
