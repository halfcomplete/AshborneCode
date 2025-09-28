using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

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
            Sublocation? sublocation = player.CurrentSublocation;

            if (sublocation == null)
            {
                await IOService.Output.DisplayFailMessage("There's nothing to close here.");
                return false;
            }

            if (sublocation.FocusObject.TryGetBehaviour<IInteractable>(out var openCloseBehaviour).Result && openCloseBehaviour is ContainerBehaviour)
            {
                if (sublocation.FocusObject.TryGetBehaviour<IInteractable>(out var lockUnlockBehaviour).Result && lockUnlockBehaviour is LockUnlockBehaviour)
                {
                    LockUnlockBehaviour lockUnlockBehaviour1 = (LockUnlockBehaviour)lockUnlockBehaviour;
                    openCloseBehaviour.Interact(ObjectInteractionTypes.Close, player);
                }
                return false;
            }
            else
            {
                await IOService.Output.DisplayFailMessage($"You cannot close that.");
                return false;
            }
        }
    }
}
