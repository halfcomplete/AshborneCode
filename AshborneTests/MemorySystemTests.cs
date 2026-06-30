using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Game.Events;

namespace AshborneTests
{
    [Collection("AshborneTests")]
    public class MemorySystemTests : IDisposable
    {
        public MemorySystemTests()
        {
            EventBus.Clear();
        }

        public void Dispose()
        {
            EventBus.Clear();
            GC.SuppressFinalize(this);
        }

        [Fact]
        public void ReceiveMemorableEvent_ShouldIgnoreEventsWithoutTheOwnerAsParticipant()
        {
            var ownerId = Guid.NewGuid();
            var witnessId = Guid.NewGuid();
            var profile = new MemoryProfile(ownerId, new PersonalityProfile(), new Dictionary<Guid, Attitude>());

            var cause = new GameEvents.Player.StoleItemEvent(
                10,
                null!,
                null!,
                [new MemoryParticipant(witnessId, [MemoryRole.Actor])]);

            profile.ReceiveMemorableEvent(cause);

            Assert.Empty(profile.GetMemories());
        }

        [Fact]
        public void ReceiveMemorableEvent_ShouldCreateMemory_WhenOwnerIsAnyParticipantRole()
        {
            var ownerId = Guid.NewGuid();
            var profile = new MemoryProfile(ownerId, new PersonalityProfile(), new Dictionary<Guid, Attitude>());

            var cause = new GameEvents.Player.StoleItemEvent(
                10,
                null!,
                null!,
                [new MemoryParticipant(ownerId, [MemoryRole.Actor])]);

            profile.ReceiveMemorableEvent(cause);

            Assert.Single(profile.GetMemories());
        }

        // TODO: Fix this later when we decide how exactly memory strengthening should work
        // [Fact]
        // public void StrengthenMemory_ShouldIncreaseMatchingMemoryStrength()
        // {
        //     var ownerId = Guid.NewGuid();
        //     var profile = new MemoryProfile(ownerId, new PersonalityProfile(), new Dictionary<Guid, Attitude>());
        //     var memorableGameEvent = CreateMemorableCause(ownerId);

        //     profile.ReceiveMemorableEvent(memorableGameEvent);

        //     var priorStrength = profile.GetMemories().Single().Strength;

        //     profile.StrengthenMemory(memorableGameEvent, 0.1, 11);

        //     Assert.True(profile.GetMemories().Single().Strength > priorStrength);
        // }

        [Fact]
        public void ReceiveMemorableEvent_ShouldLowerIntensity_WhenWitnessDislikesVictim()
        {
            var ownerId = Guid.NewGuid();
            var targetId = Guid.NewGuid();
            var relationships = new Dictionary<Guid, Attitude>
            {
                [targetId] = new Attitude { Affection = -1, Trust = -1, Respect = -1, Fear = 0, Dominance = 0 }
            };

            var profile = new MemoryProfile(ownerId, new PersonalityProfile(), relationships);
            var cause = new GameEvents.Player.StoleItemEvent(
                10,
                null!,
                null!,
                [
                    new MemoryParticipant(ownerId, [MemoryRole.Witness]),
                    new MemoryParticipant(targetId, [MemoryRole.Target])
                ]);

            profile.ReceiveMemorableEvent(cause);

            Assert.Single(profile.GetMemories());
            Assert.True(profile.GetMemories().Single().Intensity > 0.4);
        }

        private static IMemorableGameEvent CreateMemorableCause(Guid ownerId)
        {
            return new GameEvents.Player.StoleItemEvent(
                10,
                null!,
                null!,
                [new MemoryParticipant(ownerId, [MemoryRole.Witness])]);
        }
    }
}