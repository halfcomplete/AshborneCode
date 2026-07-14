using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record MemoryTagDefinition
    {
        public Dictionary<EmotionType, (MemoryRole role, double value)> BaseEmotionalModifiers { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.
        /// </summary>
        public Dictionary<PersonalityTrait, List<EmotionReaction>> PersonalityEmotionModifiers { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each attitude type (love, hate, etc) and the value is a list of intensity rules that define how intensity is affected if this NPC loves/hates/etc the victim/beneficiary/actor.
        /// </summary>
        public Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>> AttitudeIntensityModifiers { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each attitude type (loves, hates, etc) and the value is a list of emotion rules that define how emotion modifiers towards the target/actor/etc are affected if this NPC loves/hates/etc the target/actor/etc.
        /// </summary>
        public Dictionary<RelationshipType, List<AttitudeRoleEmotionRule>> AttitudeEmotionModifiers { get; init; }

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
            Dictionary<EmotionType, (MemoryRole role, double value)> baseEmotionalModifiers,
            Dictionary<PersonalityTrait, List<EmotionReaction>> personalityEmotionModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>> attitudeIntensityModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleEmotionRule>>? attitudeEmotionModifiers = null,
            Dictionary<PersonalityTrait, double>? personalityIntensityModifiers = null)
        {
            BaseEmotionalModifiers = baseEmotionalModifiers;
            AttitudeEmotionModifiers = attitudeEmotionModifiers ?? new();
            PersonalityEmotionModifiers = personalityEmotionModifiers;
            AttitudeIntensityModifiers = attitudeIntensityModifiers;
            PersonalityIntensityModifiers = personalityIntensityModifiers ?? new();
        }
    }
}