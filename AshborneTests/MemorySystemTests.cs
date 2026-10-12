using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;

namespace AshborneTests;

[Collection("AshborneTests")]
public class MemorySystemTests
{
    [Fact]
    public void ReceiveMemorySource_ShouldIgnoreSource_WhenOwnerIsNotParticipant()
    {
        var owner = new DefinitionID("Test.Owner");
        var source = new SyntheticMemorySource(
            1,
            new MemoryDefinition([MemoryTagType.Theft]),
            [new MemoryParticipant(new DefinitionID("Test.Other"), [MemoryRole.Actor])],
            new DefinitionID("Locations.Test"));
        var profile = new MemoryEmotionProfile(owner, new PersonalityProfile(), new(), subscribeToEvents: false);

        var memory = profile.ReceiveMemorySource(source);

        Assert.Null(memory);
        Assert.Empty(profile.GetMemories());
    }

    [Fact]
    public void ReceiveMemorySource_ShouldCreateMemory_WhenOwnerHasAnyParticipantRole()
    {
        var owner = new DefinitionID("Test.Owner");
        var source = new SyntheticMemorySource(
            1,
            new MemoryDefinition([MemoryTagType.Theft]),
            [new MemoryParticipant(owner, [MemoryRole.Target])],
            new DefinitionID("Locations.Test"));
        var profile = new MemoryEmotionProfile(owner, new PersonalityProfile(), new(), subscribeToEvents: false);

        var memory = profile.ReceiveMemorySource(source);

        Assert.NotNull(memory);
        Assert.Single(profile.GetMemoriesByCause(source));
        Assert.True(profile.RemembersEvent(source));
    }

    [Fact]
    public void StrengthenMemories_ShouldIncreaseMatchingStrength_WhenQueryMatchesTags()
    {
        var owner = new DefinitionID("Test.Owner");
        var profile = new MemoryEmotionProfile(owner, new PersonalityProfile(), new(), subscribeToEvents: false);
        var memory = profile.ReceiveSyntheticMemory(
            new MemoryDefinition([MemoryTagType.Theft]),
            1,
            new DefinitionID("Locations.Test"),
            [new MemoryParticipant(owner, [MemoryRole.Target])]);
        var query = new MemoryQuery { Tags = [MemoryTagType.Theft] };

        memory!.Strength = 0.2;
        profile.StrengthenMemories(query, 0.25);

        Assert.Equal(0.45, memory.Strength, precision: 10);
    }

    [Fact]
    public void ReceiveMemorySource_ShouldPreserveBoundedIntensity_WhenOwnerHasRelationshipToVictim()
    {
        var owner = new DefinitionID("Test.Owner");
        var victim = new DefinitionID("Test.Victim");
        var relationships = new Dictionary<DefinitionID, Attitude>
        {
            [victim] = new Attitude(-1, 0, 0, 0, 0, 0, 0, 0)
        };
        var source = new SyntheticMemorySource(
            1,
            new MemoryDefinition([MemoryTagType.Theft]),
            [
                new MemoryParticipant(owner, [MemoryRole.Actor]),
                new MemoryParticipant(victim, [MemoryRole.Target])
            ],
            new DefinitionID("Locations.Test"));
        var profile = new MemoryEmotionProfile(owner, new PersonalityProfile(), relationships, subscribeToEvents: false);

        var memory = profile.ReceiveMemorySource(source);

        Assert.NotNull(memory);
        Assert.InRange(memory!.Intensity, 0, 1);
    }
}
