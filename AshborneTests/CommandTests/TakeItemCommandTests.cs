using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Globals.Enums;

namespace AshborneTests.CommandTests;

[Collection("AshborneTests")]
public class TakeItemCommandTests : IsolatedTestBase
{
    [Fact]
    public async Task TakeItemCommand_ShouldFail_WhenNoContainerOrNPCIsSelected()
    {
        var player = TestUtils.CreateTestPlayer();

        var result = await CommandManager.TryExecute("take", ["1", "test_item"], player);

        Assert.False(result);
    }

}
