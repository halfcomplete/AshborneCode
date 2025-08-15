using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Game.CommandHandling.Commands.BaseCommands;

namespace AshborneGame._Core.Game.CommandHandling.Commands.InventoryCommands
{
    internal class TakeCommand : BaseInventoryCommand, ICommand
    {
        public override List<string> Names => ["take"];
        public override string Description => "Takes an item from an object or NPC.";

        public override bool TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                IOService.Output.DisplayFailMessage("Take what? Specify an item and optionally a quantity (e.g. 'take 2 gold coin').");
                return false;
            }

            
            Inventory? originInventory = ResolveSourceInventory(player);
            Inventory destinationInventory = player.Inventory;

            if (originInventory == null)
            {
                IOService.Output.DisplayFailMessage("There is no open container or NPC inventory to take items from.");
                return false;
            }

            int quantity = ParseQuantity(ref args);
            string itemName = string.Join(" ", args).Trim();

            if (quantity == 0)
            {
                if (originInventory.GetItem(itemName) != null)
                {
                    quantity = 1;
                }
                else
                {
                    IOService.Output.DisplayFailMessage("Invalid amount.");
                    return false;
                }
                
            }

            if (quantity < 0)
            {
                if (string.IsNullOrEmpty(itemName))
                {
                    TakeAllItems(player, originInventory, destinationInventory);
                }
                else
                {
                    
                    Item? targetItem = originInventory.GetItem(itemName);
                    if (targetItem == null)
                    {
                        IOService.Output.DisplayFailMessage($"You cannot take {itemName} because it is not there.");
                        return false;
                    }

                    TakeAllOfAnItem(originInventory, destinationInventory, targetItem);
                }

                return true;
            }

            if (string.IsNullOrEmpty(itemName))
            {
                IOService.Output.DisplayFailMessage("Take what? Specify an item.");
                return false;
            }

            Item? item = originInventory.GetItem(itemName);
            if (item == null)
            {
                IOService.Output.DisplayFailMessage($"You cannot take {itemName} because it is not there.");
                return false;
            }

            int availableCount = originInventory.Slots
                .Where(slot => slot.Item.Name == item.Name)
                .Sum(slot => slot.Quantity);

            if (quantity < 0)
            {
                quantity = availableCount;
            }

            if (availableCount < quantity)
            {
                IOService.Output.DisplayFailMessage($"There are not enough {itemName} to take {quantity}.");
                return false;
            }

            originInventory.TransferItem(originInventory, destinationInventory, item, quantity);
            IOService.Output.WriteLine($"Successfully took {quantity} x {item.Name}.");

            ShowInventorySummary(player, player.Inventory, "Your inventory now contains:");
            ShowInventorySummary(player, originInventory, "The container / NPC now has:");

            return true;
        }

        private Inventory? ResolveSourceInventory(Player player)
        {
            if (player.OpenedInventory != null)
                return player.OpenedInventory;

            if (player.CurrentNPCInteraction != null &&
                player.CurrentNPCInteraction.TryGetBehaviour<IHasInventory>(out var npcInventory))
            {
                return npcInventory.Inventory;
            }

            return null;
        }

        private void TakeAllItems(Player player, Inventory origin, Inventory destination)
        {
            origin.TransferAllItems(origin, destination);
            IOService.Output.WriteLine("You took all available items.");

            ShowInventorySummary(player, destination, "Your inventory now contains:");
            ShowInventorySummary(player, origin, "The container / NPC now has:");
        }

        private void TakeAllOfAnItem(Inventory origin, Inventory destination, Item item)
        {
            int count = origin.Slots.Where(s => s.Item.Name == item.Name).Sum(s => s.Quantity);
            origin.TransferItem(origin, destination, item, count);
        }
    }
}
