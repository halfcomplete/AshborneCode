using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public static class MemoryTagDefinitions
    {
        public static Dictionary<MemoryTagType, IMemoryTag> Definitions = 
            new Dictionary<MemoryTagType, IMemoryTag> 
            {
                [MemoryTagType.Theft] = new TheftMemoryTag(),
                [MemoryTagType.Betrayal] = new BetrayalMemoryTag(),
                [MemoryTagType.Deception] = new DeceptionMemoryTag(),
            };
    }
}