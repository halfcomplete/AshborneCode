using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Game.CommandHandling.Commands.BaseCommands;

namespace AshborneGame._Core.Game.CommandHandling.Commands.InventoryCommands
{
    internal class GiveCommand : BaseInventoryCommand, ICommand
    {
        public override List<string> Names => ["give"];
        public override string Description => "Takes something from the player and gives it to someone / something else.";

        public override async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Give what? Specify an item and optionally a quantity (e.g. 'give 3 gold coin').");
                return false;
            }

            Inventory originInventory = player.Inventory;
            Inventory? destinationInventory = await ResolveDestinationInventory(player);

            if (destinationInventory == null)
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

            if (quantity < 0)
            {
                if (string.IsNullOrEmpty(itemName))
                {
                    return await GiveAllItems(player, originInventory, destinationInventory);
                }
                else
                {
                    Item? targetItem = originInventory.GetItem(itemName);
                    if (targetItem == null)
                    {
                        await IOService.Output.DisplayFailMessage($"You cannot give {itemName} because it is not in your inventory.");
                        return false;
                    }
                    GiveAllOfAnItem(originInventory, destinationInventory, targetItem);
                    return true;
                }
            }

            if (string.IsNullOrEmpty(itemName))
            {
                await IOService.Output.DisplayFailMessage("Give what? Specify an item.");
                return false;
            }

            Item? item = originInventory.GetItem(itemName);
            if (item == null)
            {
                await IOService.Output.DisplayFailMessage($"You cannot give {itemName} because it is not in your inventory.");
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
                await IOService.Output.DisplayFailMessage($"You don't have enough {itemName} to give {quantity}.");
                return false;
            }

            originInventory.TransferItem(originInventory, destinationInventory, item, quantity);
            await IOService.Output.WriteNonDialogueLine($"Successfully gave {quantity} x {item.Name}.");

            ShowInventorySummary(player, player.Inventory, "Your inventory now contains:");
            ShowInventorySummary(player, destinationInventory, "The opened container / NPC now has:");

            return true;
        }

        private async Task<Inventory?> ResolveDestinationInventory(Player player)
        {
            if (player.OpenedInventory != null)
                return player.OpenedInventory;
            if (player.CurrentNPCInteraction == null) return null;
            (bool hasInventory, var npcInventory) = await player.CurrentNPCInteraction.TryGetBehaviour<IHasInventory>();
            if (hasInventory)
            {
                return npcInventory.Inventory;
            }

            return null;
        }

        private async Task<bool> GiveAllItems(Player player, Inventory origin, Inventory? destination)
        {
            if (destination == null)
            {
                await IOService.Output.DisplayFailMessage("There is no opened container or NPC to give items to.");
                return false;
            }

            origin.TransferAllItems(origin, destination);
            await IOService.Output.WriteNonDialogueLine("You gave all your items.");

            ShowInventorySummary(player, origin, "Your inventory is now empty.");
            ShowInventorySummary(player, destination, "The opened container / NPC now has:");

            return true;
        }

        private void GiveAllOfAnItem(Inventory origin, Inventory destination, Item item)
        {
            int count = origin.Slots.Where(s => s.Item.Name == item.Name).Sum(s => s.Quantity);
            origin.TransferItem(origin, destination, item, count);
        }
    }
}
