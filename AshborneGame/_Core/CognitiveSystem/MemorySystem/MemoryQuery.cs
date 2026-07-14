using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public record MemoryQuery
    {
        public HashSet<MemoryTagType>? Tags { get; init; }
        public Dictionary<DefinitionID, List<MemoryRole>?>? Participants { get; init; }
        public List<DefinitionID>? Locations { get; init; }
    }
}