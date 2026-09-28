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
    public class CareMemoryTag : IMemoryTag
    {
        public MemoryTagType Type => MemoryTagType.Care;

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
                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (EmotionType.Happiness, 0.5)
                        }
                    )
                },

                [MemoryRole.Actor] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Happiness, 0.3)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Target] = new() { 0.5 },
                [MemoryRole.Actor] = new() { 0.4 },
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
                                    (EmotionType.Happiness, 1.4)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Happiness, 1.3)
                                }
                            ),
                            (
                                RelationshipType.Hates,
                                new()
                                {
                                    (EmotionType.Happiness, 0.7)
                                }
                            )
                        }
                    )
                },

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
                                    (EmotionType.Happiness, 1.3)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Happiness, 1.15)
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
                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (RelationshipType.Loves, 0.2),
                            (RelationshipType.Trusts, 0.15)
                        }
                    )
                },

                [MemoryRole.Actor] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (RelationshipType.Loves, 0.15),
                            (RelationshipType.Trusts, 0.1)
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
                            (EmotionType.Happiness, 1.4)
                        }
                    ),
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Happiness, 1.3)
                        }
                    )
                }
            },

            // Personality intensity
            new Dictionary<PersonalityTrait, List<(MemoryRole self, double value)>>
            {
                [PersonalityTrait.Compassion] = new()
                {
                    (MemoryRole.Actor, 0.15),
                    (MemoryRole.Target, 0.1)
                }
            }
        );
    }
}
