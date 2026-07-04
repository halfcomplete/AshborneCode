using AshborneGame._Core.Data.BOCS.Behaviours;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.Data.Definitions
{
    public static class NPCDefinitions
    {
        public static readonly BOCSObjectDefinition BoundOne =
            new(
                DefinitionIDs.NPCs.BoundOne,
                "The Bound One",
                "A mysterious figure...",
                ["Prisoner"],
                [
                    new TalkableBehaviour(null, "Act1_Scene1_Prisoner_Dialogue"),
                ]
            );

        public static readonly BOCSObjectDefinition Dummy =
            new(
                DefinitionIDs.NPCs.Dummy,
                "A dummy NPC",
                "He's a bit slow in the head...",
                ["stupid boy", "Ricky"],
                [
                    new TalkableBehaviour(null, "Act1_Scene1_Prisoner_Dialogue"),
                    new CanBeAttackedBehaviour(100),
                ]
            );
    }
}
