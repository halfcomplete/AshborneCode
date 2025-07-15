using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules.MaskBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.MaskBehaviours;
using AshborneGame._Core.Game.CommandHandling.Commands;
using AshborneGame._Core.Game.CommandHandling.Commands.InventoryCommands;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game.CommandHandling
{
    public static class CommandManager
    {
        public static IReadOnlyDictionary<string, ICommand> Commands => _commands;
        private static Dictionary<string, ICommand> _commands = new();

        static CommandManager()
        {
            RegisterCommand(new UseCommand());
            RegisterCommand(new TalkToNPCCommand());
            RegisterCommand(new ShowStatsCommand());
            RegisterCommand(new ShowInventoryCommand());
            RegisterCommand(new OpenObjectCommand());
            RegisterCommand(new CloseObjectCommand());
            RegisterCommand(new ShowStatsCommand());
            RegisterCommand(new ExitGameCommand());
            RegisterCommand(new AttackTargetCommand());
            RegisterCommand(new GiveCommand());
            RegisterCommand(new TakeCommand());
            RegisterCommand(new LookCommand());
            RegisterCommand(new GoCommand());
            RegisterCommand(new GoToCommand());
            RegisterCommand(new HelpCommand());
        }

        public static void RegisterCommand(ICommand command)
        {
            _commands[command.Name.ToLower()] = command;
        }

        public static bool TryExecute(string action, List<string> args, Player player)
        {
            // Check sublocation custom commands first
            if (player.CurrentSublocation != null)
            {
                foreach (var kvp in player.CurrentSublocation.CustomCommands)
                {
                    var args2 = new List<string>(args);
                    args2.Insert(0, action);
                    if (string.Join(' ', args2).Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                    {
                        IOService.Output.WriteLine(kvp.Value.message.Invoke());
                        kvp.Value.effect?.Invoke();
                        return true;
                    }
                }
            }

            // Then check location custom commands
            foreach (var kvp in player.CurrentLocation.CustomCommands)
            {
                var args2 = new List<string>(args);
                args2.Insert(0, action);
                if (string.Join(' ', args2).Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    IOService.Output.WriteLine(kvp.Value.message.Invoke());
                    kvp.Value.effect?.Invoke();
                    return true;
                }
            }

            if (CheckIfCaughtByCommandBuckets(player, action, out string message))
            {
                IOService.Output.WriteLine(message, ms: 50);
                return true;
            }

            if (_commands.TryGetValue(action.ToLower(), out var command) && command.TryExecute(args, player))
            {
                return true;
            }

            return false;
        }

        public static string ExtractAction(List<string> input, out List<string> args)
        {
            if (input.Count >= 2 && (input[0] == "go" || input[0] == "talk") && input[1] == "to")
            {
                var copy = new List<string>(input);
                args = new List<string>(input);
                args.RemoveRange(0, 2);
                return string.Join(' ', copy.GetRange(0, 2));
            }
            args = new List<string>(input);
            args.RemoveAt(0);
            return input[0];
        }

        private static bool CheckIfCaughtByCommandBuckets(Player player, string action, out string message)
        {
            message = string.Empty;
            Item? currentMask = player.EquippedItems["face"];
            string currentMaskName = currentMask != null ? currentMask.Name : string.Empty;
            if (CommandCatchers.ShoutVerbs.Contains(action))
            {
                message = "You try to shout, yell, scream, but nothing comes out.";
            }
            else if (CommandCatchers.HelpVerbs.Contains(action))
            {
                if (!GameContext.GameState.TryIncrementCounter("player.prayers"))
                {
                    GameContext.GameState.SetCounter("player.prayers", 1);
                }
                GameEvent playerPrayedEvent = new GameEvent("player.actions.prayed", new Dictionary<string, object>()
                {
                    {"location_group", "Ossaneth's Domain"}
                });
                EventBus.Call(playerPrayedEvent);
            }
            else
            {
                return false;
            }

            return true;
        }
    }
}
