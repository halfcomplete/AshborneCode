using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    internal class GoToCommand : ICommand
    {
        public string Name => "go to";
        public string Description => "Allows the player to travel to a location by name.";

        public bool TryExecute(List<string> args, Player player)
        {
            IOService.Output.DisplayDebugMessage($"Parsed Input for 'go to': {string.Join(" ", args)}", ConsoleMessageTypes.INFO); // Debugging output
            if (args.Count == 0)
            {
                IOService.Output.WriteLine("Go to what? Specify an object or location.");
                return false;
            }
            IOService.Output.DisplayDebugMessage("Handling 'go to' command...", ConsoleMessageTypes.INFO);

            string place = string.Join(" ", args).Trim();
            IOService.Output.DisplayDebugMessage($"Place to go to: {place}", ConsoleMessageTypes.INFO); // Debugging output
            return player.TryMoveTo(args);
        }
    }
}
