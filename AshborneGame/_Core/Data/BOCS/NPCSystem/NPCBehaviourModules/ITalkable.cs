using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules
{
    internal interface ITalkable
    {
        public string? Greeting { get; }
        public string? DialogueFileName { get; init; }

        virtual async Task Talk() { }
    }
}
