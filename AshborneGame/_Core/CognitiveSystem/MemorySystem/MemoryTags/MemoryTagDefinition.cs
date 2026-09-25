using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.DefinitionRules;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;

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
        public List<SelfEmotionRule> SelfEmotionRules { get; init; }
        
        /// <summary>
        /// A Dictionary where the Key is each MemoryRole and the value is a tuple of (MemoryRole, EmotionType, value) that defines the directed emotions the self would have towards other NPCs with the target MemoryRole if the self has the first MemoryRole.
        /// </summary>
        /// <remarks>
        /// These emotions are primarily used to affect attitudes between NPCs and also contribute to the aggregate emotion profile by a factor of 10%.
        /// </remarks>
        public List<DirectedEmotionRule> DirectedEmotionRules { get; init; }
        
        /// <summary>
        /// A Dictionary where the Key is each MemoryRole and the value is a tuple of (MemoryRole, RelationshipType, EmotionType, value) that defines the relationship emotions the self would have towards other NPCs with the target MemoryRole if the self has the first MemoryRole.
        /// </summary>
        public List<RelationshipEmotionRule> RelationshipEmotionRules { get; init; }
        
        
        public List<RelationshipIntensityRule> RelationshipIntensityRules { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a list of personality reactions that define the effect that personality trait has on each emotion if this memory tag is on the memory.
        /// </summary>
        public List<PersonalityEmotionRule> PersonalityEmotionRules { get; init; }

        /// <summary>
        /// A Dictionary where the Key is each personality trait and the value is a double that defines how much of an effect that personality trait has on the Memory's intensity if this memory tag is on it.
        /// </summary>
        /// <remarks>
        /// For example, the SecretMemoryTag may have a kvp of (PersonalityTrait.Curiosity, +0.3), meaning that if the NPC is fully curious, then the intensity of memories with a Secret tag on them will increase by 0.3.
        /// </remarks>
        public List<PersonalityIntensityRule> PersonalityIntensityRules { get; init; }

        public MemoryTagDefinition(
            Dictionary<MemoryRole, List<(EmotionType emotion, double value)>> selfEmotionRules,
            Dictionary<MemoryRole, List<(MemoryRole target, List<(EmotionType emotion, double value)> emotionValues)>> directedEmotionRules,
            Dictionary<MemoryRole, List<(MemoryRole target, List<(RelationshipType relationship, List<(EmotionType emotion, double value)> emotionValues)> relationships)>> relationshipEmotionRules,
            Dictionary<MemoryRole, List<(MemoryRole target, List<(RelationshipType relationship, double value)> relationships)>> attitudeIntensityRules,
            Dictionary<PersonalityTrait, List<(MemoryRole self, List<(EmotionType emotion, double value)> emotionValues)>> personalityEmotionRules,
            Dictionary<PersonalityTrait, List<(MemoryRole self, double value)>> personalityIntensityRules)
        {
            SelfEmotionRules = selfEmotionRules
                .SelectMany(pair => 
                    pair.Value.Select(emotion => 
                        new SelfEmotionRule(
                            pair.Key, 
                            emotion.emotion, 
                            emotion.value
                        )
                    )
                )
                .ToList();

            DirectedEmotionRules = directedEmotionRules
                .SelectMany(role =>
                    role.Value.SelectMany(target =>
                        target.emotionValues.Select(emotion =>
                            new DirectedEmotionRule(
                                SubjectRole: role.Key,
                                TargetRole: target.target,
                                Emotion: emotion.emotion,
                                Value: emotion.value
                            )
                        )
                    )
                )
                .ToList();

            RelationshipEmotionRules = relationshipEmotionRules
                .SelectMany(role =>
                    role.Value.SelectMany(target =>
                        target.relationships.SelectMany(relationship =>
                            relationship.emotionValues.Select(pair =>
                                new RelationshipEmotionRule(
                                    role.Key,
                                    target.target,
                                    relationship.relationship,
                                    pair.emotion,
                                    pair.value
                                )
                            )
                        )
                    )
                )
                .ToList();

            RelationshipIntensityRules = attitudeIntensityRules
                .SelectMany(role =>
                    role.Value.SelectMany(target =>
                        target.relationships.Select(relationship =>
                            new RelationshipIntensityRule(
                                role.Key,
                                target.target,
                                relationship.relationship,
                                relationship.value
                            )
                        )
                    )
                )
                .ToList();

            PersonalityEmotionRules = personalityEmotionRules
                .SelectMany(pair => 
                    pair.Value.SelectMany(emotion =>
                        emotion.emotionValues.Select(emotionValue =>
                            new PersonalityEmotionRule(
                                pair.Key, 
                                emotion.self, 
                                emotionValue.emotion,
                                emotionValue.value
                            )
                        )
                    )
                )
                .ToList();

            PersonalityIntensityRules = personalityIntensityRules
                .SelectMany(pair => pair.Value.Select(intensity => 
                    new PersonalityIntensityRule(
                        pair.Key,
                        intensity.self,
                        intensity.value
                    )
                ))
                .ToList();
        }
    }
}