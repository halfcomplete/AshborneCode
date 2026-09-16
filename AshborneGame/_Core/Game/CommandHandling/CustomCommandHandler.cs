using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.CommandHandling
{
    // TODO: add support for checking how many times a command has been used, and limit it to a certain number of uses or change the effect of the command based on how many times it has been used
    public class CustomCommandHandler
    {
        private Dictionary<string, Action> _commands = new();

        public CustomCommandHandler AddCustomCommand(CustomCommandPhrasing phrasing, Action effect)
        {
            foreach (var phrase in phrasing.Phrases)
            {
                _commands.Add(phrase, effect);
            }

            return this;
        }

        public CustomCommandHandler AddCustomCommand(string command, Action effect)
        {
            _commands.Add(command, effect);
            return this;
        }

        // TODO: add support for full command removal
        public void RemoveCommand(string command)
        {
            _commands.Remove(command);
        }

        public async Task<bool> CheckForMatch(string action, List<string> args)
        {
            foreach (var kvp in _commands)
            {
                var args2 = new List<string>(args);
                args2.Insert(0, action);
                if (string.Join(' ', args2).Equals(kvp.Key, StringComparison.OrdinalIgnoreCase))
                {
                    kvp.Value.Effect?.Invoke();
                    return true;
                }
            }

            return false;
        }

        public Dictionary<string, Action> GetCommands()
        {
            return new(_commands);
        }
    }
}