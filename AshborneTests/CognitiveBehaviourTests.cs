using AshborneGame._Core.CognitiveSystem;
using AshborneGame._Core.CognitiveSystem.AttitudeSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions.LocationSpecific;
using AshborneGame._Core.Data.Definitions.Registries;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneTests;

[Collection("AshborneTests")]
public class CognitiveBehaviourTests
{
    [Fact]
    public void DeepClone_DoesNotRequireAnAttachedOwner()
    {
        var behaviour = Assert.IsType<CognitiveBehaviour>(
            Gardener.BehaviourPrototypes.Single());

        var clone = behaviour.DeepClone();

        Assert.IsType<CognitiveBehaviour>(clone);
        Assert.Null(behaviour.Owner);
    }

    [Fact]
    public void SaveLoadContext_RegisterStoresBehaviourSaveData()
    {
        var context = new SaveLoadContext(new InstanceRegistry(), new LocationRegistry());
        var instanceId = InstanceID.New();
        var bocsObject = new BOCSObject(
            new ObjectNameAdapter("Test object", null),
            "A test object.",
            new DefinitionID("Test.Object"),
            instanceId);
        var behaviours = new List<BehaviourSaveData>();

        context.Register(bocsObject, behaviours);

        Assert.True(context.TryGetBehaviourSaveData(instanceId, out var savedBehaviours));
        Assert.Same(behaviours, savedBehaviours);
    }

    [Fact]
    public void CognitiveBehaviour_SaveDataRoundTripsRelationshipsAndMemories()
    {
        var gardenerId = new DefinitionID("NPCs.Gardener");
        var playerId = new DefinitionID("Player");
        var state = new PsychologicalState(gardenerId);
        state.AddRelationship(playerId, new Attitude(0.8, 0.7, 0.6, 0.2, 0.3, 0.5, 0.4, 0.5));
        state.Personality.PersonalityTraits[PersonalityTrait.Compassion] = 0.6;

        var memory = state.MemoryEmotion.ReceiveSyntheticMemory(
            new MemoryDefinition(new HashSet<MemoryTagType> { MemoryTagType.Theft }),
            1,
            new DefinitionID("Locations.Test"),
            new List<MemoryParticipant>
            {
                new(gardenerId, new List<MemoryRole> { MemoryRole.Target }),
                new(playerId, new List<MemoryRole> { MemoryRole.Actor })
            });

        Assert.NotNull(memory);

        var behaviour = new CognitiveBehaviour(state);
        var context = new SaveLoadContext(new InstanceRegistry(), new LocationRegistry());
        var saveData = behaviour.GetSaveData(context);
        var restored = new CognitiveBehaviour(new PsychologicalState(gardenerId));

        restored.LoadSaveData(saveData);

        Assert.True(restored.PsychologicalState.Relationships.TryGetValue(playerId, out var restoredAttitude));
        Assert.Equal(0.8, restoredAttitude!.Factors[AttitudeFactor.Affection]);
        Assert.Equal(0.6, restored.PsychologicalState.Personality.PersonalityTraits[PersonalityTrait.Compassion]);
        Assert.Single(restored.PsychologicalState.MemoryEmotion.GetMemories());
    }

    [Fact]
    public void LoadingCognitiveBehaviour_RegistersSentientEntityModule()
    {
        var gardenerId = new DefinitionID("NPCs.Gardener");
        var context = new SaveLoadContext(new InstanceRegistry(), new LocationRegistry());
        var sourceBehaviour = new CognitiveBehaviour(new PsychologicalState(gardenerId));
        var saveData = sourceBehaviour.GetSaveData(context);
        var loadedObject = new BOCSObject(
            new ObjectNameAdapter("Gardener", null),
            "A gardener.",
            gardenerId,
            InstanceID.New());

        BOCSObject.LoadBehavioursIntoObject(loadedObject, [saveData], context);

        Assert.True(loadedObject.HasBehaviours<ISentientEntity>());
        Assert.NotNull(loadedObject.GetAllBehavioursOfType<ISentientEntity>().Single());
    }
}
