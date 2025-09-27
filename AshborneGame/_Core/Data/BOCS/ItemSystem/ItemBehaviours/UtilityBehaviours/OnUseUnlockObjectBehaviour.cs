using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours
{
    public class OnUseUnlockObjectBehaviour : ItemBehaviourBase<OnUseUnlockObjectBehaviour>, IActOnUse, IUnlocksTarget
    {
        public bool ConsumeOnUse { get; set; }

        public List<string> UnlockableObjectIDs { get; private set; }

        public OnUseUnlockObjectBehaviour(List<string> unlockableObjectIDs, bool consumeOnUse = true)
        {
            UnlockableObjectIDs = unlockableObjectIDs ?? throw new ArgumentNullException(nameof(unlockableObjectIDs), "Unlockable object IDs cannot be null.");
            ConsumeOnUse = consumeOnUse;
        }

        public void OnUse(Player player)
        {
            IOService.Output.DisplayDebugMessage("On Use Trigger successfully called for OnUseUnlockObjectBehaviour", ConsoleMessageTypes.INFO);
            BOCSGameObject? targetObject = null; // Reset targetObject to null before searching
            // If we're in a sublocation, get the object directly
            if (player.CurrentSublocation != null)
            {
                targetObject = player.CurrentSublocation.FocusObject;
            }
            else // If we're in a regular location, output and return
            {
                IOService.Output.WriteNonDialogueLine("There's nothing to unlock here.");
                return;
            }

            foreach (var objectID in UnlockableObjectIDs)
            {
                if (targetObject != null && targetObject.GetAllBehaviours<IInteractable>().ToList().Any(s => s is LockUnlockBehaviour))
                {
                    var lockUnlockBehaviour = targetObject.GetAllBehaviours<IInteractable>().ToList().First(s => s.GetType() == typeof(LockUnlockBehaviour));
                    lockUnlockBehaviour.Interact(ObjectInteractionTypes.Unlock, player);
                    return;
                }
            }
            IOService.Output.WriteNonDialogueLine("There's nothing to unlock here.");
        }

        public override OnUseUnlockObjectBehaviour DeepClone()
        {
            return new OnUseUnlockObjectBehaviour(new List<string>(UnlockableObjectIDs), ConsumeOnUse);
        }
    }
}
