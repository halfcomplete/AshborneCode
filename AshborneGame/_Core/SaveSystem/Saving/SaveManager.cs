using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Game;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Data;
using AshborneGame._Core.SaveSystem.Data.PlayerDTOs;
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
            SaveGameData data = CollectSaveData(new SaveLoadContext(instanceRegistry, locationRegistry), player, gameState, instanceRegistry, locationRegistry, inkRunner);

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
            saveData.InkStoryStateJson = inkRunner.Story?.state.ToJson() ?? throw new ArgumentNullException("CollectSaveData failed: Ink story is null");
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

        public static void CreateContextAndLoadRegistries(string json, Player player, GameStateManager gameState, InkRunner inkRunner)
        {
            SaveGameData saveData = Deserialise(json);
            SaveLoadContext context = new SaveLoadContext(new InstanceRegistry(), new LocationRegistry());

            LoadRegistries(saveData, context);

            LoadPlayerState(saveData.Player, context, player, gameState, inkRunner);
            LoadGameState(saveData.GameState, context, gameState);
            LoadInkStoryState(saveData.InkStoryStateJson, inkRunner);
        }

        public static void LoadRegistries(SaveGameData saveData, SaveLoadContext context)
        {
            LoadInstanceRegistry(saveData, context);
            LoadLocationRegistry(saveData, context);
        }

        private static void LoadInstanceRegistry(SaveGameData saveData, SaveLoadContext context)
        {
            IInstanceRegistry instanceRegistry = context.InstanceRegistry;

            // Load objects into the instance registry
            foreach (var objData in saveData.Objects)
            {
                var obj = BOCSObject.LoadFromSaveData(objData, context);
                instanceRegistry.Register(obj);
            }
        }

        private static void LoadLocationRegistry(SaveGameData saveData, SaveLoadContext context)
        {
            ILocationRegistry locationRegistry = context.LocationRegistry;

            // Load locations into the location registry
            foreach (var locData in saveData.Locations)
            {
                // first build locations from definitions using WorldBuilder
                // then load the save data into them
                // locationRegistry.RegisterLocation(location);
            }
        }

        public static void LoadPlayerState(PlayerSaveData saveData, SaveLoadContext context, Player player, GameStateManager gameState, InkRunner inkRunner)
        {
            if (saveData == null)
            {
                throw new InvalidOperationException("LoadPlayerState failed: PlayerSaveData is null.");
            }

            player.LoadFromSaveData(saveData, context);
        }

        private static void LoadGameState(GameStateSaveData saveData, SaveLoadContext context, GameStateManager gameState)
        {
            if (saveData == null)
            {
                throw new InvalidOperationException("LoadGameState failed: GameState is null.");
            }

            gameState.LoadSaveData(saveData, context);
        }

        private static void LoadInkStoryState(string storyStateJson, InkRunner inkRunner)
        {
            if (storyStateJson == null)
            {
                throw new InvalidOperationException("LoadInkStoryState failed: InkStoryStateJson is null.");
            }

            inkRunner.Story?.state.LoadJson(storyStateJson);
        }

        public static SaveGameData Deserialise(string json)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true, // Allow case-insensitive property matching
                Converters = { new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) } // Handle enums as strings
            };
            return JsonSerializer.Deserialize<SaveGameData>(json, options) ?? throw new InvalidOperationException("Deserialisation failed: Resulting SaveGameData is null.");
        }
    }
}
