using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A class representing a single modifier to an emotion, which has a type, an initial amount (positive or negative), an intensity that controls how quickly it decays over time, and the in-game hour when it was applied. The current value of the modifier can be calculated lazily based on the time elapsed since it was applied, allowing for dynamic changes in emotion intensity as time passes.
    /// </summary>
    public class EmotionModifier
    {
        public EmotionTypes Type { get; private set; }
        public float InitialAmount { get; private set; }

        /// <summary>
        /// Controls duration. Higher intensity = slower decay. 
        /// E.g., an arbitrary high value like 9999 means effectively permanent, while a value of 1 means it decays very quickly. Must be at least 0.
        /// </summary>
        public int Intensity { get; private set; }
        
        /// <summary>
        /// The in-game total hour when this modifier was applied.
        /// </summary>
        public int StartHour { get; private set; }

        /// <summary>
        /// Creates a new modifier for a particular emotion.
        /// </summary>
        /// <param name="type">The emotion that this modifier affects.</param>
        /// <param name="initialAmount">The initial amount of the modifier.</param>
        /// <param name="intensity">The intensity of the modifier, controlling decay rate. The higher this is, the slower the decay. Must be at least 0.</param>
        /// <param name="startHour">The in-game hour when the modifier was applied.</param>
        public EmotionModifier(EmotionTypes type, float initialAmount, int intensity, int startHour)
        {
            Type = type;
            InitialAmount = Math.Clamp(initialAmount, -1f, 1f);
            Intensity = Math.Max(1, intensity); // Prevent divide by zero
            StartHour = startHour;
        }

        /// <summary>
        /// Lazily calculates the current value of this emotion modifier based on the time elapsed.
        /// The way the modifier decays over time is determined by its intensity. The higher the intensity, the slower it decays.
        /// <br>The formula is: <code>amountDecayed = hoursElapsed / Intensity</code></br>
        /// Note: the formula uses integer division, meaning any fractional part is discarded and will only affect the value later.
        /// </summary>
        public float GetCurrentAmount(int currentTotalHours)
        {
            int hoursElapsed = currentTotalHours - StartHour;
            if (hoursElapsed < 0) hoursElapsed = 0;

            // amountDecayed in the old 0..100 units was: hoursElapsed / Intensity.
            // Convert that to the new 0..1 scale: each old unit == 0.01f.
            float amountDecayed = (hoursElapsed / (float)Intensity) * 0.01f;

            if (InitialAmount > 0f)
            {
                return Math.Max(0f, InitialAmount - amountDecayed);
            }
            else if (InitialAmount < 0f)
            {
                return Math.Min(0f, InitialAmount + amountDecayed);
            }

            return 0f;
        }
        
        public bool IsDepleted(int currentTotalHours)
        {
            var current = GetCurrentAmount(currentTotalHours);
            return Math.Abs(current) <= 0.00001f;
        }
    }
}