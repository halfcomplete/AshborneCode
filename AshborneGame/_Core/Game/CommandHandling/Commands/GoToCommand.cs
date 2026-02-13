using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    internal class GoToCommand : ICommand
    {
        public List<string> Names => ["go to"];
        public string Description => "Allows the player to travel to a location by name.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            await IOService.Output.DisplayDebugMessage($"Parsed Input for 'go to': {string.Join(" ", args)}", ConsoleMessageTypes.INFO); // Debugging output
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Go to what? Specify an object or location.");
                return false;
            }
            await IOService.Output.DisplayDebugMessage("Handling 'go to' command...", ConsoleMessageTypes.INFO);

            string place = string.Join(" ", args).Trim();
            await IOService.Output.DisplayDebugMessage($"Place to go to: {place}", ConsoleMessageTypes.INFO); // Debugging output
            return await player.TryMoveTo(args);
        }
    }
}
