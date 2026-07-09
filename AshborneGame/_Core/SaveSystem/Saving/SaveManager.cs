using AshborneGame._Core._Player;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Game;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Data;
using AshborneGame._Core.SaveSystem.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Saving
{
    public static class SaveManager
    {
        public static string SerialiseSaveData(IInstanceRegistry instanceRegistry, IDefinitionRegistry definitionRegistry, ILocationRegistry locationRegistry, Player player, GameStateManager gameState, InkRunner inkRunner)
        {
            SaveGameData data = CollectSaveData(new SaveLoadContext(), player, gameState, instanceRegistry, locationRegistry, inkRunner);

            return Serialise(data);
        }

        public static string SerialiseSaveData()
        {
            return SerialiseSaveData(GameContext.InstanceRegistry, GameContext.DefinitionRegistry, GameContext.LocationRegistry, GameContext.Player, GameContext.GameState, GameContext.InkRunner);
        }

        public static SaveGameData CollectSaveData(SaveLoadContext context, Player player, GameStateManager gameState, IInstanceRegistry instanceRegistry, ILocationRegistry locationRegistry, InkRunner inkRunner)
        {
            SaveGameData saveData = new SaveGameData();
            saveData.Metadata = new SaveMetadata { Version = SaveGameData.CurrentVersion, SavedAt = DateTime.Now };
            saveData.Player = player.GetSaveData();
            saveData.GameState = gameState.GetSaveData();
            saveData.Objects = instanceRegistry.GetAll().Select(obj => obj.GetSaveData(context)).ToList();
            saveData.Locations = locationRegistry.GetLocations().Select(loc => loc.GetSaveData()).ToList();
            saveData.InkStoryStateJson = inkRunner.Story?.ToJson() ?? throw new ArgumentNullException("CollectSaveData failed: Ink story is null");
            return saveData;
        }

        public static string Serialise(SaveGameData saveData)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // camelCase keys
                WriteIndented = true,                              // Human-readable format
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Handle enums as strings
            };

            return JsonSerializer.Serialize(saveData, options);
        }
    }
}
