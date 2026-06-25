using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.MemorySystem
{
    /// <summary>
    /// An enum for the different emotional archetypes that a Memory may have. Used for similarity checks and calculating the emotional impact of a Memory.
    /// </summary>
    /// <remarks>
    /// Archetypes also define emotional tendencies.
    /// </remarks>
    public enum MemoryTag
    {
        Kindness,
        Betrayal,
        Theft,
        Violence,
        Sacrifice,
        Generosity,
        Protection,
        Humiliation,
        Deception,
        Curiosity
    }
}
