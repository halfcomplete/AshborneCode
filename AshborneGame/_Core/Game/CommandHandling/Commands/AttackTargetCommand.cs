using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using System.Collections.Generic;
using AshborneGame._Core.Data.BOCS;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class AttackTargetCommand : ICommand
    {
        public List<string> Names => ["attack"];
        public string Description => "Attacks a target.";

        public async Task<bool> TryExecute(List<string> args, _Player.Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Attack what? Specify a target.");
                return true;
            }
            string targetName = string.Join(" ", args).Trim();
            Location location = player.CurrentLocation;
            // TODO: make cleaner with method on bocsobject
            BOCSObject? targetNPC = location.ContainedObjects.Where(o => o.IsNPC() && (o.Name == string.Join(" ", args) || o.Synonyms.Any(s => s == string.Join(" ", args)))).FirstOrDefault();
            if (targetNPC == null)
            {
                await IOService.Output.DisplayFailMessage($"You cannot attack that.");
                return true;
            }
            
            player.Attack(targetNPC);
            return true;
        }
    }
}
