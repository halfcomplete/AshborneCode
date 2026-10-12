using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;

namespace AshborneTests;

[Collection("AshborneTests")]
public class CognitionBehaviourTests
{
    [Fact]
    public void MemoryEmotionProfile_ShouldCreateRoleDirectedAnger_WhenTargetHasTheftMemory()
    {
        var owner = new DefinitionID("Test.Target");
        var actor = new DefinitionID("Test.Actor");
        var profile = new MemoryEmotionProfile(
            owner,
            new PersonalityProfile(),
            new Dictionary<DefinitionID, Attitude>(),
            subscribeToEvents: false);

        var memory = profile.ReceiveSyntheticMemory(
            new MemoryDefinition([MemoryTagType.Theft]),
            8,
            new DefinitionID("Locations.Test"),
            [
                new MemoryParticipant(owner, [MemoryRole.Target]),
                new MemoryParticipant(actor, [MemoryRole.Actor])
            ]);

        Assert.NotNull(memory);
        var anger = Assert.Single(memory!.EmotionModifiers.Where(modifier => modifier.Type == EmotionType.Anger));
        Assert.Equal(actor, anger.Target!.EntityId);
        Assert.True(anger.InitialAmount > 0);
    }

    [Fact]
    public void MemoryEmotionProfile_ShouldIncreaseAngerReaction_WhenAggressionIsHigher()
    {
        var owner = new DefinitionID("Test.Target");
        var participants = new List<MemoryParticipant>
        {
            new(owner, [MemoryRole.Target]),
            new(new DefinitionID("Test.Actor"), [MemoryRole.Actor])
        };
        var calmPersonality = new PersonalityProfile();
        var aggressivePersonality = new PersonalityProfile();
        aggressivePersonality.PersonalityTraits[PersonalityTrait.Aggression] = 1;
        var calmProfile = new MemoryEmotionProfile(owner, calmPersonality, new(), subscribeToEvents: false);
        var aggressiveProfile = new MemoryEmotionProfile(owner, aggressivePersonality, new(), subscribeToEvents: false);
        var definition = new MemoryDefinition([MemoryTagType.Theft]);

        var calmMemory = calmProfile.ReceiveSyntheticMemory(definition, 1, new DefinitionID("Locations.Test"), participants);
        var aggressiveMemory = aggressiveProfile.ReceiveSyntheticMemory(definition, 1, new DefinitionID("Locations.Test"), participants);

        Assert.NotNull(calmMemory);
        Assert.NotNull(aggressiveMemory);
        var calmAnger = Assert.Single(calmMemory!.EmotionModifiers.Where(modifier => modifier.Type == EmotionType.Anger));
        var aggressiveAnger = Assert.Single(aggressiveMemory!.EmotionModifiers.Where(modifier => modifier.Type == EmotionType.Anger));
        Assert.True(aggressiveAnger.InitialAmount > calmAnger.InitialAmount);
    }

    [Fact]
    public void MemoryEmotionProfile_ShouldFilterAndStrengthenMatchingMemories_WhenQueryMatchesTags()
    {
        var owner = new DefinitionID("Test.Owner");
        var profile = new MemoryEmotionProfile(owner, new PersonalityProfile(), new(), subscribeToEvents: false);
        var memory = profile.ReceiveSyntheticMemory(
            new MemoryDefinition([MemoryTagType.Theft]),
            1,
            new DefinitionID("Locations.Test"),
            [new MemoryParticipant(owner, [MemoryRole.Target])]);
        var query = new MemoryQuery { Tags = [MemoryTagType.Theft] };

        Assert.NotNull(memory);
        memory!.Strength = 0.2;
        profile.StrengthenMemories(query, 0.25);

        Assert.Single(profile.GetMemories(query));
        Assert.Equal(0.45, memory.Strength, precision: 10);
        Assert.True(profile.HasMemory(query));
    }

    [Fact]
    public void Memory_ShouldClampIntensityAndStrength_WhenAssignedOutsideValidRange()
    {
        var source = new SyntheticMemorySource(
            1,
            new MemoryDefinition([MemoryTagType.Theft]),
            [],
            new DefinitionID("Locations.Test"));
        var memory = new Memory(2, source, [], [MemoryTagType.Theft], 1, 1);

        memory.Intensity = -1;
        memory.Strength = 2;

        Assert.Equal(0, memory.Intensity);
        Assert.Equal(1, memory.Strength);
        Assert.Equal(0, memory.Influence);
    }
}