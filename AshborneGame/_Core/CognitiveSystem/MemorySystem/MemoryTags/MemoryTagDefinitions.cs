using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public static class MemoryTagDefinitions
    {
        public static Dictionary<MemoryTag, IMemoryTag> Definitions = 
            new Dictionary<MemoryTag, IMemoryTag> 
            {
                [MemoryTag.Theft] = new TheftMemoryTag(),
            };
    }
}