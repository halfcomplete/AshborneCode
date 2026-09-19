using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record AttitudeRule(
        MemoryRole SubjectRole,
        MemoryRole TargetRole,
        RelationshipType Relationship,
        double Modifier);
}