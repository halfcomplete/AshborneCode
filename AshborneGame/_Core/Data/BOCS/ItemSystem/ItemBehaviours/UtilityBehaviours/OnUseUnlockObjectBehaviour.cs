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
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

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


        private record SaveData(List<DefinitionID> UnlockableObjectIDs, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, System.Text.Json.JsonSerializer.SerializeToElement(new SaveData(UnlockableObjectIDs, ConsumeOnUse)));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnUseUnlockObjectBehaviour save data is missing state.");
            }
            SaveData save = System.Text.Json.JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnUseUnlockObjectBehaviour save data.");
            UnlockableObjectIDs = save.UnlockableObjectIDs;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
