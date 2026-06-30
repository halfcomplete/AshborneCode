using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public class TheftMemoryTag : IMemoryTag
    {
        public MemoryTag Type { get; } = MemoryTag.Theft;

        public MemoryTagDefinition Definition { get; } = 
            new MemoryTagDefinition(
                // Base emotional modifiers
                new Dictionary<EmotionType, double>
                {
                    {EmotionType.Anger, 0.7},
                    {EmotionType.Contempt, 0.3},
                    {EmotionType.Surprise, 0.2},
                    {EmotionType.Happiness, -0.4},
                }, 
                // The personality reactions of each personality trait in relation to this memory tag
                new Dictionary<PersonalityTrait, List<PersonalityReaction>>
                {
                    {PersonalityTrait.Aggression,
                    [
                        new PersonalityReaction(EmotionType.Anger, 1.3, 0.8),
                        new PersonalityReaction(EmotionType.Contempt, 1.3, 0.4),
                    ]},
                    {PersonalityTrait.Compassion,
                    [
                        new PersonalityReaction(EmotionType.Sadness, 1.5, 0.4),
                        new PersonalityReaction(EmotionType.Anger, 0.8, 0.2),
                        new PersonalityReaction(EmotionType.Contempt, 0.7, 0.2),
                        new PersonalityReaction(EmotionType.Disgust, 0.6, 0.2),
                        new PersonalityReaction(EmotionType.Surprise, 1.3, 0.23),
                    ]},
                    {PersonalityTrait.Curiosity,
                    [
                        new PersonalityReaction(EmotionType.Sadness, 1.5, 0.3),
                        new PersonalityReaction(EmotionType.Surprise, 1.5, 0.4),
                        new PersonalityReaction(EmotionType.Contempt, 0.8, 0.2),
                        new PersonalityReaction(EmotionType.Anger, 0.8, 0.2),
                    ]},
                },
                new Dictionary<AttitudeType, List<AttitudeRoleIntensityRule>>
                {
                    {
                        AttitudeType.Loves,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.3),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.3)
                        ]
                    },
                    {
                        AttitudeType.Hates,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.2)
                        ]
                    },
                    {
                        AttitudeType.Respects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.15)
                        ]
                    },
                    {
                        AttitudeType.Disrespects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.15),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.3),
                        ]
                    },
                    {
                        AttitudeType.Trusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.2),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.3),
                        ]
                    },
                    {
                        AttitudeType.Distrusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.25),
                        ]
                    }
                }
            );
    }
}