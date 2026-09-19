using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags
{
    public class ViolenceMemoryTag : IMemoryTag
    {
        public MemoryTagType Type { get; } = MemoryTagType.Violence;

        public MemoryTagDefinition Definition { get; } =
            new MemoryTagDefinition(
                // Base emotional modifiers
                new Dictionary<EmotionType, (MemoryRole? role, double value)>
                {
                    { EmotionType.Anger, (MemoryRole.Actor, 0.7) },
                    { EmotionType.Contempt, (MemoryRole.Actor, 0.3) },
                    { EmotionType.Disgust, (MemoryRole.Actor, 0.5) },
                    { EmotionType.Fear, (MemoryRole.Target, 0.6) },
                    { EmotionType.Sadness, (MemoryRole.Target, 0.4) },
                    { EmotionType.Fear, (MemoryRole.Witness, 0.3) },
                    { EmotionType.Surprise, (MemoryRole.Witness, 0.3) },
                },

                // Personality reactions
                new Dictionary<PersonalityTrait, List<EmotionReaction>>
                {
                    {
                        PersonalityTrait.Aggression,
                        [
                            new EmotionReaction(EmotionType.Anger, 1.3, 0.5, MemoryRole.Actor),
                            new EmotionReaction(EmotionType.Contempt, 1.2, 0.3, MemoryRole.Actor),
                            new EmotionReaction(EmotionType.Fear, 0.8, -0.1, MemoryRole.Witness),
                        ]
                    },
                    {
                        PersonalityTrait.Compassion,
                        [
                            new EmotionReaction(EmotionType.Sadness, 1.4, 0.3, MemoryRole.Target),
                            new EmotionReaction(EmotionType.Disgust, 1.2, 0.2, MemoryRole.Actor),
                            new EmotionReaction(EmotionType.Anger, 1.2, 0.2, MemoryRole.Actor),
                        ]
                    },
                    {
                        PersonalityTrait.Curiosity,
                        [
                            new EmotionReaction(EmotionType.Surprise, 1.2, 0.2, MemoryRole.Witness),
                            new EmotionReaction(EmotionType.Surprise, 1.1, 0.1, MemoryRole.Target),
                        ]
                    }
                },

                // Relationships affect how intensely the memory is retained
                new Dictionary<RelationshipType, List<AttitudeRoleIntensityRule>>
                {
                    {
                        RelationshipType.Loves,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, 0.5),
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, 0.2)
                        ]
                    },
                    {
                        RelationshipType.Hates,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, -0.2)
                        ]
                    },
                    {
                        RelationshipType.Trusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, -0.15)
                        ]
                    },
                    {
                        RelationshipType.Distrusts,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, 0.25)
                        ]
                    },
                    {
                        RelationshipType.Fears,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Actor, 0.4)
                        ]
                    },
                    {
                        RelationshipType.Respects,
                        [
                            new AttitudeRoleIntensityRule(MemoryRole.Target, 0.2)
                        ]
                    }
                },

                // Relationship-specific emotional interpretation
                new Dictionary<RelationshipType, List<AttitudeRoleEmotionRule>>
                {
                    {
                        RelationshipType.Loves,
                        [
                            new AttitudeRoleEmotionRule(
                                MemoryRole.Target, EmotionType.Sadness, 1.4, 0.2),
                            new AttitudeRoleEmotionRule(
                                MemoryRole.Actor, EmotionType.Anger, 1.2, 0.2)
                        ]
                    },
                    {
                        RelationshipType.Hates,
                        [
                            new AttitudeRoleEmotionRule(
                                MemoryRole.Actor, EmotionType.Anger, 0.8, -0.1)
                        ]
                    },
                    {
                        RelationshipType.Fears,
                        [
                            new AttitudeRoleEmotionRule(
                                MemoryRole.Actor, EmotionType.Fear, 1.3, 0.2)
                        ]
                    }
                },

                // Personality affects memory persistence
                new Dictionary<PersonalityTrait, double>
                {
                    { PersonalityTrait.Aggression, 0.25 },
                    { PersonalityTrait.Compassion, 0.25 },
                    { PersonalityTrait.Curiosity, 0.05 }
                }
            );
    }
}