using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
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
            return null;
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
            return new Location(descriptor, narrative, new DefinitionID("locations.test-" + Guid.NewGuid().ToString("N")[..8]));
        }

        // TODO: hack
        /// <summary>
        /// Returns a test sublocation with a default name "Test Sublocation" and a default location "Test Location". Has a default game object "Test Object".
        /// </summary>
        static internal Sublocation CreateTestSublocation(BOCSObject gameObject)
        {
            var parent = CreateTestLocation(Guid.NewGuid().ToString());
            var identifier = new LocationNameAdapter("test", null);
            return new Sublocation(
                parent,
                gameObject,
                identifier,
                new DescriptionComposer(),
                new DefinitionID("test-sublocatioon"),
                "[shortened positional phrase]",
                "[short ref desc]"
            );
        }

        static internal GameObject CreateTestGameObject(string name = "Test Object")
        {
            return new GameObject(name, "A test object.");
        }

        static internal async Task<GameObject> CreateTestGameObjectChest(bool hasItem = false, int amount = 1)
        {
            GameObject chest = GameObjectFactory.CreateChest("Test chest", "A test chest");
            (_, var inv) = await chest.TryGetBehaviour<IHasInventory>();
            if (hasItem)
                inv.Inventory.TryAddItem(CreateTestItem(), amount);
            return chest;
        }

        static internal Item CreateTestItem(string name = "test_item", int stackLimit = 1, ItemTypes itemType = ItemTypes.None)
        {
            return new Item(name, "A test item", "A test item has been used", stackLimit, itemType, ItemQualities.None);
        }
    }
}
