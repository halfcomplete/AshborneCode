using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.CommandHandling
{
    public class CustomCommandHandler
    {
        private Dictionary<string, (Func<string> Message, Action Effect)> _commands = new();

        public CustomCommandHandler AddCustomCommand(CustomCommandPhrasing phrasing, Func<string> message, Action effect)
        {
            foreach (var phrase in phrasing.Phrases)
            {
                _commands.Add(phrase, (message, effect));
            }

            return this;
        }
        
        // TODO: add support for full command removal
        public void RemoveCommand(string command)
        {
            _commands.Remove(command);
        }
    }
}