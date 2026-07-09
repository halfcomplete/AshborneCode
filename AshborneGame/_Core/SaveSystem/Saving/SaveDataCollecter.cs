using AshborneGame._Core._Player;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Game;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Saving
{
    public class SaveDataCollecter
    {
        public SaveGameData CollectSaveData(Player player, GameStateManager gameState, IInstanceRegistry instanceRegistry, ILocationRegistry locationRegistry, InkRunner inkRunner)
        {
            SaveGameData saveData = new SaveGameData();
            saveData.Metadata = new SaveMetadata { Version = SaveGameData.CurrentVersion, SavedAt = DateTime.Now };
            saveData.Player = player.GetSaveData();
            saveData.GameState = gameState.GetSaveData();
            saveData.Objects = instanceRegistry.GetAll().Select(obj => obj.()).ToList();
            saveData.Locations = locationRegistry.GetLocations().Select(loc => loc.GetSaveData()).ToList();
            saveData.InkStoryStateJson = inkRunner.Story?.ToJson() ?? throw new ArgumentNullException("CollectSaveData failed: Ink story is null");
            return saveData;
        }
    }
}
