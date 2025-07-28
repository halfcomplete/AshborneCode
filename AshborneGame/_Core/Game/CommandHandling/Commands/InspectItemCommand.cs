using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    internal class InspectItemCommand : ICommand
    {
        public List<string> Names => ["inspect"];
        public string Description => "Inspect an an in-game item for details.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                IOService.Output.DisplayFailMessage("Inspect what? Specify an item.");
                return false;
            }

            string itemName = string.Join(" ", args).Trim();
            Item? item = player.Inventory.GetItem(itemName);
            if (item != null)
            {
                if (!item.TryGetBehaviour<IInspectable>(out var inspectableBehaviour))
                {
                    IOService.Output.DisplayFailMessage($"That can't be inspected.");
                    return false;
                }
                IOService.Output.WriteLine($"Inspecting {item.Name}: {item.Description}");
                inspectableBehaviour.Inspect();
                return true;
            }
            else
            {
                IOService.Output.DisplayFailMessage($"You don't have a {itemName} to inspect.");
                return false;
            }
        }
    }
}
