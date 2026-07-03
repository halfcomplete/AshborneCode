using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    public interface IDefinitionRegistry
    {
        T Get<T>(DefinitionID id) where T : Definition;
        bool TryGet<T>(DefinitionID id, out T definition) where T : Definition;

        void Register(Definition definition);
    }
}
