using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record PersonalityIntensityModifier(
        PersonalityTrait Trait,
        MemoryRole SubjectRole,
        double Value);
}