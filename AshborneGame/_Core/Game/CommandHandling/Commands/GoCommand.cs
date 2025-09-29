using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    internal class GoCommand : ICommand
    {
        public List<string> Names => ["go"];
        public string Description => "Takes the player to a new location based on direction.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Go where? Specify a direction or location.");
                return false;
            }

            await IOService.Output.DisplayDebugMessage($"Parsed Input for 'go': {string.Join(" ", args)}", ConsoleMessageTypes.INFO); // Debugging output

            return await player.TryMoveTo(args);
        }
    }
}
