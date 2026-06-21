using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.MemorySystem
{
    /// <summary>
    /// Implemented by classes that can cause the creation of a Memory in an NPC, such as GameEvents.
    /// </summary>
    public interface ICanCauseMemories
    {
        public MemoryCauseCategories Category { get; init; }
    }
}
