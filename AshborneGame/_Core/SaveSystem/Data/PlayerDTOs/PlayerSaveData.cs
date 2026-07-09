using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Data.CognitionDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data.PlayerDTOs
{
    public sealed class PlayerSaveData
    {
        public InstanceID InstanceId { get; set; }
        public DefinitionID DefinitionId { get; set; }
        public DefinitionID CurrentLocationDefinitionId { get; set; }
        public DefinitionID CurrentSceneDefinitionId { get; set; }
        public DefinitionID? PreviousLocationDefinitionId { get; set; }
        public InventorySaveData Inventory { get; set; } = null!;
        public Dictionary<string, InstanceID?> EquippedItems { get; set; } = new();  // slot -> item InstanceId
        public StatCollectionSaveData Stats { get; set; } = null!;
        public PsychologicalStateSaveData PsychologicalState { get; set; } = null!;
        public InstanceID? CurrentNpcInteractionInstanceId { get; set; }
        public InstanceID? CurrentMaskInstanceId { get; set; }
        public int Visibility { get; set; }
    }
}
