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
                new("Bound One", ["prisoner"]),
                "A mysterious figure...",
                [
                    new TalkableBehaviour(null, "Act1_Scene1_Prisoner_Dialogue"),
                ]
            );

        public static readonly BOCSObjectDefinition Dummy =
            new(
                DefinitionIDs.NPCs.Dummy,
                new("Dummy NPC", ["stupid boy", "Ricky"], "a"),
                "He's a bit slow in the head...",
                [
                    new TalkableBehaviour(null, "Act1_Scene1_Prisoner_Dialogue"),
                    new CanBeAttackedBehaviour(100),
                ]
            );
    }
}
