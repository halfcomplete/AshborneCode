﻿using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands.InventoryCommands
{
    public class ShowInventoryCommand : ICommand
    {
        public string Name => "inventory";
        public string Description => "Displays your current inventory.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count > 0)
            {
                IOService.Output.WriteLine("Did you mean \"inventory\"?");
                return false;
            }

            (bool isInventoryEmpty, string inventoryContents) = player.Inventory.GetInventoryContents(player);
            if (isInventoryEmpty)
            {
                IOService.Output.WriteLine("Your inventory is empty.");
            }
            else
            {
                IOService.Output.WriteLine("Your inventory contains:");
                IOService.Output.WriteLine(inventoryContents);
            }
            return true;
        }
    }
}
