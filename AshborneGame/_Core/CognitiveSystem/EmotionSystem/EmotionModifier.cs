using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Game.Events;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// A record representing a single modifier to an emotion, which has a type and an initial amount (positive or negative).
    /// </summary>
    public record EmotionModifier
    {
        public EmotionType Type { get; init; }
        public double InitialAmount { get; init; }

        public MemoryParticipant Target { get; init; }
        public Memory? ParentMemory { get; init; }

        /// <summary>
        /// Creates a new modifier for a particular emotion.
        /// </summary>
        /// <param name="parentMemory">The Memory that caused this modifier.</param>
        /// <param name="target">The MemoryParticipant this modifier is directed to.</param>
        /// <param name="type">The emotion that this modifier affects.</param>
        /// <param name="initialAmount">The initial amount of the modifier. Can be any value, just the final emotional value that the NPC calculates will be clamped from -1 to 1.</param>
        public EmotionModifier(Memory? parentMemory, MemoryParticipant target, EmotionType type, double initialAmount)
        {
            ParentMemory = parentMemory;
            Target = target;
            Type = type;
            InitialAmount = initialAmount;
        }
    }
}