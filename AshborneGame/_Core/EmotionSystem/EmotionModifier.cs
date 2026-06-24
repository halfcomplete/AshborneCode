using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A class representing a single modifier to an emotion, which has a type, an initial amount (positive or negative), an intensity that controls how quickly it decays over time, and the in-game hour when it was applied. The current value of the modifier can be calculated lazily based on the time elapsed since it was applied, allowing for dynamic changes in emotion intensity as time passes.
    /// </summary>
    public class EmotionModifier
    {
        public EmotionType Type { get; private set; }
        public double InitialAmount { get; private set; }
        
        /// <summary>
        /// The in-game total hour when this modifier was applied.
        /// </summary>
        public int StartHour { get; private set; }

        /// <summary>
        /// Creates a new modifier for a particular emotion.
        /// </summary>
        /// <param name="type">The emotion that this modifier affects.</param>
        /// <param name="initialAmount">The initial amount of the modifier.</param>
        /// <param name="startHour">The in-game hour when the modifier was applied.</param>
        public EmotionModifier(EmotionType type, double initialAmount, int startHour)
        {
            Type = type;
            InitialAmount = Math.Clamp(initialAmount, -1, 1);
            StartHour = startHour;
        }
    }
}