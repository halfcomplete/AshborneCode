using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Globals.Enums;

namespace AshborneTests.CommandTests;

[Collection("AshborneTests")]
public class GiveItemCommandTests : IsolatedTestBase
{
    [Fact]
    public async Task GiveItemCommand_ShouldFail_WhenNoContainerOrNPCIsSelected()
    {
        var player = TestUtils.CreateTestPlayer();
        player.Inventory.TryAddItem(TestUtils.CreateTestItem());

        var result = await CommandManager.TryExecute("give", ["1", "test_item"], player);

        Assert.False(result);
    }

}
