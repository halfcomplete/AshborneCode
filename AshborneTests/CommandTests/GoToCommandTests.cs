using AshborneGame._Core._Player;
using AshborneGame._Core.Game.CommandHandling;
using FluentAssertions;
using AshborneTests;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class GoToCommandTests : IsolatedTestBase
    {
        [Fact]
        public async Task GoToCommand_Fails_With_Invalid_Sublocation()
        {
            var player = new Player();

            bool result = await CommandManager.TryExecute("go to", ["test_sublocation"], player);

            Assert.False(result);
        }

        [Fact]
        public async Task GoToCommand_Succeeds_With_Same_Location()
        {
            var player = new Player();

            bool result = await CommandManager.TryExecute("go to", ["Test", "Location"], player);

            Assert.True(result);
        }
    }
}
