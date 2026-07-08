using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    public class OpenCloseBehaviour: Behaviour, IInteractable
    {
        public bool IsOpen { get; private set; } = false;

        public OpenCloseBehaviour(bool isOpenInitially)
        {
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
            var behaviours = Owner.GetAllBehaviours<IInteractable>();

            if (behaviours.FirstOrDefault(s => s.GetType() == typeof(LockUnlockBehaviour)) is LockUnlockBehaviour lockBehaviour && lockBehaviour.IsLocked)
            {
                await IOService.Output.WriteNonDialogueLine($"The {Owner.Name} is locked. You need to unlock it first.");
                return;
            }

            if (IsOpen)
            {
                await IOService.Output.WriteNonDialogueLine($"The {Owner.Name} is already open.");
                return;
            }

            IsOpen = true;
            await IOService.Output.DisplayDebugMessage($"Behaviours available for {Owner.Name}: {string.Join(", ", behaviours.Select(b => b.GetType().Name))}", ConsoleMessageTypes.INFO);
            await IOService.Output.WriteNonDialogueLine($"You open the {Owner.Name}.");
            
            if (Owner.GetAllBehaviours<IHasInventory>().FirstOrDefault(s => s.GetType() == typeof(ContainerBehaviour)) is ContainerBehaviour containerBehaviour)
            {
                player.OpenedInventory = containerBehaviour.Inventory;
                (bool isEmpty, string contents) = await containerBehaviour.Inventory.GetInventoryContents(player: null);
                if (!isEmpty)
                {
                    await IOService.Output.WriteNonDialogueLine($"Inside the {Owner.Name} you see:");
                    await IOService.Output.WriteNonDialogueLine(contents);
                }
                else
                {
                    await IOService.Output.WriteNonDialogueLine($"The {Owner.Name} is empty.");
                }
            }
            (bool hasExitBehaviour, ExitToNewLocationBehaviour exitToNewLocationBehaviour) = await Owner.TryGetBehaviour<ExitToNewLocationBehaviour>();
            if (hasExitBehaviour && exitToNewLocationBehaviour != null && player.CurrentLocation != null)
            {
                Location location = player.CurrentLocation;
                location.AddExit(new(exitToNewLocationBehaviour.Location.DefinitionID, "through"));
            }
        }

        private async void Close(Player player)
        {
            if (!IsOpen)
            {
                await IOService.Output.WriteNonDialogueLine($"The {Owner.Name} is already closed.");
                return;
            }
            
            IsOpen = false;
            player.OpenedInventory = null;
            await IOService.Output.WriteNonDialogueLine($"You close the {Owner.Name}.");
        }

        // TODO: rethink this?
        public override OpenCloseBehaviour DeepClone()
        {
            return new OpenCloseBehaviour(IsOpen);
        }
    }
}
