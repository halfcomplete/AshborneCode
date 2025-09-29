using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    public class OpenCloseBehaviour: IInteractable, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }

        public bool IsOpen { get; private set; } = false;

        public OpenCloseBehaviour(BOCSGameObject parentObject, bool isOpenInitially)
        {
            ParentObject = parentObject;
            IsOpen = isOpenInitially;
        }

        public async void Interact(ObjectInteractionTypes _interaction, Player player)
        {
            switch (_interaction)
            {
                case ObjectInteractionTypes.Open:
                    Open(player);
                    break;
                case ObjectInteractionTypes.Close:
                    Close(player);
                    break;
                default:
                    await IOService.Output.WriteNonDialogueLine("Invalid interaction type for OpenCloseBehaviour.");
                    break;
            }
        }

        private async void Open(Player player)
        {
            var behaviours = ParentObject.GetAllBehaviours<IInteractable>();

            if (behaviours.FirstOrDefault(s => s.GetType() == typeof(LockUnlockBehaviour)) is LockUnlockBehaviour lockBehaviour && lockBehaviour.IsLocked)
            {
                await IOService.Output.WriteNonDialogueLine($"The {ParentObject.Name} is locked. You need to unlock it first.");
                return;
            }

            if (IsOpen)
            {
                await IOService.Output.WriteNonDialogueLine($"The {ParentObject.Name} is already open.");
                return;
            }

            IsOpen = true;
            await IOService.Output.DisplayDebugMessage($"Behaviours available for {ParentObject.Name}: {string.Join(", ", behaviours.Select(b => b.GetType().Name))}", ConsoleMessageTypes.INFO);
            await IOService.Output.WriteNonDialogueLine($"You open the {ParentObject.Name}.");
            
            if (ParentObject.GetAllBehaviours<IHasInventory>().FirstOrDefault(s => s.GetType() == typeof(ContainerBehaviour)) is ContainerBehaviour containerBehaviour)
            {
                player.OpenedInventory = containerBehaviour.Inventory;
                (bool isEmpty, string contents) = await containerBehaviour.Inventory.GetInventoryContents(player: null);
                if (!isEmpty)
                {
                    await IOService.Output.WriteNonDialogueLine($"Inside the {ParentObject.Name} you see:");
                    await IOService.Output.WriteNonDialogueLine(contents);
                }
                else
                {
                    await IOService.Output.WriteNonDialogueLine($"The {ParentObject.Name} is empty.");
                }
            }
            (bool hasExitBehaviour, ExitToNewLocationBehaviour exitToNewLocationBehaviour) = await ParentObject.TryGetBehaviour<ExitToNewLocationBehaviour>();
            if (hasExitBehaviour && player.CurrentSublocation != null)
            {
                ILocation sublocation = player.CurrentSublocation;
                sublocation.AddExit("through", exitToNewLocationBehaviour.Location);
            }
        }

        private async void Close(Player player)
        {
            if (!IsOpen)
            {
                await IOService.Output.WriteNonDialogueLine($"The {ParentObject.Name} is already closed.");
                return;
            }
            
            IsOpen = false;
            player.OpenedInventory = null;
            await IOService.Output.WriteNonDialogueLine($"You close the {ParentObject.Name}.");
        }
    }
}
