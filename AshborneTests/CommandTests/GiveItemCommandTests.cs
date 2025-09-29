using FluentAssertions;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;

namespace AshborneTests.CommandTests
{
    [Collection("AshborneTests")]
    public class GiveItemCommandTests
    {
        [Fact]
        internal async void GiveItem_Fails_When_No_Container_Is_Opened()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem());
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestGameObject());
            player.MoveTo(testSublocation);

            bool result = await CommandManager.TryExecute("give", ["1", "torch"], player);

            Assert.False(result);
        }

        [Fact]
        internal async void GiveItem_Fails_When_No_NPC_Is_Interacted_With()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem());
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestNPCWithInventory());
            player.MoveTo(testSublocation);

            bool result = await CommandManager.TryExecute("give", ["1", "torch"], player);

            Assert.False(result);
        }

        [Fact]
        internal async void GiveItem_Fails_When_NPC_Is_Not_Tradeable_With()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem());
            var npc = TestUtils.CreateTestNPC();
            var testSublocation = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation);
            player.CurrentNPCInteraction = npc;

            bool result = await CommandManager.TryExecute("give", ["1", "torch"], player);

            Assert.False(result);
        }

        [Fact]
        internal async void GiveItem_Fails_When_Item_Does_Not_Exist()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            var testSublocation = TestUtils.CreateTestSublocation(TestUtils.CreateTestGameObject());
            player.MoveTo(testSublocation);

            bool result = await CommandManager.TryExecute("give", ["1", "torch"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["1", "torch"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal async void GiveItem_Fails_With_Negative_Amount()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            var chest = TestUtils.CreateTestGameObjectChest(true);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            (_, IInteractable openClose) = await chest.TryGetBehaviour<IInteractable>();
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["-1", "test_item"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["-1", "test_item"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal async void GiveItem_Fails_With_Zero_Amount()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem());
            var chest = TestUtils.CreateTestGameObjectChest(false);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            (_, IInteractable openClose) = await chest.TryGetBehaviour<IInteractable>();
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["0", "test_item"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["0", "test_item"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal async void GiveItem_Fails_With_No_Provided_Quantity_And_Invalid_ItemName()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem());
            var chest = TestUtils.CreateTestGameObjectChest();
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            (_, IInteractable openClose) = await chest.TryGetBehaviour<IInteractable>();
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["item_test"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["item_test"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal async void GiveAllItem_Fails_When_Item_Does_Not_Exist()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            var chest = TestUtils.CreateTestGameObjectChest();
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            (_, IInteractable openClose) = await chest.TryGetBehaviour<IInteractable>();
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["1", "item_test"], player);

            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["1", "item_test"], player);

            Assert.False(result);
            Assert.False(result2);
        }

        [Fact]
        internal async void GiveAllItem_Succeeds_In_Good_Conditions()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            var chest = TestUtils.CreateTestGameObjectChest(false);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            chest.GetAllBehaviours<IInteractable>().Where(s => s.GetType() == typeof(OpenCloseBehaviour)).ToList()[0].Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["all", "test_item"], player);
            (_, var hasInventory) = await chest.TryGetBehaviour<IHasInventory>();

            Assert.True(result);
            hasInventory.Inventory.Slots.Sum(s => s.Quantity).Should().Be(2);
            player.Inventory.Slots.Count().Should().Be(0);

            player.OpenedInventory = null;
            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["all", "test_item"], player);
            (_, var npcHasInventory) = await npc.TryGetBehaviour<IHasInventory>();

            Assert.True(result2);
            npcHasInventory.Inventory.Slots.Sum(s => s.Quantity).Should().Be(2);
            player.Inventory.Slots.Count().Should().Be(0);
        }

        [Fact]
        internal async void GiveItem_Succeeds_In_Good_Conditions()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            var chest = TestUtils.CreateTestGameObjectChest(false);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            (_, IInteractable openClose) = await chest.TryGetBehaviour<IInteractable>();
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["1", "test_item"], player);
            (_, var hasInventory) = await chest.TryGetBehaviour<IHasInventory>();

            Assert.True(result);
            hasInventory.Inventory.Slots.Sum(s => s.Quantity).Should().Be(1);
            player.Inventory.Slots.Count().Should().Be(1);

            player.OpenedInventory = null;
            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["1", "test_item"], player);
            (_, var npcHasInventory) = await npc.TryGetBehaviour<IHasInventory>();

            Assert.True(result2);
            npcHasInventory.Inventory.Slots.Sum(s => s.Quantity).Should().Be(1);
            player.Inventory.Slots.Count().Should().Be(0);
        }

        [Fact]
        internal async void GiveItem_Succeeds_With_No_Provided_Quantity_But_Valid_ItemName()
        {
            var player = TestUtils.CreateTestPlayer();
            player.Inventory.AddItem(TestUtils.CreateTestItem(), 2);
            var chest = TestUtils.CreateTestGameObjectChest(false);
            var testSublocation = TestUtils.CreateTestSublocation(chest);
            player.MoveTo(testSublocation);
            (_, IInteractable openClose) = await chest.TryGetBehaviour<IInteractable>();
            openClose.Interact(ObjectInteractionTypes.Open, player);

            bool result = await CommandManager.TryExecute("give", ["test_item"], player);
            (_, var hasInventory) = await chest.TryGetBehaviour<IHasInventory>();
            
            Assert.True(result);
            hasInventory.Inventory.Slots.Sum(s => s.Quantity).Should().Be(1);
            player.Inventory.Slots.Count().Should().Be(1);

            player.OpenedInventory = null;
            var npc = TestUtils.CreateTestNPCWithInventory();
            var testSublocation2 = TestUtils.CreateTestSublocation(npc);
            player.MoveTo(testSublocation2);
            player.CurrentNPCInteraction = npc;

            bool result2 = await CommandManager.TryExecute("give", ["test_item"], player);
            (_, var npcHasInventory) = await npc.TryGetBehaviour<IHasInventory>();

            Assert.True(result2);
            npcHasInventory.Inventory.Slots.Sum(s => s.Quantity).Should().Be(1);
            player.Inventory.Slots.Count().Should().Be(0);
        }
    }
}
