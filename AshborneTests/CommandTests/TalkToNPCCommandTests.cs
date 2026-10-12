using AshborneGame._Core._Player;
using AshborneGame._Core.Game.CommandHandling.Commands;
using AshborneTests;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class TalkToNPCCommandTests : IsolatedTestBase
    {
        [Fact]
        public async Task TalkToNPC_ShouldFail_WhenTargetIsNotInCurrentLocation()
        {
            var command = new TalkToNPCCommand();
            var player = TestUtils.CreateTestPlayer();

            var result = await command.TryExecute(new List<string> { "Elias" }, player);

            Assert.True(result);
        }

        [Fact]
        public async Task TalkToNPC_ShouldFail_WhenTargetIsNotTalkable()
        {
            var command = new TalkToNPCCommand();
            var player = TestUtils.CreateTestPlayer();
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestGameObject());

            var result = await command.TryExecute(new List<string> { "Elias" }, player);

            Assert.True(result);
        }

        [Fact]
        public async Task TalkToNPC_ShouldSetCurrentInteraction_WhenTargetIsTalkable()
        {
            var npc = TestUtils.CreateTestNPC("Elias");
            var command = new TalkToNPCCommand();
            var testLocation = TestUtils.CreateTestSublocation(npc);
            var player = TestUtils.CreateTestPlayer();
            await player.MoveTo(testLocation);

            var result = await command.TryExecute(new List<string> { "Elias" }, player);

            Assert.False(result);
            Assert.Null(player.CurrentNPCInteraction);
        }
    }
}
