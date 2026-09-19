using System;
using System.Collections.Generic;
using System.Linq;
using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public record MemoryTagDefinition
    {
        public IReadOnlyList<SelfEmotionRule> SelfEmotionRules { get; init; }
        public IReadOnlyList<DirectedEmotionRule> DirectedEmotionRules { get; init; }
        public IReadOnlyList<AttitudeRule> AttitudeRules { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.
        /// </summary>
        public IReadOnlyList<PersonalityEmotionModifier> PersonalityEmotionModifiers { get; init; }

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
        public IReadOnlyList<PersonalityIntensityModifier> PersonalityIntensityModifiers { get; init; }

        /// <summary>
        /// Instantiates a new MemoryTagDefinition record given the base emotional modifiers and a Dictionary of personality reactions.
        /// </summary>
        /// <param name="personalityEmotionModifiers">A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.</param>
        public MemoryTagDefinition(
            IReadOnlyList<SelfEmotionRule> selfEmotionRules,
            IReadOnlyList<DirectedEmotionRule> directedEmotionRules,
            IReadOnlyList<AttitudeRule> attitudeRules,
            IReadOnlyList<PersonalityEmotionModifier> personalityEmotionModifiers,
            IReadOnlyList<PersonalityIntensityModifier> personalityIntensityModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>>? attitudeIntensityModifiers = null,
            Dictionary<RelationshipType, List<AttitudeRoleEmotionRule>>? attitudeEmotionModifiers = null)
        {
            SelfEmotionRules = selfEmotionRules;
            DirectedEmotionRules = directedEmotionRules;
            AttitudeRules = attitudeRules;
            AttitudeEmotionModifiers = attitudeEmotionModifiers ?? new();
            PersonalityEmotionModifiers = personalityEmotionModifiers;
            AttitudeIntensityModifiers = attitudeIntensityModifiers ?? new();
            PersonalityIntensityModifiers = personalityIntensityModifiers;
        }

        public MemoryTagDefinition(
            Dictionary<EmotionType, (MemoryRole role, double value)> baseEmotionalModifiers,
            Dictionary<PersonalityTrait, List<EmotionReaction>> personalityEmotionModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>> attitudeIntensityModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleEmotionRule>>? attitudeEmotionModifiers = null,
            Dictionary<PersonalityTrait, double>? personalityIntensityModifiers = null)
            : this(
                baseEmotionalModifiers
                    .Select(pair => new SelfEmotionRule(pair.Value.role, pair.Key, pair.Value.value))
                    .ToList(),
                [],
                [],
                personalityEmotionModifiers
                    .SelectMany(pair => pair.Value.Select(reaction => new PersonalityEmotionModifier(
                        pair.Key,
                        reaction.Role,
                        null,
                        reaction.Emotion,
                        reaction.Mult,
                        reaction.Add)))
                    .ToList(),
                personalityIntensityModifiers
                    ?.Select(pair => new PersonalityIntensityModifier(pair.Key, MemoryRole.Witness, pair.Value))
                    .ToList() ?? [],
                attitudeIntensityModifiers,
                attitudeEmotionModifiers)
        {
        }

        public MemoryTagDefinition(
            Dictionary<EmotionType, (MemoryRole? role, double value)> baseEmotionalModifiers,
            Dictionary<PersonalityTrait, List<EmotionReaction>> personalityEmotionModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>> attitudeIntensityModifiers,
            Dictionary<RelationshipType, List<AttitudeRoleEmotionRule>>? attitudeEmotionModifiers = null,
            Dictionary<PersonalityTrait, double>? personalityIntensityModifiers = null)
            : this(
                baseEmotionalModifiers.ToDictionary(
                    pair => pair.Key,
                    pair => (pair.Value.role ?? MemoryRole.Witness, pair.Value.value)),
                personalityEmotionModifiers,
                attitudeIntensityModifiers,
                attitudeEmotionModifiers,
                personalityIntensityModifiers)
        {
        }
    }
}