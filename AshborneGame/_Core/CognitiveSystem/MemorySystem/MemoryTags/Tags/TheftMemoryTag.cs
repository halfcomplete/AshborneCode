using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;

public class TheftMemoryTag : IMemoryTag
{
    public MemoryTagType Type => MemoryTagType.Theft;

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
                            (EmotionType.Anger, 0.6),
                            (EmotionType.Sadness, 0.35)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Target] = new() { 0.65 },
                [MemoryRole.Actor] = new() { 0.25 },
                [MemoryRole.Witness] = new() { 0.15 }
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
                                    (EmotionType.Sadness, 0.7),
                                    (EmotionType.Anger, 0.8)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Sadness, 0.55),
                                    (EmotionType.Anger, 0.6)
                                }
                            ),
                            (
                                RelationshipType.Hates,
                                new()
                                {
                                    (EmotionType.Anger, 1.35)
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
                            (RelationshipType.Loves, 0.25),
                            (RelationshipType.Trusts, 0.2),
                            (RelationshipType.Hates, 0.1)
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
                            (EmotionType.Sadness, 1.3)
                        }
                    )
                }
            },

            // Personality intensity
            new Dictionary<PersonalityTrait, List<(MemoryRole self, double value)>>
            {
                [PersonalityTrait.Aggression] = new()
                {
                    (MemoryRole.Target, 0.15)
                }
            }
        );
}