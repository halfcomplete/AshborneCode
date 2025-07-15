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

        public void Interact(ObjectInteractionTypes _interaction, Player player)
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
                    IOService.Output.WriteLine("Invalid interaction type for OpenCloseBehaviour.");
                    break;
            }
        }

        private void Open(Player player)
        {
            var behaviours = ParentObject.GetAllBehaviours<IInteractable>();

            if (behaviours.FirstOrDefault(s => s.GetType() == typeof(LockUnlockBehaviour)) is LockUnlockBehaviour lockBehaviour && lockBehaviour.IsLocked)
            {
                IOService.Output.WriteLine($"The {ParentObject.Name} is locked. You need to unlock it first.");
                return;
            }

            if (IsOpen)
            {
                IOService.Output.WriteLine($"The {ParentObject.Name} is already open.");
                return;
            }

            IsOpen = true;
            IOService.Output.DisplayDebugMessage($"Behaviours available for {ParentObject.Name}: {string.Join(", ", behaviours.Select(b => b.GetType().Name))}", ConsoleMessageTypes.INFO);
            IOService.Output.WriteLine($"You open the {ParentObject.Name}.");
            
            if (ParentObject.GetAllBehaviours<IHasInventory>().FirstOrDefault(s => s.GetType() == typeof(ContainerBehaviour)) is ContainerBehaviour containerBehaviour)
            {
                player.OpenedInventory = containerBehaviour.Inventory;
                (bool isEmpty, string contents) = containerBehaviour.Inventory.GetInventoryContents(player: null);
                if (!isEmpty)
                {
                    IOService.Output.WriteLine($"Inside the {ParentObject.Name} you see:");
                    IOService.Output.WriteLine(contents);
                }
                else
                {
                    IOService.Output.WriteLine($"The {ParentObject.Name} is empty.");
                }
            }

            if (ParentObject.TryGetBehaviour<ExitToNewLocationBehaviour>(out var exitToNewLocationBehaviour) && player.CurrentSublocation != null)
            {
                ILocation sublocation = player.CurrentSublocation;
                sublocation.AddExit("through", exitToNewLocationBehaviour.Location);
            }
        }

        private void Close(Player player)
        {
            if (!IsOpen)
            {
                IOService.Output.WriteLine($"The {ParentObject.Name} is already closed.");
                return;
            }
            
            IsOpen = false;
            player.OpenedInventory = null;
            IOService.Output.WriteLine($"You close the {ParentObject.Name}.");
        }
    }
}
