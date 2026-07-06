using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;

namespace AshborneGame._Core.LocationManagement
{
    public class WorldBuilder
    {
        public void Initialise(ILocationRegistry registry, IDefinitionRegistry definitionRegistry, BOCSFactory factory)
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
                var location = new Location(locationDefinition.Name, locationDefinition.DefinitionID);
                location.SetDescriptionComposer(locationDefinition.DescriptionComposer);

                foreach (var containedObjectDefinitionId in locationDefinition.ContainedObjects)
                {
                    var containedObject = factory.Create(containedObjectDefinitionId);
                    location.AddObject(containedObject);
                }

                foreach (var (command, commandHandlers) in locationDefinition.CustomCommands.GetCommands())
                {
                    location.CustomCommands.AddCustomCommand(command, commandHandlers.Message, commandHandlers.Effect);
                }

                if (!scenesByDefinition.TryGetValue(locationDefinition.Scene, out var scene))
                {
                    throw new KeyNotFoundException($"Definition ID {locationDefinition.Scene} not found in scenesByDefinition.");
                }

                scene.AddLocation(location);
                locationsByDefinition[locationDefinition.DefinitionID] = location;
            }

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
            }

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

            foreach (var location in locationsByDefinition.Values)
            {
                registry.RegisterLocation(location);
            }

            foreach (var scene in scenesByDefinition.Values)
            {
                registry.RegisterScene(scene);
            }
        }

        public void RemoveParentChildRelationship(ILocationRegistry registry, DefinitionID parent, DefinitionID child)
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