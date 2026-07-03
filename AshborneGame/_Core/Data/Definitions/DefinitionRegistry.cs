using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    /// <summary>
    /// Registry for all the Definition IDs and their associated Definitions.
    /// </summary>
    public class DefinitionRegistry : IDefinitionRegistry
    {
        private readonly Dictionary<DefinitionID, Definition> _definitions = new();

        /// <summary>
        /// Gets the Definition associated with a Definition ID.
        /// </summary>
        public T Get<T>(DefinitionID id) where T : Definition
        {
            if (!_definitions.TryGetValue(id, out var def))
            {
                throw new ArgumentException($"Definition not found: {id}");
            }

            return (T)def;
        }

        public bool TryGet<T>(DefinitionID id, out T definition) where T : Definition
        {
            if (_definitions.TryGetValue(id, out var def) && def is T typed)
            {
                definition = typed;
                return true;
            }

            definition = null!;
            return false;
        }

        public void Register(Definition definition)
        {
            _definitions[definition.ID] = definition;
        }
    }
}
