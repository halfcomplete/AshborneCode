using AshborneGame._Core.CognitiveSystem;
using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.Definitions.Registries;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneTests;

[Collection("AshborneTests")]
public class BocsAndCloneTests
{
    [Fact]
    public void PsychologicalState_DeepClone_ShouldCopyRelationshipsPersonalityAndMemories_WhenSourceIsPopulated()
    {
        var owner = new DefinitionID("Test.Owner");
        var target = new DefinitionID("Test.Target");
        var source = new PsychologicalState(owner);
        source.AddRelationship(target, new Attitude(0.8, 0.7, 0.6, 0.2, 0.3, 0.5, 0.4, 0.5));
        source.Personality.PersonalityTraits[PersonalityTrait.Compassion] = 0.9;
        source.MemoryEmotion.ReceiveSyntheticMemory(
            new MemoryDefinition([MemoryTagType.Theft]),
            4,
            new DefinitionID("Locations.Test"),
            [new MemoryParticipant(owner, [MemoryRole.Actor])]);

        var clone = source.DeepClone();

        Assert.NotSame(source.Relationships, clone.Relationships);
        Assert.NotSame(source.Personality, clone.Personality);
        Assert.NotSame(source.MemoryEmotion, clone.MemoryEmotion);
        Assert.Single(clone.MemoryEmotion.GetMemories());
        Assert.Equal(0.9, clone.Personality.PersonalityTraits[PersonalityTrait.Compassion]);
        Assert.Equal(0.8, clone.Relationships[target].Factors[AttitudeFactor.Affection]);

        clone.Personality.PersonalityTraits[PersonalityTrait.Compassion] = 0.1;
        Assert.Equal(0.9, source.Personality.PersonalityTraits[PersonalityTrait.Compassion]);
    }

    [Fact]
    public async Task BocsFactory_Clone_ShouldCreateIndependentBehaviourInstances_WhenSourceHasBehaviours()
    {
        var source = new BOCSObject(
            new ObjectNameAdapter("Source", "Source", []),
            "A source object.",
            new DefinitionID("Test.Source"));
        source.AddBehaviour(typeof(TalkableBehaviour).GetInterfaces().Single(), new TalkableBehaviour("Hello"));
        var factory = new BOCSFactory(new DefinitionRegistry(), new InstanceRegistry());

        var clone = await factory.Clone(source);

        var sourceBehaviour = source.ByBehaviour.OfType<TalkableBehaviour>().Single();
        var cloneBehaviour = clone.ByBehaviour.OfType<TalkableBehaviour>().Single();
        Assert.NotSame(sourceBehaviour, cloneBehaviour);
        Assert.Same(clone, ((TalkableBehaviour)cloneBehaviour).Owner);
        Assert.NotEqual(source.InstanceID, clone.InstanceID);
    }

    [Fact]
    public void BocsObject_SaveData_ShouldRoundTripIdentityAndDescription_WhenObjectHasNoBehaviours()
    {
        var source = new BOCSObject(
            new ObjectNameAdapter("Saved object", "a saved object", []),
            "Persisted description.",
            new DefinitionID("Test.Saved"));
        var context = new SaveLoadContext(new InstanceRegistry(), new LocationRegistry());

        var restored = BOCSObject.LoadFromSaveData(source.GetSaveData(context), context);

        Assert.Equal(source.InstanceID, restored.InstanceID);
        Assert.Equal(source.DefinitionID, restored.DefinitionID);
        Assert.Equal(source.Description, restored.Description);
        Assert.Equal(source.Name.DisplayName, restored.Name.DisplayName);
    }
}