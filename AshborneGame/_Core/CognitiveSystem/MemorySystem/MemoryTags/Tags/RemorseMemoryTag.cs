using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags
{
    public class RemorseMemoryTag : IMemoryTag
    {
        public MemoryTagType Type => MemoryTagType.Remorse;

        public MemoryTagDefinition Definition => new MemoryTagDefinition(

            // Initial emotions
            new Dictionary<
                MemoryRole,
                List<(
                    MemoryRole? target,
                    List<(EmotionType emotion, double value)> emotionValues
                )>
            >
            {
                [MemoryRole.Actor] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Sadness, 0.55),
                            (EmotionType.Fear, 0.15)
                        }
                    ),
                    (
                        null,
                        new()
                        {
                            (EmotionType.Sadness, 0.2)
                        }
                    )
                },

                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (EmotionType.Surprise, 0.15)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Actor] = new() { 0.55 },
                [MemoryRole.Target] = new() { 0.3 },
                [MemoryRole.Witness] = new() { 0.1 }
            },

            // Relationship emotion
            new Dictionary<
                MemoryRole,
                List<(
                    MemoryRole target,
                    List<(
                        RelationshipType relationship,
                        List<(EmotionType emotion, double value)> emotionValues
                    )> relationships
                )>
            >
            {
                [MemoryRole.Actor] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (
                                RelationshipType.Loves,
                                new()
                                {
                                    (EmotionType.Sadness, 1.4)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Sadness, 1.25)
                                }
                            ),
                            (
                                RelationshipType.Hates,
                                new()
                                {
                                    (EmotionType.Sadness, 0.7)
                                }
                            )
                        }
                    )
                },

                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (
                                RelationshipType.Loves,
                                new()
                                {
                                    (EmotionType.Surprise, 1.2)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Surprise, 1.15)
                                }
                            )
                        }
                    )
                }
            },

            // Relationship intensity
            new Dictionary<
                MemoryRole,
                List<(
                    MemoryRole target,
                    List<(RelationshipType relationship, double value)> relationships
                )>
            >
            {
                [MemoryRole.Actor] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (RelationshipType.Loves, 1.2),
                            (RelationshipType.Trusts, 1.15)
                        }
                    )
                },

                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (RelationshipType.Trusts, 1.1)
                        }
                    )
                }
            },

            // Personality emotion
            new Dictionary<
                PersonalityTrait,
                List<(
                    MemoryRole self,
                    List<(EmotionType emotion, double value)> emotionValues
                )>
            >
            {
                [PersonalityTrait.Compassion] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (EmotionType.Sadness, 1.4)
                        }
                    )
                }
            },

            // Personality intensity
            new Dictionary<
                PersonalityTrait,
                List<(MemoryRole self, double value)>
            >
            {
                [PersonalityTrait.Compassion] = new()
                {
                    (MemoryRole.Actor, 1.15)
                }
            }
        );
    }
}