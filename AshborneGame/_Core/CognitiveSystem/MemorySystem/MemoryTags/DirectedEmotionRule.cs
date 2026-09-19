using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record DirectedEmotionRule(
        MemoryRole SubjectRole,
        MemoryRole TargetRole,
        EmotionType Emotion,
        double Value);
}