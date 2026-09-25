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
        /// <summary>
        /// A Dictionary where the Key is each MemoryRole and the value is a tuple of (EmotionType, double) that defines the effect that this memory tag has on the self's emotions if this memory tag is on the memory and the self has this MemoryRole.
        /// </summary>
        /// <remarks>
        /// For example, the Deception may have a kvp of (MemoryRole.Target, (EmotionType.Happiness, -0.5)), meaning that if the memory has the Deception memory tag, then the Target NPC's happiness emotion will decrease by 0.5.
        /// </remarks>
        public Dictionary<MemoryRole, (EmotionType emotion, double value)> SelfEmotionRules { get; init; }
        
        /// <summary>
        /// A Dictionary where the Key is each MemoryRole and the value is a tuple of (MemoryRole, EmotionType, value) that defines the directed emotions the self would have towards other NPCs with the target MemoryRole if the self has the first MemoryRole.
        /// </summary>
        /// <remarks>
        /// These emotions are primarily used to affect attitudes between NPCs and also contribute to the aggregate emotion profile by a factor of 10%.
        /// </remarks>
        public Dictionary<MemoryRole, (MemoryRole target, EmotionType emotion, double value)> DirectedEmotionRules { get; init; }
        
        /// <summary>
        /// A Dictionary where the Key is each MemoryRole and
        /// </summary>
        public Dictionary<MemoryRole, (MemoryRole target, RelationshipType relationship, EmotionType emotion, double value)> AttitudeEmotionRules { get; init; }
        
        
        public Dictionary<MemoryRole, (MemoryRole target, RelationshipType relationship, double value)> AttitudeIntensityRules { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.
        /// </summary>
        public Dictionary<PersonalityTrait, (MemoryRole self, EmotionType emotion, double value)> PersonalityEmotionRules { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a double that defines how much of an effect that personality trait has on the Memory's intensity if this memory tag is on it.
        /// </summary>
        /// <remarks>
        /// For example, the SecretMemoryTag may have a kvp of (PersonalityTrait.Curiosity, +0.3), meaning that if the NPC is fully curious, then the intensity of memories with a Secret tag on them will increase by 0.3.
        /// </remarks>
        public Dictionary<PersonalityTrait, (MemoryRole self, double value)> PersonalityIntensityRules { get; init; }

        public MemoryTagDefinition(
            Dictionary<MemoryRole, (EmotionType emotion, double value)> selfEmotionRules,
            Dictionary<MemoryRole, (MemoryRole target, EmotionType emotion, double value)> directedEmotionRules,
            Dictionary<MemoryRole, (MemoryRole target, RelationshipType relationship, EmotionType emotion, double value)> attitudeEmotionRules,
            Dictionary<MemoryRole, (MemoryRole target, RelationshipType relationship, double value)> attitudeIntensityRules,
            Dictionary<PersonalityTrait, (MemoryRole self, EmotionType emotion, double value)> personalityEmotionRules,
            Dictionary<PersonalityTrait, (MemoryRole self, double value)> personalityIntensityRules)
        {
            SelfEmotionRules = selfEmotionRules;
            DirectedEmotionRules = directedEmotionRules;
            AttitudeEmotionRules = attitudeEmotionRules;
            AttitudeIntensityRules = attitudeIntensityRules;
            PersonalityEmotionRules = personalityEmotionRules;
            PersonalityIntensityRules = personalityIntensityRules;
        }
    }
}