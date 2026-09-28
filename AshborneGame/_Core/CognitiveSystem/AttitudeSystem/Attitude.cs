using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.AttitudeSystem
{
    /// <summary>
    /// A class representing the attitude of a character towards another character.
    /// In the future this can be expanded to represent the attitude towards specific objects, events, or concepts, but for now it is focused on interpersonal relationships.
    /// </summary>
    /// <remarks>
    /// Attitude influences the final intensity of a Memory and the NPC's emotional interpretation of it.
    /// </remarks>
    public class Attitude
    {
        public Dictionary<AttitudeFactor, double> Factors { get; private set; } = new Dictionary<AttitudeFactor, double>
        {
            { AttitudeFactor.Affection, 0 },
            { AttitudeFactor.Respect, 0 },
            { AttitudeFactor.Trust, 0 },
            { AttitudeFactor.Fear, 0 },
            { AttitudeFactor.Dominance, 0 },
        };

        public Attitude() { }

        public Attitude(double affection, double respect, double trust, double fear, double dominance, double dependence, double envy, double curiosity)
        {
            foreach (var factor in Enum.GetValues(typeof(AttitudeFactor)).Cast<AttitudeFactor>())
            {
                Factors[factor] = factor switch
                {
                    AttitudeFactor.Affection => Math.Clamp(affection, 0, 1),
                    AttitudeFactor.Respect => Math.Clamp(respect, 0, 1),
                    AttitudeFactor.Trust => Math.Clamp(trust, 0, 1),
                    AttitudeFactor.Fear => Math.Clamp(fear, 0, 1),
                    AttitudeFactor.Dominance => Math.Clamp(dominance, 0, 1),
                    _ => throw new ArgumentOutOfRangeException()
                };
            }
        }
    }
}
