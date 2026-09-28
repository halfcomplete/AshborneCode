using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags;

public class BetrayalMemoryTag : IMemoryTag
{
    public MemoryTagType Type => MemoryTagType.Betrayal;
    
    public MemoryTagDefinition Definition =>
        new MemoryTagDefinition(
            // Initial emotions
            new Dictionary<MemoryRole, List<(MemoryRole? target, List<(EmotionType emotion, double value)> emotionValues)>>
            {
                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (EmotionType.Anger, 0.7),
                            (EmotionType.Sadness, 0.65)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Target] = new() { 0.8 },
                [MemoryRole.Actor] = new() { 0.35 },
                [MemoryRole.Witness] = new() { 0.2 }
            },

            // Relationship emotion
            new Dictionary<MemoryRole, List<(MemoryRole target, List<(RelationshipType relationship, List<(EmotionType emotion, double value)> emotionValues)> relationships)>>
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
                                    (EmotionType.Anger, 1.4)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Sadness, 1.4),
                                    (EmotionType.Anger, 1.3)
                                }
                            ),
                            (
                                RelationshipType.Hates,
                                new()
                                {
                                    (EmotionType.Anger, 1.15)
                                }
                            )
                        }
                    )
                }
            },

            // Relationship intensity
            new Dictionary<MemoryRole, List<(MemoryRole target, List<(RelationshipType relationship, double value)> relationships)>>
            {
                [MemoryRole.Target] = new()
                {
                    (
                        MemoryRole.Actor,
                        new()
                        {
                            (RelationshipType.Loves, 1.3),
                            (RelationshipType.Trusts, 1.35),
                            (RelationshipType.Hates, 1.05)
                        }
                    )
                }
            },

            // Personality emotion
            new Dictionary<PersonalityTrait, List<(MemoryRole self, List<(EmotionType emotion, double value)> emotionValues)>>
            {
                [PersonalityTrait.Aggression] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Anger, 1.45)
                        }
                    )
                },
                [PersonalityTrait.Compassion] = new()
                {
                    (
                        MemoryRole.Target,
                        new()
                        {
                            (EmotionType.Sadness, 1.35)
                        }
                    )
                }
            },

            // Personality intensity
            new Dictionary<PersonalityTrait, List<(MemoryRole self, double value)>>
            {
                [PersonalityTrait.Aggression] = new()
                {
                    (MemoryRole.Target, 1.1)
                },
                [PersonalityTrait.Compassion] = new()
                {
                    (MemoryRole.Target, 1.1)
                }
            }
        );
}