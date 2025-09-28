using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands.InventoryCommands
{
    public class ShowInventoryCommand : ICommand
    {
        public List<string> Names => ["inventory"];
        public string Description => "Displays your current inventory.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count > 0)
            {
                await IOService.Output.DisplayFailMessage("Did you mean \"inventory\"?");
                return false;
            }

            (bool isInventoryEmpty, string inventoryContents) = player.Inventory.GetInventoryContents(player);
            if (isInventoryEmpty)
            {
                await IOService.Output.WriteNonDialogueLine("Your inventory is empty.");
            }
            else
            {
                await IOService.Output.WriteNonDialogueLine("Your inventory contains:");
                await IOService.Output.WriteNonDialogueLine(inventoryContents);
            }
            return true;
        }
    }
}
