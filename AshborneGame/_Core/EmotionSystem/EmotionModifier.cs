using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A class representing a single modifier to an emotion, which has a type, an initial amount (positive or negative), an intensity that controls how quickly it decays over time, and the in-game hour when it was applied. The current value of the modifier can be calculated lazily based on the time elapsed since it was applied, allowing for dynamic changes in emotion intensity as time passes.
    /// </summary>
    public class EmotionModifier
    {
        public EmotionTypes Type { get; private set; }
        public int InitialAmount { get; private set; }
        
        /// <summary>
        /// Controls duration. Higher intensity = slower decay. 
        /// E.g., an arbitrary high value like 9999 means effectively permanent.
        /// </summary>
        public int Intensity { get; private set; }
        
        /// <summary>
        /// The in-game total hour when this modifier was applied.
        /// </summary>
        public int StartHour { get; private set; }

        public EmotionModifier(EmotionTypes type, int initialAmount, int intensity, int startHour)
        {
            Type = type;
            InitialAmount = initialAmount;
            Intensity = Math.Max(1, intensity); // Prevent divide by zero
            StartHour = startHour;
        }

        /// <summary>
        /// Lazily calculates the current value of this emotion modifier based on the time elapsed.
        /// </summary>
        public int GetCurrentAmount(int currentTotalHours)
        {
            int hoursElapsed = currentTotalHours - StartHour;
            if (hoursElapsed < 0) hoursElapsed = 0;

            // Simple linear decay: decays by 1 point per 'Intensity' hours.
            // Example: InitialAmount = 50, Intensity = 2.
            // Every 2 in-game hours, it drops by 1.
            // If it runs out, clamping handles it.
            int amountDecayed = hoursElapsed / Intensity;

            if (InitialAmount > 0)
            {
                return Math.Max(0, InitialAmount - amountDecayed);
            }
            else if (InitialAmount < 0)
            {
                return Math.Min(0, InitialAmount + amountDecayed);
            }

            return 0;
        }
        
        public bool IsDepleted(int currentTotalHours)
        {
            return GetCurrentAmount(currentTotalHours) == 0;
        }
    }
}