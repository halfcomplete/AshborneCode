using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.SaveSystem.Data.CognitionDTOs;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public interface IMemorySource
    {
        int CurrentTotalHours { get; }
        MemoryDefinition MemoryDefinition { get; }
        List<MemoryParticipant> Participants { get; }
        DefinitionID LocationID { get; }

        public MemorySourceSaveData GetSaveData()
        {
            return new MemorySourceSaveData
            {
                CurrentTotalHours = CurrentTotalHours,
                MemoryDefinition = MemoryDefinition,
                Participants = Participants,
                LocationID = LocationID
            };
        }

        public static IMemorySource LoadFromSaveData(MemorySourceSaveData saveData)
        {
            return new SyntheticMemorySource(
                saveData.CurrentTotalHours,
                saveData.MemoryDefinition,
                saveData.Participants,
                saveData.LocationID
            );
        }
    }

    public sealed record SyntheticMemorySource(
        int CurrentTotalHours,
        MemoryDefinition MemoryDefinition,
        List<MemoryParticipant> Participants,
        DefinitionID LocationID) : IMemorySource;
}