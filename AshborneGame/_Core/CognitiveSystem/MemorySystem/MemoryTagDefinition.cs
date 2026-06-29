using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public record MemoryTagDefinition
    {
        public Dictionary<EmotionType, double> BaseEmotionalModifiers { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.
        /// </summary>
        public Dictionary<PersonalityTrait, List<PersonalityReaction>> PersonalityModifiers { get; init; }

        /// <summary>
        /// Instantiates a new MemoryTagDefinition record given the base emotional modifiers and a Dictionary of personality reactions.
        /// </summary>
        /// <param name="personalityModifiers">A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.</param>
        public MemoryTagDefinition(Dictionary<EmotionType, double> baseEmotionalModifiers, Dictionary<PersonalityTrait, List<PersonalityReaction>> personalityModifiers)
        {
            BaseEmotionalModifiers = baseEmotionalModifiers;
            PersonalityModifiers = personalityModifiers;
        }
    }
}