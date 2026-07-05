using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.LocationManagement
{
    public class WorldBuilder
    {
        public void Initialise(ILocationRegistry registry)
        {
            ArgumentNullException.ThrowIfNull(registry);

            if (GameContext.DefinitionRegistry is null)
            {
                throw new InvalidOperationException("Definition registry is not initialised.");
            }

            if (GameContext.BOCSFactory is null)
            {
                throw new InvalidOperationException("BOCS factory is not initialised.");
            }

            var locationsByDefinition = new Dictionary<DefinitionID, Location>();
            var scenesByDefinition = new Dictionary<DefinitionID, Scene>();

            foreach (var locationDefinition in LocationDefinitions.All)
            {
                var location = new Location(locationDefinition.Name, locationDefinition.DefinitionID);
                location.SetDescriptionComposer(locationDefinition.DescriptionComposer);
                location.Name.SetParentLocation(location);

                foreach (var containedObjectDefinitionId in locationDefinition.ContainedObjects)
                {
                    var containedObject = GameContext.BOCSFactory.Create(containedObjectDefinitionId);
                    location.AddObject(containedObject);
                }

                foreach (var (command, commandHandlers) in locationDefinition.CustomCommands)
                {
                    location.CustomCommands[command] = commandHandlers;
                }

                if (!scenesByDefinition.TryGetValue(locationDefinition.Scene, out var scene))
                {
                    scene = new Scene(locationDefinition.Scene.Value, locationDefinition.Scene.Value);
                    scenesByDefinition[locationDefinition.Scene] = scene;
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
                    throw new KeyNotFoundException($"Target location definition '{exitDefinition.to}' was not built.");
                }

                sourceLocation.AddExit(exitDefinition.From());
                targetLocation.AddExit(exitDefinition.To());
            }

            foreach (var location in locationsByDefinition.Values)
            {
                registry.RegisterLocation(location);
            }
        }
    }
}