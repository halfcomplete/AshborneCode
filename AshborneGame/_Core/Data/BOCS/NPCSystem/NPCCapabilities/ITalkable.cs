using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities
{
    internal interface ITalkable
    {
        public string? Greeting { get; set; }
        public string? DialogueFileName { get; set; }

        virtual async Task Talk() { }
    }
}
