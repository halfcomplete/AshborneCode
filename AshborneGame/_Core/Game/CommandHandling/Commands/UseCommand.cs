using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class UseCommand : ICommand
    {
        public List<string> Names { get; } = ["use"];
        public string Description { get; } = "Use an item from your inventory.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Usage: use <item_name>");
                return false;
            }

            string itemName = string.Join(" ", args).Trim();
            Item? item = player.Inventory.GetItem(itemName);

            if (item == null)
            {
                await IOService.Output.DisplayFailMessage($"You do not have an item named '{itemName}' in your inventory.");
                return false;
            }

            if (!item.Behaviours.ContainsKey(typeof(IUsable)))
            {
                await IOService.Output.DisplayFailMessage($"The item '{item.Name}' cannot be used.");
                return false;
            }

            await IOService.Output.DisplayDebugMessage($"Parsed Input for 'use': {string.Join(" ", args)}", ConsoleMessageTypes.INFO); // Debugging output
            await IOService.Output.DisplayDebugMessage($"Using item: {item.Name}", ConsoleMessageTypes.INFO); // Debugging output

            IUsable usableItem = (IUsable)item.Behaviours[typeof(IUsable)][0];
            usableItem.Use(player);

            await IOService.Output.WriteNonDialogueLine($"You used {item.Name}.");

            return true;
        }
    }
}
