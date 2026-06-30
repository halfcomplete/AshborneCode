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
        public Dictionary<PersonalityTrait, List<PersonalityReaction>> PersonalityEmotionModifiers { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each attitude type (love, hate, etc) and the value is a list of intensity rules that define how intensity is affected if this NPC loves/hates/etc the victim/beneficiary/actor.
        /// </summary>
        public Dictionary<AttitudeType, List<AttitudeRoleIntensityRule>> AttitudeIntensityModifiers { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a double that defines how much of an effect that personality trait has on the Memory's intensity if this memory tag is on it.
        /// </summary>
        /// <remarks>
        /// For example, the SecretMemoryTag may have a kvp of (PersonalityTrait.Curiosity, +0.3), meaning that if the NPC is fully curious, then the intensity of memories with a Secret tag on them will increase by 0.3.
        /// </remarks>
        public Dictionary<PersonalityTrait, double> PersonalityIntensityModifiers { get; init; }

        /// <summary>
        /// Instantiates a new MemoryTagDefinition record given the base emotional modifiers and a Dictionary of personality reactions.
        /// </summary>
        /// <param name="personalityEmotionModifiers">A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.</param>
        public MemoryTagDefinition(
            Dictionary<EmotionType, double> baseEmotionalModifiers,
            Dictionary<PersonalityTrait, List<PersonalityReaction>> personalityEmotionModifiers,
            Dictionary<AttitudeType, List<AttitudeRoleIntensityRule>> attitudeIntensityModifiers,
            Dictionary<PersonalityTrait, double>? personalityIntensityModifiers = null)
        {
            BaseEmotionalModifiers = baseEmotionalModifiers;
            PersonalityEmotionModifiers = personalityEmotionModifiers;
            AttitudeIntensityModifiers = attitudeIntensityModifiers;
            PersonalityEmotionModifiers = personalityEmotionModifiers ?? new();
        }
    }
}