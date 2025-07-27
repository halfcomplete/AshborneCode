
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;

namespace AshborneGame._Core.Game.CommandHandling.Commands
{
    public class TalkToNPCCommand : ICommand
    {
        public string Name => "talk to";
        public string Description => "Begins a conversation with an NPC.";

        public bool TryExecute(List<string> args, Player player)
        {
            if (args.Count == 0)
            {
                IOService.Output.DisplayFailMessage("Talk to whom? Specify a target.");
                return false;
            }
            string targetName = string.Join(" ", args).Trim();
            if (player.CurrentSublocation == null)
            {
                IOService.Output.DisplayFailMessage("You are not in a place where you can talk.");
                return false;
            }
            if (player.CurrentSublocation.FocusObject is not NPC)
            {
                IOService.Output.DisplayFailMessage($"There is no one to talk to.");
                return false;
            }

            Sublocation sublocation = player.CurrentSublocation!;
            NPC npc = (NPC)sublocation.FocusObject;

            // Check if the NPC's name matches the target name (case-insensitive, partial match)
            if (!npc.Name.ToLowerInvariant().Contains(targetName.ToLowerInvariant()))
            {
                IOService.Output.DisplayFailMessage($"There is no one named '{targetName}' here.");
                return false;
            }

            // May cause an issue where this method returns true before the dialogue begins
            npc.Talk(player);
            return true;
        }
    }
}
