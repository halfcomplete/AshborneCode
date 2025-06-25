using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Game.CommandHandling.Commands;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
using Moq;
using FluentAssertions;
using AshborneGame._Core.Globals.Interfaces;
using AshborneTests;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class TakeItemCommandTests
    {
        [Fact]
        internal void TakeItem_Fails_When_No_Container_Is_Opened()
        {
            var player = TestUtils.CreateTestPlayer();
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestGameObject());
            player.MoveTo(testSublocation);

            bool result = CommandManager.TryExecute("take", ["1", "torch"], player);

            Assert.False(result);
        }

        [Fact]
        internal void TakeItem_Fails_When_No_NPC_Is_Interacted_With()
        {
            var player = TestUtils.CreateTestPlayer();
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestNPC());
            player.MoveTo(testSublocation);

            bool result = CommandManager.TryExecute("take", ["1", "torch"], player);

            Assert.False(result);
        }

        [Fact]
        internal void TakeItem_Fails_When_NPC_Is_Not_Tradeable_With()
        {
            var player = TestUtils.CreateTestPlayer();
            var npc = TestUtils.CreateTestNPC();
            var testSublocation = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation);
            player.CurrentNPCInteraction = npc;

            bool result = CommandManager.TryExecute("take", ["1", "torch"], player);

            Assert.False(result);
        }

        [Fact]
        internal void TakeItem_Fails_When_Item_Does_Not_Exist()
        {
            var player = TestUtils.CreateTestPlayer();
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestGameObjectChest());
            player.MoveTo(testSublocation);

            bool result = CommandManager.TryExecute("take", ["1", "torch"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["1", "torch"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal void TakeItem_Fails_With_Negative_Amount()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest(true);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.TryGetBehaviour<IInteractable>(out IInteractable openClose);
            openClose.Interact(ObjectInteractionTypes.Open, player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);
            hasInventory.Inventory.AddItem(TestUtils.CreateTestItem());

            bool result = CommandManager.TryExecute("take", ["-1", "test_item"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["-1", "test_item"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal void TakeItem_Fails_With_Zero_Amount()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest(false);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.TryGetBehaviour<IInteractable>(out IInteractable openClose);
            openClose.Interact(ObjectInteractionTypes.Open, player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);
            hasInventory.Inventory.AddItem(TestUtils.CreateTestItem());

            bool result = CommandManager.TryExecute("take", ["0", "test_item"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["0", "test_item"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal void TakeItem_Fails_With_No_Provided_Quantity_And_Invalid_ItemName()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest();
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.TryGetBehaviour<IInteractable>(out IInteractable openClose);
            openClose.Interact(ObjectInteractionTypes.Open, player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);
            hasInventory.Inventory.AddItem(TestUtils.CreateTestItem());

            bool result = CommandManager.TryExecute("take", ["item_test"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["item_test"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal void TakeAllItem_Fails_When_Item_Does_Not_Exist()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest(true);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.TryGetBehaviour<IInteractable>(out var openClose);
            openClose.Interact(ObjectInteractionTypes.Open, player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);
            hasInventory.Inventory.AddItem(TestUtils.CreateTestItem());

            bool result = CommandManager.TryExecute("take", ["1", "item_test"], player);

            var npc = TestUtils.CreateTestNPCWithInventory(1);
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["1", "item_test"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal void TakeAllItem_Succeeds_In_Good_Conditions()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest(true, 2);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.GetAllBehaviours<IInteractable>().Where(s => s.GetType() == typeof(OpenCloseBehaviour)).ToList()[0].Interact(ObjectInteractionTypes.Open, player);

            bool result = CommandManager.TryExecute("take", ["all", "test_item"], player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);

            Assert.True(result);
            hasInventory.Inventory.Slots.Count().Should().Be(0);
            player.Inventory.Slots.Sum(s => s.Quantity).Should().Be(2);

            player.OpenedInventory = null;
            var npc = TestUtils.CreateTestNPCWithInventory(2);
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["all", "test_item"], player);
            npc.TryGetBehaviour<IHasInventory>(out var npcHasInventory);

            Assert.True(result);
            npcHasInventory.Inventory.Slots.Count().Should().Be(0);
            player.Inventory.Slots.Sum(s => s.Quantity).Should().Be(4);
        }

        [Fact]
        internal void TakeItem_Succeeds_In_Good_Conditions()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest(true);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.TryGetBehaviour<IInteractable>(out var openClose);
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = CommandManager.TryExecute("take", ["1", "test_item"], player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);

            Assert.True(result);
            hasInventory.Inventory.Slots.Count().Should().Be(0);
            player.Inventory.Slots.Sum(s => s.Quantity).Should().Be(1);

            player.OpenedInventory = null;
            var npc = TestUtils.CreateTestNPCWithInventory(1);
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["1", "test_item"], player);
            npc.TryGetBehaviour<IHasInventory>(out var npcHasInventory);

            Assert.True(result2);
            npcHasInventory.Inventory.Slots.Count().Should().Be(0);
            player.Inventory.Slots.Sum(s => s.Quantity).Should().Be(2);
        }

        [Fact]
        internal void TakeItem_Succeeds_With_No_Provided_Quantity_But_Valid_ItemName()
        {
            var player = TestUtils.CreateTestPlayer();
            var chest = TestUtils.CreateTestGameObjectChest(true);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.TryGetBehaviour<IInteractable>(out var openClose);
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = CommandManager.TryExecute("take", ["test_item"], player);
            chest.TryGetBehaviour<IHasInventory>(out var hasInventory);

            Assert.True(result);
            hasInventory.Inventory.Slots.Count().Should().Be(0);
            player.Inventory.Slots.Sum(s => s.Quantity).Should().Be(1);

            player.OpenedInventory = null;
            var npc = TestUtils.CreateTestNPCWithInventory(1);
            npc.AddBehaviour(typeof(IHasInventory), new TradeableNPCBehaviour());
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = CommandManager.TryExecute("take", ["test_item"], player);
            npc.TryGetBehaviour<IHasInventory>(out var npcHasInventory);

            Assert.True(result2);
            npcHasInventory.Inventory.Slots.Count().Should().Be(0);
            player.Inventory.Slots.Sum(s => s.Quantity).Should().Be(2);
        }
    }
}
