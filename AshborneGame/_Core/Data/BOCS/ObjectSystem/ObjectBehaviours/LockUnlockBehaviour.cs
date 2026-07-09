using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    public class LockUnlockBehaviour : Behaviour, IInteractable
    {
        public bool IsLocked { get; private set; } = false;

        public LockUnlockBehaviour(bool initialState = false)
        {
            IsLocked = initialState;
        }

        public async void Interact(ObjectInteractionTypes _interaction, Player player)
        {
            switch (_interaction)
            {
                case ObjectInteractionTypes.Lock:
                    Lock();
                    break;
                case ObjectInteractionTypes.Unlock:
                    Unlock();
                    break;
                default:
                    await IOService.Output.WriteNonDialogueLine("Invalid interaction type for LockUnlockBehaviour.");
                    break;
            }
        }

        private async void Lock()
        {
            if (IsLocked)
            {
                await IOService.Output.WriteNonDialogueLine($"The {Owner.Name} is already locked.");
                return;
            }

            IsLocked = true;
            await IOService.Output.WriteNonDialogueLine($"You lock the {Owner.Name}.");
        }

        private async void Unlock()
        {
            if (!IsLocked)
            {
                await IOService.Output.WriteNonDialogueLine($"The {Owner.Name} is already unlocked.");
                return;
            }
            IsLocked = false;
            await IOService.Output.WriteNonDialogueLine($"You unlock the {Owner.Name}.");
        }

        // TODO: rethink?
        public override LockUnlockBehaviour DeepClone()
        {
            return new LockUnlockBehaviour(IsLocked);
        }

        private record SaveData(bool IsLocked);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, System.Text.Json.JsonSerializer.SerializeToElement(new SaveData(IsLocked)));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("LockUnlockBehaviour save data is missing state.");
            }
            SaveData save = System.Text.Json.JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise LockUnlockBehaviour save data.");
            IsLocked = save.IsLocked;
        }
    }
}
