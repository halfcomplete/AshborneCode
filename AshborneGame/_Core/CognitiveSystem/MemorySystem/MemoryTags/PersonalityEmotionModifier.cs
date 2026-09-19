using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record PersonalityEmotionModifier(
        PersonalityTrait Trait,
        MemoryRole SubjectRole,
        MemoryRole? TargetRole,
        EmotionType Emotion,
        double Multiplier,
        double Value);
}