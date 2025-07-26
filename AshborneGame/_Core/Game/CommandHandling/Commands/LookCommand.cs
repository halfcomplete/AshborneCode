
using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class LookCommand : ICommand
    {
        public string Name => "look";
        public string Description => "Reprints your location and what you're currently doing.";

        public bool TryExecute(List<string> args, Player player)
        {
            IOService.Output.WriteLine(player.CurrentSublocation?.GetDescription(player, GameContext.GameState) ?? player.CurrentLocation.GetDescription(player, GameContext.GameState));
            return true;
        }
    }
}
