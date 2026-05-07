using AshborneGame._Core.Globals.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AshborneGame._Core.EmotionSystem
{
    public class EmotionProfile
    {
        /// <summary>
        /// A list of all the active emotion modifiers currently affecting the character. Each modifier can contribute positively or negatively to the intensity of a specific emotion type, and their effects are evaluated lazily based on the time elapsed since they were applied. This allows for dynamic changes in the character's emotional state over time, as modifiers decay or are removed when depleted.
        /// If there are no modifiers active for a particular emotion type, the intensity of that emotion is considered to be 0. The overall intensity of each emotion type is calculated by summing the current values of all relevant modifiers and clamping the result between -100 and 100 to maintain a reasonable range for emotional intensity.
        /// </summary>
        private List<EmotionModifier> _modifiers = new List<EmotionModifier>();

        public void AddModifier(EmotionModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        /// <summary>
        /// Retrieves the evaluated emotion intensity for a specific type, 
        /// clamped between -100 and 100, factoring in time decay.
        /// </summary>
        public int GetCurrentEmotion(EmotionTypes type, int currentTotalHours)
        {
            CleanUpDepleted(currentTotalHours);

            int totalAmount = 0;
            foreach (var mod in _modifiers.Where(m => m.Type == type))
            {
                totalAmount += mod.GetCurrentAmount(currentTotalHours);
            }

            return Math.Clamp(totalAmount, -100, 100);
        }

        private void CleanUpDepleted(int currentTotalHours)
        {
            _modifiers.RemoveAll(m => m.IsDepleted(currentTotalHours));
        }
    }
}