using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours
{
    public class OnUseUnlockObjectBehaviour : Behaviour, IActOnUse, IUnlocksTarget
    {
        public bool ConsumeOnUse { get; set; }

        public List<DefinitionID> UnlockableObjectIDs { get; private set; }

        public OnUseUnlockObjectBehaviour(List<DefinitionID> unlockableObjectIDs, bool consumeOnUse = true)
        {
            UnlockableObjectIDs = unlockableObjectIDs ?? throw new ArgumentNullException(nameof(unlockableObjectIDs), "Unlockable object IDs cannot be null.");
            ConsumeOnUse = consumeOnUse;
        }

        public async void OnUse(Player player)
        {
            await IOService.Output.DisplayDebugMessage("On Use Trigger successfully called for OnUseUnlockObjectBehaviour", ConsoleMessageTypes.INFO);
            var targetObjects = player.CurrentLocation.ContainedObjects;

            foreach (var obj in targetObjects)
            {
                var bs = obj.GetAllBehaviours<IInteractable>().ToList();
                foreach (var b in bs)
                {
                    // TODO: bad practice?
                    if (b is LockUnlockBehaviour l && UnlockableObjectIDs.Contains(obj.DefinitionID))
                    {
                        l.Interact(ObjectInteractionTypes.Unlock, player);
                        return;
                    }
                }
            }
            await IOService.Output.WriteNonDialogueLine("There's nothing to unlock here.");
        }

        public override OnUseUnlockObjectBehaviour DeepClone()
        {
            return new OnUseUnlockObjectBehaviour([.. UnlockableObjectIDs], ConsumeOnUse);
        }
    }
}
