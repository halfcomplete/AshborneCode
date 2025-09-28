using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class ShowStatsCommand : ICommand
    {
        public List<string> Names => ["stats"];
        public string Description => "Shows your current stats.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count > 0)
            {
                await IOService.Output.DisplayFailMessage("Did you mean \"stats\"?");
                return false;
            }

            await IOService.Output.WriteNonDialogueLine("Your current stats:");
            await IOService.Output.WriteNonDialogueLine(player.Stats.GetFormattedStats());

            return true;
        }
    }
}
