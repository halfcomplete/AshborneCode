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
    public class LossMemoryTag : IMemoryTag
    {
        public MemoryTagType Type => MemoryTagType.Loss;

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
                            (EmotionType.Sadness, 0.7),
                            (EmotionType.Anger, 0.45)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Target] = new() { 0.75 },
                [MemoryRole.Actor] = new() { 0.3 },
                [MemoryRole.Witness] = new() { 0.2 }
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
                                    (EmotionType.Sadness, 1.5),
                                    (EmotionType.Anger, 1.25)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Sadness, 1.35),
                                    (EmotionType.Anger, 1.2)
                                }
                            ),
                            (
                                RelationshipType.Hates,
                                new()
                                {
                                    (EmotionType.Anger, 1.25)
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
                            (RelationshipType.Loves, 1.3),
                            (RelationshipType.Trusts, 1.25),
                            (RelationshipType.Hates, 1.1)
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
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Sadness, 1.4)
                        }
                    )
                },

                [PersonalityTrait.Aggression] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Anger, 1.5)
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
                    (MemoryRole.Target, 1.15)
                },

                [PersonalityTrait.Aggression] = new()
                {
                    (MemoryRole.Target, 1.1)
                }
            }
        );
    }
}

