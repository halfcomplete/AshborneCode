
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.Behaviours;
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
            if (!player.CurrentSublocation.FocusObject.IsNPC())
            {
                await IOService.Output.DisplayFailMessage($"There is no one to talk to.");
                return false;
            }

            Sublocation sublocation = player.CurrentSublocation!;
            BOCSObject npc = sublocation.FocusObject;

            // Check if the NPC's name or synonyms match the target name
            // TODO: add new helper method for objects to check for name similarity
            if (!npc.Synonyms.Contains(targetName))
            {
                await IOService.Output.DisplayFailMessage($"There is no one named '{targetName}' here.");
                return false;
            }

            // TODO: May cause an issue where this method returns true before the dialogue begins

            var res = npc.TryGetBehaviour<TalkableBehaviour>().Result;

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
