using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;

public class DeceptionMemoryTag : IMemoryTag
{
    public MemoryTagType Type => MemoryTagType.Deception;
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
                            (EmotionType.Anger, 0.5),
                            (EmotionType.Surprise, 0.4)
                        }
                    )
                }
            },

            // Intensity
            new Dictionary<MemoryRole, List<double>>
            {
                [MemoryRole.Target] = new() { 0.55 },
                [MemoryRole.Actor] = new() { 0.25 },
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
                                RelationshipType.Trusts,
                                new()
                                {
                                    (EmotionType.Anger, 1.3),
                                    (EmotionType.Surprise, 1.25)
                                }
                            ),
                            (
                                RelationshipType.Loves,
                                new()
                                {
                                    (EmotionType.Anger, 1.25),
                                    (EmotionType.Sadness, 1.2)
                                }
                            ),
                            (
                                RelationshipType.Hates,
                                new()
                                {
                                    (EmotionType.Anger, 1.2)
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
                            (RelationshipType.Trusts, 0.25),
                            (RelationshipType.Loves, 0.2),
                            (RelationshipType.Hates, 0.05)
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
                            (EmotionType.Anger, 1.35)
                        }
                    )
                }
            },

            // Personality intensity
            new Dictionary<PersonalityTrait, List<(MemoryRole self, double value)>>
            {
                [PersonalityTrait.Aggression] = new()
                {
                    (MemoryRole.Target, 0.1)
                }
            }
        );
}