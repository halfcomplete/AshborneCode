using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.DefinitionRules
{
    public record PersonalityEmotionRule(
        PersonalityTrait Trait,
        MemoryRole SubjectRole,
        EmotionType Emotion,
        double Value);
}