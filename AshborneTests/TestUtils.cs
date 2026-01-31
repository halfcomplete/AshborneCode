using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.SceneManagement;
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

        /// <summary>
        /// Creates a test NPC with a default name "TestNPC". Has no behaviours.
        /// </summary>
        static internal NPC CreateTestNPC(string name = "TestNPC")
        {
            return new NPC(name, null);
        }

        /// <summary>
        /// Creates a test NPC with a default name "TestNPCWithInventory" and adds a TradeableNPCBehaviour to it. Optionally adds items to the NPC's inventory.
        /// </summary>
        static internal async Task<NPC> CreateTestNPCWithInventory(int itemNumber = 0)
        {
            var npc = CreateTestNPC();
            npc.AddBehaviour(typeof(IHasInventory), new TradeableNPCBehaviour());
            (_, var inv) = await npc.TryGetBehaviour<IHasInventory>();
            if (itemNumber > 0)
                inv.Inventory.AddItem(CreateTestItem(), itemNumber);
            return npc;
        }

        /// <summary>
        /// Returns a test location with a default name "Test Location" and a default description "A place for testing.". Has a minimum visibility of 1.
        /// </summary>
        static internal Location CreateTestLocation(string name = "Test Location")
        {
            var descriptor = new LocationNameAdapter(name, null);
            var narrative = new DescriptionComposer();
            return new Location(descriptor, narrative, Guid.NewGuid().ToString());
        }

        /// <summary>
        /// Returns a test sublocation with a default name "Test Sublocation" and a default location "Test Location". Has a default game object "Test Object".
        /// </summary>
        static internal Sublocation CreateTestSublocation(BOCSGameObject gameObject)
        {
            var parent = CreateTestLocation(Guid.NewGuid().ToString());
            var identifier = new LocationNameAdapter("test", null);
            return new Sublocation(
                parent,
                gameObject,
                identifier,
                new DescriptionComposer(),
                Guid.NewGuid().ToString(),
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
                inv.Inventory.AddItem(CreateTestItem(), amount);
            return chest;
        }

        static internal Item CreateTestItem(string name = "test_item", int stackLimit = 1, ItemTypes itemType = ItemTypes.None)
        {
            return new Item(name, "A test item", "A test item has been used", stackLimit, itemType, ItemQualities.None);
        }
    }
}
