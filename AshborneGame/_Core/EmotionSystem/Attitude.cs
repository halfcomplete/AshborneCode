using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A class representing the attitude of a character towards another character.
    /// In the future this can be expanded to represent the attitude towards specific objects, events, or concepts, but for now it is focused on interpersonal relationships.
    /// </summary>
    public class Attitude
    {
        // How much the character likes/dislikes the other character. -1 = hates, 0 = neutral, 1 = loves.
        private float _affection = 0;
        public float Affection { get => _affection; set => _affection = Math.Clamp(value, -1f, 1f); }

        // How much the character respects/disrespects the other character. -1 = disrespects, 0 = neutral, 1 = respects.
        private float _respect = 0;
        public float Respect { get => _respect; set => _respect = Math.Clamp(value, -1f, 1f); }

        // How much the character trusts/distrusts the other character. -1 = distrusts, 0 = neutral, 1 = trusts.
        private float _trust = 0;
        public float Trust { get => _trust; set => _trust = Math.Clamp(value, -1f, 1f); }

        // How much the character fears the other character. 0 = no fear, 1 = extreme fear.
        private float _fear = 0;
        public float Fear { get => _fear; set => _fear = Math.Clamp(value, 0f, 1f); }

        // How dominant the character feels in relation to the other character. -1 = submissive, 0 = neutral, 1 = dominant.
        private float _dominance = 0;
        public float Dominance { get => _dominance; set => _dominance = Math.Clamp(value, -1f, 1f); }

        // How much the character depends on the other character. 0 = independent, 1 = completely dependent.
        private float _dependence = 0;
        public float Dependence { get => _dependence; set => _dependence = Math.Clamp(value, 0f, 1f); }

        // How much the character is loyal to the other character. 0 = disloyal, 1 = completely loyal.
        private float _loyalty = 0;
        public float Loyalty { get => _loyalty; set => _loyalty = Math.Clamp(value, 0f, 1f); }

        // How much the character envies the other character. 0 = no envy, 1 = extreme envy.
        private float _envy = 0;
        public float Envy { get => _envy; set => _envy = Math.Clamp(value, 0f, 1f); }

        // How much the character is curious about the other character. 0 = not curious, 1 = extremely curious.
        private float _curiosity = 0;
        public float Curiosity { get => _curiosity; set => _curiosity = Math.Clamp(value, 0f, 1f); }

        public Attitude() { }

        public Attitude(float affection, float respect, float trust, float fear, float dominance, float dependence, float loyalty, float envy, float curiosity)
        {
            Affection = affection;
            Respect = respect;
            Trust = trust;
            Fear = fear;
            Dominance = dominance;
            Dependence = dependence;
            Loyalty = loyalty;
            Envy = envy;
            Curiosity = curiosity;
        }
    }
}
