using AshborneGame._Core.CognitiveSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.Events;

namespace AshborneTests;

[Collection("AshborneTests")]
public class PsychologicalStateTests
{
    [Fact]
    public void LoadingStatePreservesRelationshipUpdatesFromMemoryProfile()
    {
        DefinitionID gardenerID = new("NPCs.Gardener");
        DefinitionID playerID = new("Player");
        var state = new PsychologicalState(gardenerID);

        state.LoadSaveData(state.GetSaveData());

        state.MemoryEmotion.ReceiveSyntheticMemory(
            new MemoryDefinition(new HashSet<MemoryTagType> { MemoryTagType.Theft }),
            1,
            new DefinitionID("Locations.Test"),
            [
                new MemoryParticipant(gardenerID, [MemoryRole.Target]),
                new MemoryParticipant(playerID, [MemoryRole.Actor])
            ]);

        Assert.True(state.Relationships.ContainsKey(playerID));
    }
}