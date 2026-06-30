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
                new Dictionary<PersonalityTrait, List<PersonalityReaction>>
                {
                    {PersonalityTrait.Curiosity,
                    [
                        new PersonalityReaction(EmotionType.Surprise, 1.7, 0.6),
                        new PersonalityReaction(EmotionType.Contempt, 0.9, 0.2),
                    ]},
                    {PersonalityTrait.Aggression,
                    [
                        new PersonalityReaction(EmotionType.Anger, 1.2, 0.7),
                        new PersonalityReaction(EmotionType.Contempt, 1.1, 0.4),
                    ]},
                    {PersonalityTrait.Compassion,
                    [
                        new PersonalityReaction(EmotionType.Disgust, 1.3, 0.4),
                        new PersonalityReaction(EmotionType.Sadness, 1.1, 0.3),
                    ]},
                },
                new Dictionary<AttitudeType, List<AttitudeRoleIntensityRule>>
                {
                    {
                        AttitudeType.Trusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.5),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.3)
                        ]
                    },
                    {
                        AttitudeType.Distrusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.1)
                        ]
                    },
                    {
                        AttitudeType.Loves,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.4),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.2)
                        ]
                    },
                    {
                        AttitudeType.Hates,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.2)
                        ]
                    },
                    {
                        AttitudeType.Respects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.2)
                        ]
                    },
                    {
                        AttitudeType.Disrespects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.15)
                        ]
                    }
                }
            );
    }
}
