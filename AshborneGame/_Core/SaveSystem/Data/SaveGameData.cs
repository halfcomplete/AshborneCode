using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Data.LocationDTO;
using AshborneGame._Core.SaveSystem.Data.PlayerDTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data
{
    public sealed class SaveGameData
    {
        public static readonly int CurrentVersion = 1;
        public SaveMetadata Metadata { get; set; } = null!;
        public PlayerSaveData Player { get; set; } = null!;
        public GameStateSaveData GameState { get; set; } = null!;
        public List<BOCSObjectSaveData> BOCSObjects { get; set; } = new();
        public List<LocationSaveData> Locations { get; set; } = new();
        public DialogueSaveData Dialogue { get; set; } = new();
    }
}
