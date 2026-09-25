using AshborneGame._Core.CognitiveSystem.EmotionSystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.DefinitionRules
{
    public record SelfEmotionRule(MemoryRole SubjectRole, EmotionType Emotion, double Value);
}