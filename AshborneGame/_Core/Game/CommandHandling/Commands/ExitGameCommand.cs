using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    internal class ExitGameCommand : ICommand
    {
        public List<string> Names => ["exit"];
        public string Description => "Exits the game.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count > 0)
            {
                await IOService.Output.DisplayFailMessage("Did you mean \"exit\"?");
                return false;
            }

            await IOService.Output.WriteNonDialogueLine("Thank you for playing Ashborne!");
            Thread.Sleep(1000);
            Environment.Exit(0);

            return true;
        }
    }
}
