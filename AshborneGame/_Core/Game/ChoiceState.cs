using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game
{
    public enum ChoiceState
    {
        Hidden, // The player cannot select this choice because they don't know it exists (thus hide the choice altogether)
        Disabled, // The player cannot select this choice due to unmet conditions eg emotion constraints (thus grey the choice out and say why).
        Enabled // The choice is selectable and can be chosen by the player.
    }
}
