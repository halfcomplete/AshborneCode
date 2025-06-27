using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.CommandHandling
{
    internal static class CommandCatchers
    {
        public static readonly List<string> ShoutVerbs = ["shout", "cry", "call", "scream", "yell"];
        public static readonly List<string> HelpVerbs = ["pray", "plead", "beg"];
    }
}
