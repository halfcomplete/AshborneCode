
using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using System.Text;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class LookCommand : ICommand
    {
        public List<string> Names => ["look"];
        public string Description => "Reprints your location and what you're currently doing.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            StringBuilder sb = new StringBuilder();
            sb.AppendLine(player.CurrentSublocation?.GetDescription(player, GameContext.GameState) ?? player.CurrentLocation.GetLookDescription(player, GameContext.GameState));
            sb.AppendLine(player.CurrentSublocation?.GetExits() ?? player.CurrentLocation.GetExits());
            await IOService.Output.WriteNonDialogueLine(sb.ToString());
            return true;
        }
    }
}
