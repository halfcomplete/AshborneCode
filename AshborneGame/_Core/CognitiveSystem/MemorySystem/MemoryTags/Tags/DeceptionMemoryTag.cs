using System;
using System.Collections.Generic;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags
{
    public class DeceptionMemoryTag : IMemoryTag
    {
        public MemoryTagType Type { get; } = MemoryTagType.Deception;

        public MemoryTagDefinition Definition { get; } = 
            new MemoryTagDefinition(
                // Base emotional modifiers
                new Dictionary<EmotionType, (MemoryRole role, double value)>
                {
                    {EmotionType.Anger, (MemoryRole.Actor, 0.6)},
                    {EmotionType.Contempt, (MemoryRole.Actor, 0.5)},
                    {EmotionType.Disgust, (MemoryRole.Actor, 0.3)},
                    {EmotionType.Surprise, (MemoryRole.Actor, 0.4)},
                    {EmotionType.Happiness, (MemoryRole.Actor, -0.3)},
                    {EmotionType.Sadness, (MemoryRole.Target, 0.3)},
                }, 
                // Personality reactions
                new Dictionary<PersonalityTrait, List<EmotionReaction>>
                {
                    {PersonalityTrait.Curiosity,
                    [
                        new EmotionReaction(EmotionType.Surprise, 1.7, 0.6, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Contempt, 0.9, 0.2, MemoryRole.Actor),
                    ]},
                    {PersonalityTrait.Aggression,
                    [
                        new EmotionReaction(EmotionType.Anger, 1.2, 0.7, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Contempt, 1.1, 0.4, MemoryRole.Actor),
                    ]},
                    {PersonalityTrait.Compassion,
                    [
                        new EmotionReaction(EmotionType.Disgust, 1.3, 0.4, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Sadness, 1.1, 0.3, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Sadness, 1.3, 0.3, MemoryRole.Target),
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
