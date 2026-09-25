using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.DefinitionRules
{
    public record RelationshipEmotionRule(MemoryRole Subject, MemoryRole Target, RelationshipType Relationship, EmotionType Emotion, double Value);
}