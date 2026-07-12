using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions.LocationSpecific;
using AshborneGame._Core.Data.Definitions.Registries;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.LocationManagement
{
    public class WorldBuilder
    {
        public static void Initialise(ILocationRegistry registry, IDefinitionRegistry definitionRegistry, BOCSFactory factory)
        {
            ArgumentNullException.ThrowIfNull(registry);

            if (definitionRegistry is null)
            {
                throw new InvalidOperationException("Definition registry is not initialised.");
            }

            if (factory is null)
            {
                throw new InvalidOperationException("BOCS factory is not initialised.");
            }

            var locationsByDefinition = new Dictionary<DefinitionID, Location>();
            var scenesByDefinition = new Dictionary<DefinitionID, Scene>();

            foreach (var (definitionID, sceneDefinition) in SceneDefinitions.Definitions)
            {
                var scene = new Scene(definitionID, sceneDefinition.SceneName);
                scenesByDefinition.Add(definitionID, scene);
            }

            foreach (var locationDefinition in LocationDefinitions.All)
            {
                // dependent on definition ID
                var location = new Location(locationDefinition.Name, locationDefinition.DefinitionID);
                location.SetDescriptionComposer(locationDefinition.DescriptionComposer);
                if (!scenesByDefinition.TryGetValue(locationDefinition.Scene, out var scene))
                {
                    throw new KeyNotFoundException($"Definition ID {locationDefinition.Scene} not found in scenesByDefinition.");
                }
                InitialiseLocationCustomCommands(location, locationDefinition.CustomCommands);

                // dependent on save data
                InitialiseLocationContainedObjects(location, locationDefinition.ContainedObjects, factory);

                scene.AddLocation(location);
                locationsByDefinition[locationDefinition.DefinitionID] = location;
            }

            InitialiseLocationHierarchy(locationsByDefinition);
            InitialiseLocationExits(locationsByDefinition);

            foreach (var location in locationsByDefinition.Values)
            {
                registry.RegisterLocation(location);
            }

            foreach (var scene in scenesByDefinition.Values)
            {
                registry.RegisterScene(scene);
            }
        }

        /// <summary>
        /// Creates a Location instance from a given DefinitionID.
        /// This method ONLY initialises the Location's name, description composer, custom commands and scene. 
        /// It does NOT initialise contained objects or parent-child relationships, which are dependent
        /// on save data and the location hierarchy, respectively.
        /// </summary>
        /// <remarks>
        /// Intended to be used in the first stage of loading.
        /// </remarks>
        /// <exception cref="KeyNotFoundException">Thrown when the location definition is not found.</exception>
        public static Location CreateLocationFromDefinition(DefinitionID definitionID)
        {
            ArgumentNullException.ThrowIfNull(definitionID);
            var locationDefinition = LocationDefinitions.All.FirstOrDefault(l => l.DefinitionID == definitionID);
            if (locationDefinition == null)
            {
                throw new KeyNotFoundException($"Location definition '{definitionID}' was not found.");
            }
            var location = new Location(locationDefinition.Name, locationDefinition.DefinitionID);
            location.SetDescriptionComposer(locationDefinition.DescriptionComposer);

            location.Scene = CreateSceneFromDefinition(GetSceneDefinition(locationDefinition.Scene));

            InitialiseLocationCustomCommands(location, locationDefinition.CustomCommands);

            return location;
        }

        public static SceneDefinition GetSceneDefinition(DefinitionID definitionID)
        {
            ArgumentNullException.ThrowIfNull(definitionID);
            return SceneDefinitions.Definitions.GetValueOrDefault(definitionID) ?? throw new KeyNotFoundException($"Scene definition '{definitionID}' was not found.");
        }

        public static Scene CreateSceneFromDefinition(SceneDefinition sceneDefinition)
        {
            ArgumentNullException.ThrowIfNull(sceneDefinition);
            var scene = new Scene(sceneDefinition.ID, sceneDefinition.SceneName);
            return scene;
        }

        private static void InitialiseLocationCustomCommands(Location location, CustomCommandHandler customCommands)
        {
            foreach (var (command, commandHandlers) in customCommands.GetCommands())
            {
                location.CustomCommands.AddCustomCommand(command, commandHandlers.Message, commandHandlers.Effect);
            }
        }

        private static void InitialiseLocationContainedObjects(Location location, IReadOnlyList<DefinitionID> containedObjects, BOCSFactory factory)
        {
            foreach (var containedObjectDefinitionId in containedObjects)
            {
                var containedObject = factory.Create(containedObjectDefinitionId);
                location.AddObject(containedObject);
            }
        }

        public static void InitialiseLocationHierarchy(Dictionary<DefinitionID, Location> locationsByDefinition)
        {
            foreach (var parentChildDefinition in LocationDefinitionGraph.Hierarchy)
            {
                if (!locationsByDefinition.TryGetValue(parentChildDefinition.parent, out var parentLocation))
                {
                    throw new KeyNotFoundException($"Parent location definition '{parentChildDefinition.parent}' was not built.");
                }

                if (!locationsByDefinition.TryGetValue(parentChildDefinition.child, out var childLocation))
                {
                    throw new KeyNotFoundException($"Child location definition '{parentChildDefinition.child}' was not built.");
                }

                parentLocation.AddChild(childLocation);
                childLocation.AddExit(new Exit(parentLocation.DefinitionID, DirectionConstants.Back));
            }
        }

        public static void InitialiseLocationExits(Dictionary<DefinitionID, Location> locationsByDefinition)
        {
            foreach (var exitDefinition in LocationDefinitionGraph.Exits)
            {
                if (!locationsByDefinition.TryGetValue(exitDefinition.from, out var sourceLocation))
                {
                    throw new KeyNotFoundException($"Source location definition '{exitDefinition.from}' was not built.");
                }

                if (!locationsByDefinition.TryGetValue(exitDefinition.to, out var targetLocation))
                {
                    throw new KeyNotFoundException($"Target location definition '{exitDefinition.to}' was not built. Built definitions: {string.Join(", ", locationsByDefinition.Keys)}");
                }

                sourceLocation.AddExit(exitDefinition.FromFrom());
                targetLocation.AddExit(exitDefinition.FromTo());
            }
        }

        public static void RemoveParentChildRelationship(ILocationRegistry registry, DefinitionID parent, DefinitionID child)
        {
            ArgumentNullException.ThrowIfNull(parent);
            ArgumentNullException.ThrowIfNull(child);
            if (!registry.TryGetLocationByDefinitionID(parent, out var parentLocation))
            {
                throw new KeyNotFoundException($"Parent location definition '{parent}' was not found.");
            }
            if (!registry.TryGetLocationByDefinitionID(child, out var childLocation))
            {
                throw new KeyNotFoundException($"Child location definition '{child}' was not found.");
            }
            parentLocation.RemoveChild(childLocation);
        }
    }
}