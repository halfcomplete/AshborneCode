using System;
using System.Collections.Generic;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public class BetrayalMemoryTag : IMemoryTag
    {
        public MemoryTag Type { get; } = MemoryTag.Betrayal;

        public MemoryTagDefinition Definition { get; } = 
            new MemoryTagDefinition(
                // Base emotional modifiers
                new Dictionary<EmotionType, double>
                {
                    {EmotionType.Anger, 0.8},
                    {EmotionType.Sadness, 0.6},
                    {EmotionType.Contempt, 0.4},
                    {EmotionType.Happiness, -0.5},
                }, 
                // Personality reactions
                new Dictionary<PersonalityTrait, List<EmotionReaction>>
                {
                    {PersonalityTrait.Compassion,
                    [
                        new EmotionReaction(EmotionType.Sadness, 1.6, 0.5),
                        new EmotionReaction(EmotionType.Anger, 0.9, 0.2),
                    ]},
                    {PersonalityTrait.Aggression,
                    [
                        new EmotionReaction(EmotionType.Anger, 1.4, 0.9),
                        new EmotionReaction(EmotionType.Contempt, 1.2, 0.5),
                    ]},
                    {PersonalityTrait.Curiosity,
                    [
                        new EmotionReaction(EmotionType.Sadness, 1.2, 0.3),
                        new EmotionReaction(EmotionType.Surprise, 1.4, 0.3),
                    ]},
                },
                new Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>>
                {
                    {
                        RelationshipType.Trusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.6),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.4)
                        ]
                    },
                    {
                        RelationshipType.Distrusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.2),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.1)
                        ]
                    },
                    {
                        RelationshipType.Loves,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.5),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.3)
                        ]
                    },
                    {
                        RelationshipType.Hates,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.15),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.1)
                        ]
                    },
                    {
                        RelationshipType.Respects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25)
                        ]
                    }
                }
            );
    }
}
