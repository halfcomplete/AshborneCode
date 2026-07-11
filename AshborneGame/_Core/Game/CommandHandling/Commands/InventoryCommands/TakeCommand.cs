using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Game.CommandHandling.Commands.BaseCommands;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands.InventoryCommands
{
    internal class TakeCommand : BaseInventoryCommand, ICommand
    {
        public override List<string> Names => ["take"];
        public override string Description => "Takes an item from an object or NPC.";

        public override async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Give what? Specify an item and optionally a quantity (e.g. 'give 3 gold coin').");
                return false;
            }

            Inventory? originInventory = await ResolveSourceInventory(player);
            Inventory destinationInventory = player.Inventory;

            if (originInventory == null)
            {
                await IOService.Output.DisplayFailMessage("You are not targeting a container or an NPC with an inventory.");
                return false;
            }

            // Get the target quantity of the item to give. 0 = invalid, -1 = all
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
                    await IOService.Output.DisplayFailMessage("Invalid amount.");
                    return false;
                }
            }

            int count = originInventory.GetItemCount(itemName);
            if (count == 0)
            {
                await IOService.Output.DisplayFailMessage($"You cannot give {itemName} because it is not in your inventory.");
                return false;
            }

            if (quantity < 0)
            {
                if (string.IsNullOrEmpty(itemName))
                {
                    return await TakeAllItems(player, originInventory, destinationInventory);
                }
                else
                {
                    TakeAllOfAnItem(originInventory, destinationInventory, itemName);
                    return true;
                }
            }

            if (string.IsNullOrEmpty(itemName))
            {
                await IOService.Output.DisplayFailMessage("Give what? Specify an item.");
                return false;
            }

            // if we parsed 'all' as the quantity, we need to set it to the actual count of the item in the inventory
            if (quantity < 0)
            {
                quantity = count;
            }

            if (count < quantity)
            {
                await IOService.Output.DisplayFailMessage($"There are not enough '{itemName}' to take {quantity}.");
                return false;
            }

            originInventory.TransferItemsByName(originInventory, destinationInventory, itemName, quantity);
            await IOService.Output.WriteNonDialogueLine($"Successfully took {quantity} x {itemName}.");

            ShowInventorySummary(player, player.Inventory, "Your inventory now contains:");
            ShowInventorySummary(player, destinationInventory, "The opened container / NPC now has:");

            return true;
        }

        private async Task<Inventory?> ResolveSourceInventory(Player player)
        {
            if (player.OpenedInventory != null)
                return player.OpenedInventory;
            if (player.CurrentNPCInteraction == null) return null;
            (bool hasNPCInventory, var npcInventory) = await player.CurrentNPCInteraction.TryGetBehaviour<IHasInventory>();
            if (hasNPCInventory)
            {
                return npcInventory.Inventory;
            }

            return null;
        }

        private async Task<bool> TakeAllItems(Player player, Inventory origin, Inventory destination)
        {
            if (destination == null)
            {
                await IOService.Output.DisplayFailMessage("There is no opened container or NPC to give items to.");
                return false;
            }

            origin.TransferAllItems(destination, origin);
            await IOService.Output.WriteNonDialogueLine("You gave all your items.");

            ShowInventorySummary(player, destination, "The opened container / NPC now has:");
            ShowInventorySummary(player, origin, "Your inventory is now empty.");

            return true;
        }

        private void TakeAllOfAnItem(Inventory origin, Inventory destination, string itemName)
        {
            int count = origin.GetItemCount(itemName);
            if (count != 0)
            {
                origin.TransferItemsByName(origin, destination, itemName, count);
            }
        }
    }
}
