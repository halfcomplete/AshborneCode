using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record SelfEmotionRule(MemoryRole SubjectRole, EmotionType Emotion, double Value);
}