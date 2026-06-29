using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.MemorySystem.MemoryTags
{
    public interface IMemoryTag
    {
        public MemoryTag Type { get; }
        public MemoryTagDefinition Definition { get; }
    }
}