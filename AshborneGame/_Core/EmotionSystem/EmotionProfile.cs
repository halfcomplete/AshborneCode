using AshborneGame._Core.Globals.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// Manages the dynamic emotional state of a character over time by tracking modifiers. Note that it tracks EVERY modifier acting on a single character, not just for one emotion.
    /// </summary>
    /// <remarks>The EmotionProfile class allows for the addition and evaluation of emotion modifiers, each of
    /// which can influence the intensity of a specific emotion type. The overall emotional state is determined by
    /// aggregating the effects of all active modifiers, with intensities clamped to a defined range. Modifiers are
    /// automatically removed when they are depleted, ensuring that the emotional state reflects only current
    /// influences.</remarks>
    public class EmotionProfile
    {
        /// <summary>
        /// A list of all the active emotion modifiers currently affecting the character. Each modifier can contribute positively or negatively to the intensity of a specific emotion type, and their effects are evaluated lazily based on the time elapsed since they were applied. This allows for dynamic changes in the character's emotional state over time, as modifiers decay or are removed when depleted.
        /// If there are no modifiers active for a particular emotion type, the intensity of that emotion is considered to be 0. The overall intensity of each emotion type is calculated by summing the current values of all relevant modifiers and clamping the result between 0 and 1 to maintain a reasonable range for emotional intensity.
        /// </summary>
        private List<EmotionModifier> _modifiers = new List<EmotionModifier>();

        public void AddModifier(EmotionModifier modifier)
        {
            _modifiers.Add(modifier);
        }

        /// <summary>
        /// Retrieves the evaluated emotion intensity for a specific type, 
        /// clamped between 0.0 and 1.0, factoring in time decay.
        /// </summary>
        public float GetCurrentEmotion(EmotionTypes type, int currentTotalHours)
        {
            CleanUpDepleted(currentTotalHours);

            float totalAmount = 0f;
            foreach (var mod in _modifiers.Where(m => m.Type == type))
            {
                totalAmount += mod.GetCurrentAmount(currentTotalHours);
            }

            return Math.Clamp(totalAmount, 0f, 1f);
        }

        private void CleanUpDepleted(int currentTotalHours)
        {
            _modifiers.RemoveAll(m => m.IsDepleted(currentTotalHours));
        }
    }
}