using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    internal class GoCommand : ICommand
    {
        public string Name => "go";
        public string Description => "Takes the player to a new location based on direction.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                IOService.Output.WriteLine("Go where? Specify a direction or location.");
                return false;
            }

            IOService.Output.DisplayDebugMessage($"Parsed Input for 'go': {string.Join(" ", args)}", ConsoleMessageTypes.INFO); // Debugging output

            return player.TryMoveTo(args);
        }
    }
}
