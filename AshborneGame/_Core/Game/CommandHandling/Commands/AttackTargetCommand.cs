using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using System.Collections.Generic;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class AttackTargetCommand : ICommand
    {
        public string Name => "attack";
        public string Description => "Attacks a target.";

        public bool TryExecute(List<string> args, _Player.Player player)
        {
            if (args.Count == 0)
            {
                IOService.Output.WriteLine("Attack what? Specify a target.");
                return true;
            }
            string targetName = string.Join(" ", args).Trim();
            Sublocation? sublocation = player.CurrentSublocation;
            if (sublocation == null)
            {
                IOService.Output.WriteLine("There's nothing to attack here. You might want to get closer.");
                return true;
            }
            NPC? targetNPC = sublocation.Object as NPC;
            if (targetNPC == null)
            {
                IOService.Output.WriteLine($"You cannot attack that.");
                return true;
            }
            player.Attack(targetNPC);
            return true;
        }
    }
}
