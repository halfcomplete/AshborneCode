using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Game.Events;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public record MemoryQuery
    {
        public HashSet<MemoryTag>? Tags { get; init; }
        public Dictionary<Guid, List<MemoryRole>?>? Participants { get; init; }
        public List<Guid>? Locations { get; init; }
    }
}