using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Data.IDSystem;
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
            saveData.BOCSObjects = instanceRegistry.GetAll().Select(obj => obj.GetSaveData(context)).ToList();
            saveData.Locations = locationRegistry.GetLocations().Select(loc => loc.GetSaveData()).ToList();
            saveData.InkStoryStateJson = inkRunner.Story?.state.ToJson() ?? throw new ArgumentNullException("CollectSaveData failed: Ink story is null");
            return saveData;
        }

        public static string Serialise(SaveGameData saveData)
        {
            var options = new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // camelCase for property names
                WriteIndented = true, // pretty print the JSON for readability
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // ignore null values
                Converters =
                {
                    new DefinitionIDJsonConverter(), // custom converter for DefinitionID
                    new InstanceIDJsonConverter(), // custom converter for InstanceID
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) // handle enums as strings in camelCase
                }
            };

            return JsonSerializer.Serialize(saveData, options);
        }

        public static void LoadGame(string saveJson, Player player, GameStateManager gameState, InkRunner inkRunner, IInstanceRegistry instanceRegistry, ILocationRegistry locationRegistry)
        {
            var saveData = Deserialise(saveJson);
            SaveLoadContext context = new SaveLoadContext(instanceRegistry, locationRegistry);

            LoadRegistries(saveData, context);

            LoadPlayerState(saveData.Player, context, player, gameState, inkRunner);
            LoadGameState(saveData.GameState, context, gameState);
            LoadInkStoryState(saveData.InkStoryStateJson, inkRunner);

            Console.WriteLine("Game loaded successfully.");
        }

        public static void LoadRegistries(SaveGameData saveData, SaveLoadContext context)
        {
            LoadLocationRegistry(saveData, context);
            LoadInstanceRegistry(saveData, context);
            LoadLocationSaveData(saveData, context);
        }

        private static void LoadInstanceRegistry(SaveGameData saveData, SaveLoadContext context)
        {
            // Load objects into the instance registry
            foreach (var objData in saveData.BOCSObjects)
            {
                var obj = BOCSObject.LoadFromSaveData(objData, context);
                context.InstanceRegistry.Register(obj);
            }
        }

        /// <summary>
        /// Loads locations into the location registry. Does not fully load the save data 
        /// into them as this is only part of the first stage of loading, when the aim is to make sure everything EXISTS.
        /// </summary>
        /// <param name="saveData"></param>
        /// <param name="context"></param>
        private static void LoadLocationRegistry(SaveGameData saveData, SaveLoadContext context)
        {
            // TODO: double check; is it possible for there not to be every single location detailed in the save data?
            //       If so, we may need to handle missing locations by creating them from definitions, or logging a warning.

            // Load locations into the location registry
            foreach (var locData in saveData.Locations)
            {
                // first build locations from definitions using WorldBuilder
                // then load the save data into them
                // then register the location

                // note that this will also create the scene and register it with the scene registry, as part of the location creation process
                // this is on a first-come-first-served basis, so if a scene is already registered, it won't be re-registered
                // this means the first scene instance created for a given definition ID will be the one that is registered,
                // and subsequent locations with the same scene definition will use their own scene instances, but the registered scene will be the first one created
                // this is important to note because it means that if you have multiple locations with the same scene definition, they will not share the same scene instance, but will share the same registered scene
                // this shouldn't be a problem because scenes are immutable and should not have any state that changes during gameplay, but it's worth noting in case it causes confusion later on
                var location = WorldBuilder.CreateLocationFromDefinition(locData.DefinitionId);

                context.LocationRegistry.RegisterLocation(location);


                context.LocationRegistry.RegisterScene(location.Scene);
            }
        }

        private static void LoadLocationSaveData(SaveGameData saveData, SaveLoadContext context)
        {
            foreach (var locData in saveData.Locations)
            {
                var location = context.ResolveLocation(locData.DefinitionId);
                location.LoadSaveData(locData, context);
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
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase, // camelCase for property names
                WriteIndented = true, // pretty print the JSON for readability
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull, // ignore null values
                Converters =
                {
                    new DefinitionIDJsonConverter(), // custom converter for DefinitionID
                    new InstanceIDJsonConverter(), // custom converter for InstanceID
                    new JsonStringEnumConverter(JsonNamingPolicy.CamelCase) // handle enums as strings in camelCase
                }
            };

            return JsonSerializer.Deserialize<SaveGameData>(json, options) ?? throw new InvalidOperationException("Deserialisation failed: Resulting SaveGameData is null.");
        }
    }
}
