using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Data.Definitions.BOCSSpecific;
using AshborneGame._Core.Data.IDSystem;

namespace AshborneTests
{
    internal static class TestDefinitionIDs
    {
        public static readonly DefinitionID TestNPC = new("Test_NPC");
        public static readonly DefinitionID TestTrader = new("Test_Trader");
        public static readonly DefinitionID TestSword = new("Test_Sword");
        public static readonly DefinitionID TestChest = new("Test_Chest");
        public static readonly DefinitionID TestLocation = new("Test_Location");
    }

    internal static class TestDefinitions
    {
        public static BOCSObjectDefinition TestNPC = 
            new(
                TestDefinitionIDs.TestNPC,
                "Test NPC",
                "A test NPC for testing purposes",
                ["Ricky"],
                []
            );
        
        public static BOCSObjectDefinition TestTrader = 
            new(
                TestDefinitionIDs.TestNPC,
                "Test Trader",
                "A test Trader for testing purposes",
                ["Mr. Market"],
                [new TradeableNPCBehaviour()]
            );

        public static BOCSObjectDefinition TestSword = 
            new(
                TestDefinitionIDs.TestSword,
                "Test Sword",
                "A test sword for testing purposes",
                ["Cloud"],
                [new StorableBehaviour(1, AshborneGame._Core.Globals.Enums.ItemTypes.Weapon, AshborneGame._Core.Globals.Enums.ItemQualities.Uncommon)]
            );
        
        public static BOCSObjectDefinition TestChest = 
            new(
                TestDefinitionIDs.TestChest,
                "Test Chest",
                "A test chest for testing purposes",
                ["Pandora's Box"],
                [new OpenCloseBehaviour(false)]
            );
    }
}