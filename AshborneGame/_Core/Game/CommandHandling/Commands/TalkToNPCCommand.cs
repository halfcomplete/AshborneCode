
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class TalkToNPCCommand : ICommand
    {
        public List<string> Names => ["talk to"];
        public string Description => "Begins a conversation with an NPC.";

        public async Task<bool> TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                await IOService.Output.DisplayFailMessage("Talk to whom? Specify a target.");
                return false;
            }
            string targetName = string.Join(" ", args).Trim();

            Location loc = player.CurrentLocation;
            List<BOCSObject> possibleNPCs = loc.ContainedObjects.Where(o => o.HasBehaviours<ITalkable>() && o.Name.Matches(targetName)).ToList();

            // TODO: add "did you mean..." and "try moving closer..."
            if (possibleNPCs.Count() == 0)
            {
                await IOService.Output.WriteNonDialogueLine($"There is no-one named '{targetName}' here that you can talk to.");
            }
            else if (possibleNPCs.Count() > 1)
            {
                throw new InvalidOperationException($"There are multiple NPCs named {targetName} in {loc.Name} which can be talked to.");
            }

            // TODO: May cause an issue where this method returns true before the dialogue begins
            var res = possibleNPCs[0].TryGetBehaviour<TalkableBehaviour>().Result;

            if (res.Item1 && res.Item2 != null)
            {
                await res.Item2.Talk(player);
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
