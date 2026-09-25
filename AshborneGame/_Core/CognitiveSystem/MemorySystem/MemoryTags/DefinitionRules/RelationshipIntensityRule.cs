using AshborneGame._Core.CognitiveSystem.MemorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.DefinitionRules
{
    public record RelationshipIntensityRule(MemoryRole Subject, MemoryRole Target, RelationshipType Relationship, double Value);
}