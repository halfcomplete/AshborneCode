using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
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
            Sublocation? sublocation = player.CurrentSublocation;
            IEnumerable<IInteractable>? allIInteractableBehaviours = null;
            allIInteractableBehaviours = sublocation?.FocusObject.GetAllBehaviours<IInteractable>();

            if (allIInteractableBehaviours == null)
            {
                await IOService.Output.DisplayFailMessage($"That is not an object here.");
                return false;
            }

            await IOService.Output.DisplayDebugMessage($"You are trying to open {objectName}.");
            await IOService.Output.DisplayDebugMessage($"The object has the following behaviours: {string.Join(", ", allIInteractableBehaviours.Select(b => b.GetType().Name))}.");
            if (!allIInteractableBehaviours.ToList().Any(b => b.GetType() == typeof(OpenCloseBehaviour)))
            {
                await IOService.Output.DisplayFailMessage($"You can't open that.");
                return false;
            }

            if (allIInteractableBehaviours.ToList().Any(b => b.GetType() == typeof(LockUnlockBehaviour)))
            {
                var lockUnlockBehaviour = allIInteractableBehaviours.FirstOrDefault(b => b is LockUnlockBehaviour) as LockUnlockBehaviour;
                if (lockUnlockBehaviour!.IsLocked)
                {
                    await IOService.Output.DisplayFailMessage($"You cannot open that because it is locked.");
                    return false;
                }
                var openCloseBehaviour = allIInteractableBehaviours.FirstOrDefault(b => b is OpenCloseBehaviour) as OpenCloseBehaviour;
                openCloseBehaviour!.Interact(ObjectInteractionTypes.Open, player);
                return true;
            }
            else
            {
                var openCloseBehaviour = allIInteractableBehaviours.FirstOrDefault(b => b is OpenCloseBehaviour) as OpenCloseBehaviour;
                openCloseBehaviour!.Interact(ObjectInteractionTypes.Open, player);
                return true;
            }
        }
    }
}
