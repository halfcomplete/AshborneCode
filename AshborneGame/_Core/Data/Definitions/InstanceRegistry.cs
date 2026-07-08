using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    public sealed class InstanceRegistry : IInstanceRegistry
    {
        private readonly Dictionary<InstanceID, BOCSObject> _byInstance = new();

        private readonly Dictionary<DefinitionID, HashSet<InstanceID>> _byDefinition = new();

        public void Register(BOCSObject obj)
        {
            _byInstance[obj.InstanceID] = obj;

            if (!_byDefinition.TryGetValue(obj.DefinitionID, out var set))
            {
                set = new HashSet<InstanceID>();
                _byDefinition[obj.DefinitionID] = set;
            }

            set.Add(obj.InstanceID);
        }

        public void Unregister(BOCSObject obj)
        {
            _byInstance.Remove(obj.InstanceID);

            if (_byDefinition.TryGetValue(obj.DefinitionID, out var set))
            {
                set.Remove(obj.InstanceID);

                if (set.Count == 0)
                    _byDefinition.Remove(obj.DefinitionID);
            }
        }

        public BOCSObject Get(InstanceID id)
        {
            if (!_byInstance.TryGetValue(id, out var obj))
                throw new Exception($"Instance not found: {id}");

            return obj;
        }

        public bool TryGet(InstanceID id, out BOCSObject? obj)
        {
            return _byInstance.TryGetValue(id, out obj);
        }

        public IEnumerable<BOCSObject> GetByDefinition(DefinitionID id)
        {
            if (_byDefinition.TryGetValue(id, out var set))
            {
                List<BOCSObject> b = new();

                foreach (var i in set)
                {
                    b.Add(_byInstance[i]);
                }

                return b;
            }

            return Enumerable.Empty<BOCSObject>();
        }
    } 
}
