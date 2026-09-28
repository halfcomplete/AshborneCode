using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions.Registries
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
        /// <returns>The Definition if it was found, otherwise null.</returns>
        public T Get<T>(DefinitionID id) where T : Definition
        {
            if (!_definitions.TryGetValue(id, out var def))
            {
                throw new ArgumentException($"Definition not found: {id}");
            }

            return (T)def;
        }

        /// <summary>
        /// Tries to get the Definition associated with a Definition ID.
        /// </summary>
        /// <typeparam name="T">The type of the Definition to get.</typeparam>
        /// <param name="id">The DefinitionID of the Definition to get.</param>
        /// <param name="definition">The Definition if it was found, otherwise null.</param>
        /// <returns>true if the Definition was found, otherwise false.</returns>
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

        /// <summary>
        /// Registers a Definition with the registry.
        /// </summary>
        public void Register(Definition definition)
        {
            _definitions[definition.ID] = definition;
        }
    }
}
