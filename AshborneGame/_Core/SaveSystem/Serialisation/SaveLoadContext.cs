using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions.Registries;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.LocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Serialisation
{
    public sealed class SaveLoadContext
    {
        public IInstanceRegistry InstanceRegistry { get; }
        public ILocationRegistry LocationRegistry { get; }

        public SaveLoadContext(IInstanceRegistry instanceRegistry, ILocationRegistry locationRegistry)
        {
            InstanceRegistry = instanceRegistry;
            LocationRegistry = locationRegistry;
        }

        // Save: object -> ID
        public InstanceID ToId(BOCSObject obj) => obj.InstanceID;
        public DefinitionID ToId(Location loc) => loc.DefinitionID;

        // Load phase 2 only: ID -> object
        public BOCSObject ResolveObject(InstanceID id)
        {
            if (!InstanceRegistry.TryGet(id, out var obj))
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

            if (!InstanceRegistry.TryGet(id.Value, out var obj))
            {
                return null;
            }

            return obj as T;
        }
    }
}
