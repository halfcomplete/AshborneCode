using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;

namespace AshborneGame._Core.SaveSystem.Data.CognitionDTOs
{
    public class MemorySourceSaveData
    {
        public int CurrentTotalHours { get; set; }
        public MemoryDefinition MemoryDefinition { get; set; } = null!;
        public List<MemoryParticipant> Participants { get; set; } = new();
        public DefinitionID LocationID { get; set; }
    }
}