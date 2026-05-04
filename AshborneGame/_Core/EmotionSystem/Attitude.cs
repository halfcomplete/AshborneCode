using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A struct representing the attitude of a character towards another character.
    /// In the future this can be expanded to represent the attitude towards specific objects, events, or concepts, but for now it is focused on interpersonal relationships.
    /// </summary>
    public struct Attitude
    {
        public float Trust = 0;
        public float Affection = 0;
        public float Resentment = 0;
        public float Fear = 0;
        public float Respect = 0;

        public Attitude() { }
    }
}
