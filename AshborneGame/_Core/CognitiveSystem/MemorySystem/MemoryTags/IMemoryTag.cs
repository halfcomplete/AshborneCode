using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public interface IMemoryTag
    {
        public MemoryTagType Type { get; }
        public MemoryTagDefinition Definition { get; }
    }
}