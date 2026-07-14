using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions.Registries
{
    public interface IInstanceRegistry
    {
        void Register(BOCSObject obj);
        void Unregister(BOCSObject obj);

        BOCSObject Get(InstanceID id);
        bool TryGet(InstanceID id, out BOCSObject? obj);

        IEnumerable<BOCSObject> GetByDefinition(DefinitionID id);

        IEnumerable<BOCSObject> GetAll();
    }
}
