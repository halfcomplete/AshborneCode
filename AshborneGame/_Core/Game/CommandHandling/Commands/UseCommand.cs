using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class UseCommand : ICommand
    {
        public List<string> Names { get; } = ["use"];
        public string Description { get; } = "Use an item from your inventory.";

        // TODO: remove async
        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Usage: use <item_name>");
                return false;
            }

            string objectName = string.Join(" ", args).Trim();
            BOCSObject? obj = player.Inventory.GetItem(objectName);

            if (obj == null)
            {
                await IOService.Output.DisplayFailMessage($"You do not have an object named '{objectName}' in your inventory.");
                return false;
            }

            if (!obj.ByModule.ContainsKey(typeof(IUsable)))
            {
                await IOService.Output.DisplayFailMessage($"The object '{obj.Name}' cannot be used.");
                return false;
            }

            await IOService.Output.DisplayDebugMessage($"Parsed Input for 'use': {string.Join(" ", args)}", ConsoleMessageTypes.INFO); // Debugging output
            await IOService.Output.DisplayDebugMessage($"Using item: {obj.Name}", ConsoleMessageTypes.INFO); // Debugging output

            List<IUsable> usable = obj.GetAllBehavioursOfType<IUsable>().ToList();
            foreach (var u in usable)
            {
                u.Use(player);
            }
            

            await IOService.Output.WriteNonDialogueLine($"You used {obj.Name}.");

            return true;
        }
    }
}
