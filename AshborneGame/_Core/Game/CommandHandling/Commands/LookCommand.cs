
using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class LookCommand : ICommand
    {
        public List<string> Names => ["look"];
        public string Description => "Reprints your location and what you're currently doing.";

        public bool TryExecute(List<string> args, Player player)
        {
            IOService.Output.WriteNonDialogueLine(player.CurrentSublocation?.GetDescription(player, GameContext.GameState) ?? player.CurrentLocation.GetLookDescription(player, GameContext.GameState));
            return true;
        }
    }
}
