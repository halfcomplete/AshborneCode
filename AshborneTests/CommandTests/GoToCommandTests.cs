using AshborneGame._Core._Player;
using AshborneGame._Core.Game.CommandHandling;
using FluentAssertions;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class GoToCommandTests
    {
        [Fact]
        public void GoToCommand_Fails_With_Invalid_Sublocation()
        {
            var player = new Player();

            bool result = CommandManager.TryExecute("go to", ["test_sublocation"], player);

            Assert.False(result);
        }

        [Fact]
        public void GoToCommand_Succeeds_With_Same_Location()
        {
            var player = new Player();

            bool result = CommandManager.TryExecute("go to", ["Test", "Location"], player);

            Assert.True(result);
        }
    }
}
