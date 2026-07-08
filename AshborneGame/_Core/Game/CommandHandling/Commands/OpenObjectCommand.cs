using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class OpenObjectCommand : ICommand
    {
        public List<string> Names => ["open"];
        public string Description => "Opens an object.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Open what? Specify an object.");
                return false;
            }

            string objectName = string.Join(" ", args).Trim();
            Location loc = player.CurrentLocation;
            

            List<BOCSObject> possibleObjects = loc.ContainedObjects.Where(o => o.Name.Matches(objectName)).ToList();

            if (possibleObjects.Count() == 0)
            {
                await IOService.Output.DisplayFailMessage($"That is not an object here.");
                return false;
            }

            var allObjectsWithB = possibleObjects.Where(o => o.HasBehaviours<IInteractable>() == true).ToList();

            await IOService.Output.DisplayDebugMessage($"You are trying to open {objectName}.");
            await IOService.Output.DisplayDebugMessage($"The object has the following behaviours: {string.Join(", ", allObjectsWithB.Select(b => b.GetType().Name))}.");
            if (!allObjectsWithB.Any(b => b.GetAllBehaviours<IInteractable>().ToList().Any(b => b.GetType() == typeof(OpenCloseBehaviour))))
            {
                await IOService.Output.DisplayFailMessage($"You can't open that.");
                return false;
            }

            if (allObjectsWithB.Count() > 0)
            {
                throw new InvalidOperationException($"Multiple objects within the Location {player.CurrentLocation.Name} that have the name {allObjectsWithB[0].Name} and an IInteractable Behaviour.");
            }

            BOCSObject obj = allObjectsWithB[0];

            if (obj.ByBehaviour.Any(b => b.GetType() == typeof(LockUnlockBehaviour)))
            {
                var lockUnlockBehaviour = obj.ByBehaviour.FirstOrDefault(b => b is LockUnlockBehaviour) as LockUnlockBehaviour;
                if (lockUnlockBehaviour!.IsLocked)
                {
                    await IOService.Output.DisplayFailMessage($"You cannot open that because it is locked.");
                    return false;
                }
                var openCloseBehaviour = obj.ByBehaviour.FirstOrDefault(b => b is OpenCloseBehaviour) as OpenCloseBehaviour;
                openCloseBehaviour!.Interact(ObjectInteractionTypes.Open, player);
                return true;
            }
            else
            {
                var openCloseBehaviour = obj.ByBehaviour.FirstOrDefault(b => b is OpenCloseBehaviour) as OpenCloseBehaviour;
                openCloseBehaviour!.Interact(ObjectInteractionTypes.Open, player);
                return true;
            }
        }
    }
}
