using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Game.CommandHandling.Commands;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
using Moq;
using FluentAssertions;
using AshborneGame._Core.Globals.Interfaces;
using AshborneTests;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class TalkToNPCCommandTests
    {
        [Fact]
        public void TalkToNPC_Fails_When_NoSublocation()
        {
            var command = new TalkToNPCCommand();
            var player = TestUtils.CreateTestPlayer();

            var result = command.TryExecute(new List<string> { "Elias" }, player);

            result.Should().BeFalse();
        }

        [Fact]
        public void TalkToNPC_Fails_When_Object_Is_Not_NPC()
        {
            var command = new TalkToNPCCommand();
            var player = TestUtils.CreateTestPlayer();
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestGameObject());

            var result = command.TryExecute(new List<string> { "Elias" }, player);

            result.Should().BeFalse();
        }

        [Fact]
        public void TalkToNPC_Succeeds_In_Good_Conditions()
        {
            var npc = new TestNPC("Elias");
            var command = new TalkToNPCCommand();
            Sublocation testSublocation = TestUtils.CreateTestSublocation(npc);
            var player = TestUtils.CreateTestPlayer();
            player.MoveTo(testSublocation);

            var result = command.TryExecute(new List<string> { "Elias" }, player);

            result.Should().BeTrue();
            npc.WasTalkedTo.Should().BeTrue();
        }

        private class TestNPC : NPC
        {
            internal bool WasTalkedTo = false;
            internal TestNPC(string name) : base(name, null) { }
            public override async Task Talk(Player player) { WasTalkedTo = true; await Task.CompletedTask; }
        }
    }
}
