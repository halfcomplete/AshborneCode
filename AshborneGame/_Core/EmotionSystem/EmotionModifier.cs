using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A record representing a single modifier to an emotion, which has a type, an initial amount (positive or negative), and the in-game hour when it was applied.
    /// </summary>
    public record EmotionModifier
    {
        public EmotionType Type { get; private set; }
        public double InitialAmount { get; private set; }

        /// <summary>
        /// Creates a new modifier for a particular emotion.
        /// </summary>
        /// <param name="type">The emotion that this modifier affects.</param>
        /// <param name="initialAmount">The initial amount of the modifier. Can be any value, just the final emotional value that the NPC calculates will be clamped from -1 to 1.</param>
        public EmotionModifier(EmotionType type, double initialAmount)
        {
            Type = type;
            InitialAmount = initialAmount;
        }
    }
}