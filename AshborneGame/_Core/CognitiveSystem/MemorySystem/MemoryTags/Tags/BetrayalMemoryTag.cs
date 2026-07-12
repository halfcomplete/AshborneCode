using System;
using System.Collections.Generic;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags
{
    public class BetrayalMemoryTag : IMemoryTag
    {
        public MemoryTagType Type { get; } = MemoryTagType.Betrayal;

        public MemoryTagDefinition Definition { get; } = 
            new MemoryTagDefinition(
                // Base emotional modifiers
                new Dictionary<EmotionType, (MemoryRole role, double value)>
                {
                    {EmotionType.Anger, (MemoryRole.Actor, 0.8)},
                    {EmotionType.Sadness, (MemoryRole.Target, 0.3)},
                    {EmotionType.Contempt, (MemoryRole.Actor, 0.4)},
                    {EmotionType.Happiness, (MemoryRole.Actor, -0.5)},
                }, 
                // Personality reactions
                new Dictionary<PersonalityTrait, List<EmotionReaction>>
                {
                    {PersonalityTrait.Compassion,
                    [
                        new EmotionReaction(EmotionType.Sadness, 1.6, 0.5, MemoryRole.Target),
                        new EmotionReaction(EmotionType.Anger, 0.9, 0.2, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Sadness, 1.5, 0.3, MemoryRole.Actor),
                    ]},
                    {PersonalityTrait.Aggression,
                    [
                        new EmotionReaction(EmotionType.Anger, 1.4, 0.9, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Contempt, 1.2, 0.5, MemoryRole.Actor),
                        new EmotionReaction(EmotionType.Sadness, 0.8, -0.2, MemoryRole.Target),
                    ]},
                    {PersonalityTrait.Curiosity,
                    [
                        new EmotionReaction(EmotionType.Surprise, 1.4, 0.4, MemoryRole.Actor),
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
