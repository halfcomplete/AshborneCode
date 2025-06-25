
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
                IOService.Output.WriteLine("Talk to whom? Specify a target.");
                return false;
            }
            string targetName = string.Join(" ", args).Trim();
            if (player.CurrentSublocation == null)
            {
                IOService.Output.WriteLine("You are not in a place where you can talk.");
                return false;
            }
            if (player.CurrentSublocation.Object is not NPC)
            {
                IOService.Output.WriteLine($"There is no one to talk to.");
                return false;
            }

            string characterName = string.Join(" ", args).Trim();
            Sublocation sublocation = player.CurrentSublocation!;
            NPC npc = (NPC)sublocation.Object;

            npc.Talk(player);
            return true;
        }
    }
}
