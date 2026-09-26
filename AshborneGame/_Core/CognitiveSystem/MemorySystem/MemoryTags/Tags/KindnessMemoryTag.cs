using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;

public class KindnessMemoryTag : IMemoryTag
{
    public MemoryTagType Type => MemoryTagType.Kindness;

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
                            (EmotionType.Happiness, 0.55),
                            (EmotionType.Sadness, -0.15)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Target] = new() { 0.4 },
                [MemoryRole.Actor] = new() { 0.2 },
                [MemoryRole.Witness] = new() { 0.1 }
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
                                    (EmotionType.Happiness, 1.35)
                                }
                            ),
                            (
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Happiness, 1.25)
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
                            (RelationshipType.Loves, 0.2),
                            (RelationshipType.Trusts, 0.15)
                        }
                    )
                }
            },

            // Personality emotion
            new Dictionary<PersonalityTrait, List<(MemoryRole self, List<(EmotionType emotion, double value)> emotionValues)>>
            {
                [PersonalityTrait.Compassion] = new()
                {
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
                    (MemoryRole.Target, 0.1)
                }
            }
        );
}