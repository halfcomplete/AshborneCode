using System;
using System.Collections.Generic;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public class DeceptionMemoryTag : IMemoryTag
    {
        public MemoryTag Type { get; } = MemoryTag.Deception;

        public MemoryTagDefinition Definition { get; } = 
            new MemoryTagDefinition(
                // Base emotional modifiers
                new Dictionary<EmotionType, double>
                {
                    {EmotionType.Anger, 0.6},
                    {EmotionType.Contempt, 0.5},
                    {EmotionType.Disgust, 0.3},
                    {EmotionType.Surprise, 0.4},
                    {EmotionType.Happiness, -0.3},
                }, 
                // Personality reactions
                new Dictionary<PersonalityTrait, List<EmotionReaction>>
                {
                    {PersonalityTrait.Curiosity,
                    [
                        new EmotionReaction(EmotionType.Surprise, 1.7, 0.6),
                        new EmotionReaction(EmotionType.Contempt, 0.9, 0.2),
                    ]},
                    {PersonalityTrait.Aggression,
                    [
                        new EmotionReaction(EmotionType.Anger, 1.2, 0.7),
                        new EmotionReaction(EmotionType.Contempt, 1.1, 0.4),
                    ]},
                    {PersonalityTrait.Compassion,
                    [
                        new EmotionReaction(EmotionType.Disgust, 1.3, 0.4),
                        new EmotionReaction(EmotionType.Sadness, 1.1, 0.3),
                    ]},
                },
                new Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>>
                {
                    {
                        RelationshipType.Trusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.5),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.3)
                        ]
                    },
                    {
                        RelationshipType.Distrusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.1)
                        ]
                    },
                    {
                        RelationshipType.Loves,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.4),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.2)
                        ]
                    },
                    {
                        RelationshipType.Hates,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.2)
                        ]
                    },
                    {
                        RelationshipType.Respects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.2)
                        ]
                    },
                    {
                        RelationshipType.Disrespects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.15)
                        ]
                    }
                }
            );
    }
}
