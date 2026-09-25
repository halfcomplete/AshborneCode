using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.DefinitionRules
{
    public record PersonalityIntensityRule(
        PersonalityTrait Trait,
        MemoryRole SubjectRole,
        double Value);
}