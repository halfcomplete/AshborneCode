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
                IOService.Output.WriteLine("Open what? Specify an object.");
                return false;
            }

            string objectName = string.Join(" ", args).Trim();
            Sublocation? sublocation = player.CurrentSublocation;

            if (sublocation == null)
            {
                IOService.Output.WriteLine("There's nothing to open here.");
                return false;
            }

            var allBehaviours = sublocation.Object.GetAllBehaviours<IInteractable>();
            IOService.Output.DisplayDebugMessage($"You are trying to open {objectName}.");
            IOService.Output.DisplayDebugMessage($"The object has the following behaviours: {string.Join(", ", allBehaviours.Select(b => b.GetType().Name))}.");
            if (!allBehaviours.ToList().Any(b => b.GetType() == typeof(OpenCloseBehaviour)))
            {
                IOService.Output.WriteLine($"You can't open that.");
                return false;
            }

            if (allBehaviours.ToList().Any(b => b.GetType() == typeof(LockUnlockBehaviour)))
            {
                var lockUnlockBehaviour = allBehaviours.FirstOrDefault(b => b is LockUnlockBehaviour) as LockUnlockBehaviour;
                if (lockUnlockBehaviour!.IsLocked)
                {
                    IOService.Output.WriteLine($"You cannot open that because it is locked.");
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
