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
        // How much the character likes/dislikes the other character. 0 = hates, 0.5 = neutral, 1 = loves.
        private double _affection = 0.5;
        public double Affection { get => _affection; set => _affection = Math.Clamp(value, 0, 1); }

        // How much the character respects/disrespects the other character. 0 = disrespects, 0.5 = neutral, 1 = respects.
        private double _respect = 0.5;
        public double Respect { get => _respect; set => _respect = Math.Clamp(value, 0, 1); }

        // How much the character trusts/distrusts the other character. 0 = neutral, 0.5 = unsure, 1 = trusts.
        private double _trust = 0.5;
        public double Trust { get => _trust; set => _trust = Math.Clamp(value, 0, 1); }

        // How much the character fears the other character. 0 = neutral, 0.5 = unsure, 1 = extreme fear.
        private double _fear = 0.5;
        public double Fear { get => _fear; set => _fear = Math.Clamp(value, 0, 1); }

        // How dominant the character feels in relation to the other character. 0 = submissive, 0.5 = neutral, 1 = dominant.
        private double _dominance = 0.5;
        public double Dominance { get => _dominance; set => _dominance = Math.Clamp(value, 0, 1); }

        public Attitude() { }

        public Attitude(double affection, double respect, double trust, double fear, double dominance, double dependence, double envy, double curiosity)
        {
            Affection = affection;
            Respect = respect;
            Trust = trust;
            Fear = fear;
            Dominance = dominance;
        }
    }
}
