
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem;
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
            if (player.CurrentSublocation == null)
            {
                await IOService.Output.DisplayFailMessage("You are not in a place where you can talk.");
                return false;
            }
            if (player.CurrentSublocation.FocusObject is not NPC)
            {
                await IOService.Output.DisplayFailMessage($"There is no one to talk to.");
                return false;
            }

            Sublocation sublocation = player.CurrentSublocation!;
            NPC npc = (NPC)sublocation.FocusObject;

            // Check if the NPC's name or synonyms match the target name
            if (!npc.MatchesName(targetName))
            {
                await IOService.Output.DisplayFailMessage($"There is no one named '{targetName}' here.");
                return false;
            }

            // May cause an issue where this method returns true before the dialogue begins
            await npc.Talk(player);
            return true;
        }
    }
}
