using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.CommandHandling
{
    public record CustomCommand(string Command, Func<string> Message, Action Effect);
}