using AshborneGame._Core.Globals.Enums;
using System.Collections.Generic;
using System.Linq;

namespace AshborneGame._Core.EmotionSystem
{
    public class EmotionProfile
    {
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