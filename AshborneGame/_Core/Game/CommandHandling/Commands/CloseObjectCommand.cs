using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class CloseObjectCommand : ICommand
    {
        public List<string> Names => ["close"];
        public string Description => "Closes an object.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Close what? Specify an object.");
                return false;
            }

            string objectName = string.Join(" ", args).Trim();
            Location? location = player.CurrentLocation;

            if (location == null)
            {
                await IOService.Output.DisplayFailMessage("There's nothing to close here.");
                return false;
            }
            // TODO: add helper method to match names
            List<BOCSObject> focusObjects = location.ContainedObjects.Where(o => o.Name.Matches(objectName)).ToList();

            foreach (var obj in focusObjects)
            {
                (bool hasOpenCloseBehaviour, var openCloseBehaviour) = await obj.TryGetBehaviour<IInteractable>();

                if (hasOpenCloseBehaviour && openCloseBehaviour is ContainerBehaviour)
                {
                    (bool hasLockUnlockBehaviour, var lockUnlockBehaviour) = await obj.TryGetBehaviour<IInteractable>();
                    if (hasLockUnlockBehaviour && lockUnlockBehaviour is LockUnlockBehaviour)
                    {
                        LockUnlockBehaviour lockUnlockBehaviour1 = (LockUnlockBehaviour)lockUnlockBehaviour;
                        openCloseBehaviour.Interact(ObjectInteractionTypes.Close, player);
                        return true;
                    }
                }
            }

            await IOService.Output.DisplayFailMessage($"You cannot close that.");
            return false;
        }
    }
}
