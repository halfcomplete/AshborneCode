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
                new("Test NPC", "a test NPC", ["Ricky"]),
                "A test NPC for testing purposes",
                []
            );
        
        public static BOCSObjectDefinition TestTrader = 
            new(
                TestDefinitionIDs.TestNPC,
                new("Test Trader", "a test trader", ["Mr. Market"]),
                "A test Trader for testing purposes",
                [new TradeableNPCBehaviour()]
            );

        public static BOCSObjectDefinition TestSword = 
            new(
                TestDefinitionIDs.TestSword,
                new("Test Sword", "a test sword", ["Cloud"]),
                "A test sword for testing purposes",
                [new StorableBehaviour(1, AshborneGame._Core.Globals.Enums.ItemTypes.Weapon, AshborneGame._Core.Globals.Enums.ItemQualities.Uncommon)]
            );
        
        public static BOCSObjectDefinition TestChest = 
            new(
                TestDefinitionIDs.TestChest,
                new("Test Chest", "a test chest", ["Pandora's Box"]),
                "A test chest for testing purposes",
                [new OpenCloseBehaviour(false)]
            );
    }
}