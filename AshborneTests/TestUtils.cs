using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.LocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneTests
{
    static internal class TestUtils
    {
        /// <summary>
        /// Creates a test player with a default name "TestPlayer" and a default location "Test Location". Has a null sublocation and an empty inventory.
        /// </summary>
        static internal Player CreateTestPlayer(string name = "TestPlayer", Location? location = null)
        {
            if (location == null)
            {
                location = TestUtils.CreateTestLocation();
            }
            var player = new Player(name, location);
            return player;
        }
        // TODO: fix
        /// <summary>
        /// Creates a test NPC with a default name "TestNPC". Has no behaviours.
        /// </summary>
        static internal BOCSObject CreateTestNPC(string name = "TestNPC")
        {
            var npc = new BOCSObject(
                new ObjectNameAdapter(name, name, []),
                "A test NPC.",
                new DefinitionID("test-npc-" + Guid.NewGuid().ToString("N")));
            npc.AddBehaviour(typeof(TalkableBehaviour).GetInterfaces().Single(), new TalkableBehaviour($"Hello from {name}."));
            return npc;
        }

        /// <summary>
        /// Creates a test NPC with a default name "TestNPCWithInventory" and adds a TradeableNPCBehaviour to it. Optionally adds items to the NPC's inventory.
        /// </summary>
        static internal async Task<BOCSObject> CreateTestNPCWithInventory(int itemNumber = 0)
        {
            var npc = CreateTestNPC();
            npc.AddBehaviour(typeof(IHasInventory), new TradeableNPCBehaviour());
            (_, var inv) = await npc.TryGetBehaviour<IHasInventory>();
            if (itemNumber > 0)
                inv.Inventory.TryAddItem(CreateTestItem(), itemNumber);
            return npc;
        }

        /// <summary>
        /// Returns a test location with a default name "Test Location" and a default description "A place for testing.". Has a minimum visibility of 1.
        /// </summary>
        static internal Location CreateTestLocation(string name = "Test Location")
        {
            var descriptor = new LocationNameAdapter(name, null);
            var narrative = new DescriptionComposer();
            // Pass explicit ID to ensure uniqueness in tests
            return new Location(
                new DefinitionID("locations.test-" + Guid.NewGuid().ToString("N")[..8]),
                descriptor,
                narrative,
                new(),
                new(),
                new(),
                new());
        }

        // TODO: hack
        /// <summary>
        /// Returns a test sublocation with a default name "Test Sublocation" and a default location "Test Location". Has a default game object "Test Object".
        /// </summary>
        static internal Location CreateTestSublocation(BOCSObject gameObject)
        {
            var location = CreateTestLocation(Guid.NewGuid().ToString());
            location.AddObject(gameObject);
            return location;
        }

        static internal BOCSObject CreateTestGameObject(string name = "Test Object")
        {
            return new BOCSObject(
                new ObjectNameAdapter(name, name, []),
                "A test object.",
                new DefinitionID("test-object-" + Guid.NewGuid().ToString("N")));
        }

        static internal async Task<BOCSObject> CreateTestGameObjectChest(bool hasItem = false, int amount = 1)
        {
            var chest = new BOCSObject(
                new ObjectNameAdapter("Test chest", "Test chest", []),
                "A test chest.",
                new DefinitionID("test-chest-" + Guid.NewGuid().ToString("N")));
            chest.AddBehaviour(typeof(IHasInventory), new ContainerBehaviour());
            chest.AddBehaviour(typeof(IInteractable), new OpenCloseBehaviour(false));
            (_, var inv) = await chest.TryGetBehaviour<IHasInventory>();
            if (hasItem)
                inv.Inventory.TryAddItem(CreateTestItem(), amount);
            return chest;
        }

        static internal BOCSObject CreateTestItem(string name = "test_item", int stackLimit = 1, ItemTypes itemType = ItemTypes.None)
        {
            var item = new BOCSObject(
                new ObjectNameAdapter(name, name, []),
                "A test item.",
                new DefinitionID("test-item-" + Guid.NewGuid().ToString("N")));
            item.AddBehaviour(typeof(IStorable), new StorableBehaviour(stackLimit, itemType, ItemQualities.None));
            return item;
        }
    }

    internal static class InventoryTestExtensions
    {
        internal static bool TryAddItem(this Inventory inventory, BOCSObject item, int amount)
        {
            var added = true;
            for (var index = 0; index < amount; index++)
            {
                added &= inventory.TryAddItem(index == 0 ? item : TestUtils.CreateTestItem(item.Name.ReferenceName));
            }
            return added;
        }
    }
}
