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
                new Dictionary<EmotionType, (MemoryRole role, double value)>
                {
                    {EmotionType.Anger, (MemoryRole.Actor, 0.7)},
                    {EmotionType.Contempt, (MemoryRole.Actor, 0.3)},
                    {EmotionType.Surprise, (MemoryRole.Actor, 0.2)},
                    {EmotionType.Happiness, (MemoryRole.Actor, -0.4)},
                }, 
                // The personality reactions of each personality trait in relation to this memory tag
                new Dictionary<PersonalityTrait, List<EmotionReaction>>
                {
                    {PersonalityTrait.Aggression,
                    [
                        new EmotionReaction(EmotionType.Anger, 1.3, 0.8, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Contempt, 1.3, 0.4, MemoryRole.Actor),
                    ]},
                    {PersonalityTrait.Compassion,
                    [
                        new EmotionReaction(EmotionType.Sadness, 1.5, 0.4, MemoryRole.Target),
                        new EmotionReaction(EmotionType.Sadness, 1.3, 0.2, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Anger, 0.8, 0.2, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Contempt, 0.7, 0.2, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Disgust, 0.6, 0.2, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Surprise, 1.3, 0.23, MemoryRole.Actor),
                    ]},
                    {PersonalityTrait.Curiosity,
                    [
                        new EmotionReaction(EmotionType.Sadness, 1.5, 0.3, MemoryRole.Target),
                        new EmotionReaction(EmotionType.Surprise, 1.5, 0.4, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Contempt, 0.8, 0.2, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Anger, 0.8, 0.2, MemoryRole.Actor),
                    ]},
                },
                new Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>>
                {
                    {
                        RelationshipType.Loves,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.3),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.3)
                        ]
                    },
                    {
                        RelationshipType.Hates,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.2)
                        ]
                    },
                    {
                        RelationshipType.Respects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.15)
                        ]
                    },
                    {
                        RelationshipType.Disrespects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: -0.15),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.3),
                        ]
                    },
                    {
                        RelationshipType.Trusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.2),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: 0.3),
                        ]
                    },
                    {
                        RelationshipType.Distrusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, intensity: 0.25),
                            new AttitudeRoleIntensityRule(MemoryRole.Target, intensity: -0.25),
                        ]
                    }
                }
            );
    }
}