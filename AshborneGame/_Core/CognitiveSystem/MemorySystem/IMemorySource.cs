using AshborneGame._Core.Game.Events;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public interface IMemorySource
    {
        int CurrentTotalHours { get; }
        MemoryDefinition MemoryDefinition { get; }
        List<MemoryParticipant> Participants { get; }
        Guid LocationID { get; }
    }

    public sealed record SyntheticMemorySource(
        int CurrentTotalHours,
        MemoryDefinition MemoryDefinition,
        List<MemoryParticipant> Participants,
        Guid LocationID,
        string SourceLabel) : IMemorySource;
}