﻿using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class ShowStatsCommand : ICommand
    {
        public string Name => "stats";
        public string Description => "Shows your current stats.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count > 0)
            {
                IOService.Output.WriteLine("Did you mean \"stats\"?");
                return false;
            }

            IOService.Output.WriteLine("Your current stats:");
            IOService.Output.WriteLine(player.Stats.GetFormattedStats());

            return true;
        }
    }
}
