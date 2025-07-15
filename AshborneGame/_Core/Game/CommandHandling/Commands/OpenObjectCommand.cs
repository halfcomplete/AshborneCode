using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class OpenObjectCommand : ICommand
    {
        public string Name => "open";
        public string Description => "Opens an object.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                IOService.Output.DisplayFailMessage("Open what? Specify an object.");
                return false;
            }

            string objectName = string.Join(" ", args).Trim();
            Sublocation? location = (Sublocation)player.CurrentSublocation;

            if (location == null)
            {
                location = player.CurrentLocation;
            }

            var allBehaviours = location.Objects.FirstOrDefault(o => o.Name.Equals(objectName))?.GetAllBehaviours<IInteractable>();
            if (allBehaviours == null)
            {
                IOService.Output.DisplayFailMessage($"That is not an object here.");
                return false;
            }
            IOService.Output.DisplayDebugMessage($"You are trying to open {objectName}.");
            IOService.Output.DisplayDebugMessage($"The object has the following behaviours: {string.Join(", ", allBehaviours.Select(b => b.GetType().Name))}.");
            if (!allBehaviours.ToList().Any(b => b.GetType() == typeof(OpenCloseBehaviour)))
            {
                IOService.Output.DisplayFailMessage($"You can't open that.");
                return false;
            }

            if (allBehaviours.ToList().Any(b => b.GetType() == typeof(LockUnlockBehaviour)))
            {
                var lockUnlockBehaviour = allBehaviours.FirstOrDefault(b => b is LockUnlockBehaviour) as LockUnlockBehaviour;
                if (lockUnlockBehaviour!.IsLocked)
                {
                    IOService.Output.DisplayFailMessage($"You cannot open that because it is locked.");
                    return false;
                }
                var openCloseBehaviour = allBehaviours.FirstOrDefault(b => b is OpenCloseBehaviour) as OpenCloseBehaviour;
                openCloseBehaviour!.Interact(ObjectInteractionTypes.Open, player);
                return true;
            }
            else
            {
                var openCloseBehaviour = allBehaviours.FirstOrDefault(b => b is OpenCloseBehaviour) as OpenCloseBehaviour;
                openCloseBehaviour!.Interact(ObjectInteractionTypes.Open, player);
                return true;
            }
        }
    }
}
