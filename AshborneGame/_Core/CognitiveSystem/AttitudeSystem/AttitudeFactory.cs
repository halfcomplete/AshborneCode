using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem.AttitudeSystem
{
    // TODO: model expectation violations; a hated NPC saving your life will have a much bigger impact on your attitude of them
    public static class AttitudeFactory
    {
        public static Attitude CreateAttitude(List<AttitudeReaction> attitudeReactions)
        {
            return ModifyAttitude(new(), attitudeReactions);
        }

        public static Attitude ModifyAttitude(Attitude attitude, List<AttitudeReaction> attitudeReactions)
        {
            foreach (var reaction in attitudeReactions)
            {
                switch (reaction.AttitudeFactor)
                {
                    case AttitudeFactor.Affection:
                        attitude.Affection = ModifyValue(attitude.Affection, reaction.Add);
                        break;

                    case AttitudeFactor.Respect:
                        attitude.Respect = ModifyValue(attitude.Respect, reaction.Add);
                        break;

                    case AttitudeFactor.Trust:
                        attitude.Trust = ModifyValue(attitude.Trust, reaction.Add);
                        break;

                    case AttitudeFactor.Fear:
                        attitude.Fear = ModifyValue(attitude.Fear, reaction.Add);
                        break;

                    case AttitudeFactor.Dominance:
                        attitude.Dominance = ModifyValue(attitude.Dominance, reaction.Add);
                        break;
                }
            }

            return attitude;
        }

        private static double ModifyValue(double current, double delta)
        {
            // Resistance increases nonlinearly as attitudes become stronger
            double openness = Math.Pow(1.15 - Math.Abs(current), 2.0);

            // -1 = strongly contradictory, 0 = neutral, 1 = strongly reinforcing
            double alignment = 1 - (Math.Abs(current - delta) / 2.0);

            double x = 4.0 * alignment; // controls steepness
            double agreementMultiplier = 0.2 + 0.8 / (1.0 + Math.Exp(-x)); // 0.2 floor ensures contradictory evidence is never completely ignored

            return current + delta * openness * agreementMultiplier;
        }
    }
}