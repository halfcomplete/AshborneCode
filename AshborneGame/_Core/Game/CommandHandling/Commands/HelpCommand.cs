using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class HelpCommand : ICommand
    {
        public string Name => "help";
        public string Description => "Provides information about possible commands.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count > 1)
            {
                IOService.Output.WriteLine("Did you mean just 'help'?");
                return false;
            }

            IOService.Output.WriteLine("Available commands:");
            for (int i = 0; i < CommandManager.Commands.Count; i++)
            {
                var command = CommandManager.Commands[CommandManager.Commands.Keys.ToList()[i]];
                IOService.Output.WriteLine($"{i + 1}. '{command.Name}' - {command.Description}");
            }

            return true;
        }
    }
}
