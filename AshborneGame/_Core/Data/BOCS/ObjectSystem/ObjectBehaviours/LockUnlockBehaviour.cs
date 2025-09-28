using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    public class LockUnlockBehaviour : IInteractable, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; } = null!;
        public bool IsLocked { get; private set; } = false;

        public LockUnlockBehaviour(BOCSGameObject parentObject, bool initialState = false)
        {
            ParentObject = parentObject ?? throw new ArgumentNullException(nameof(parentObject));
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
                await IOService.Output.WriteNonDialogueLine($"The {ParentObject.Name} is already locked.");
                return;
            }

            IsLocked = true;
            await IOService.Output.WriteNonDialogueLine($"You lock the {ParentObject.Name}.");
        }

        private async void Unlock()
        {
            if (!IsLocked)
            {
                await IOService.Output.WriteNonDialogueLine($"The {ParentObject.Name} is already unlocked.");
                return;
            }
            IsLocked = false;
            await IOService.Output.WriteNonDialogueLine($"You unlock the {ParentObject.Name}.");
        }
    }
}
