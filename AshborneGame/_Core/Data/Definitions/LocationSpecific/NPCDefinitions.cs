using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.Definitions.BOCSSpecific;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.CognitiveSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;

namespace AshborneGame._Core.Data.Definitions.LocationSpecific
{
    public static class NPCDefinitions
    {
        public static readonly BOCSObjectDefinition BoundOne =
            new(
                DefinitionIDs.NPCs.BoundOne,
                new("Bound One", "a mysterious bound figure", ["prisoner"]),
                "A mysterious figure...",
                [
                    new TalkableBehaviour(null, "Act1_Scene1_Prisoner_Dialogue"),
                ]
            );

        public static readonly BOCSObjectDefinition Dummy =
            new(
                DefinitionIDs.NPCs.Dummy,
                new("Dummy NPC", "a simple dummy", ["stupid boy", "Ricky"]),
                "He's a bit slow in the head...",
                [
                    new TalkableBehaviour(null, "Act1_Scene1_Prisoner_Dialogue"),
                    new CanBeAttackedBehaviour(100),
                ]
            );

        private static PersonalityProfile GardenerPersonality =
            new(
                new()
                {
                    { PersonalityTrait.Compassion, 0.6 },
                    { PersonalityTrait.Aggression, 0.4 },
                    { PersonalityTrait.Curiosity, 0.5 }
                }
            );

        public static readonly BOCSObjectDefinition Gardener =
            new(
                DefinitionIDs.NPCs.Gardener,
                new("Gardener", "a gardener tending to the Cloister Garden", ["gardener"]),
                "She's a hardworking individual.",
                [
                    new CognitiveBehaviour(
                        new PsychologicalState(
                            new(),
                            new MemoryEmotionProfile(DefinitionIDs.NPCs.Gardener, GardenerPersonality, new()),
                            GardenerPersonality
                        )
                    )
                ]
            );

        private static PersonalityProfile KeeperPersonality =
            new(
                new()
                {
                    { PersonalityTrait.Compassion, 0.5 },
                    { PersonalityTrait.Aggression, 0.35 },
                    { PersonalityTrait.Curiosity, 0.7 }
                }
            );

        public static readonly BOCSObjectDefinition Keeper =
            new(
                DefinitionIDs.NPCs.Keeper,
                new("Keeper", "a keeper of the Ossuary", ["keeper"]),
                "He's a groundskeeper of this sacred place.",
                [
                    new CognitiveBehaviour(
                        new PsychologicalState(
                            new(),
                            new MemoryEmotionProfile(DefinitionIDs.NPCs.Keeper, KeeperPersonality, new()),
                            KeeperPersonality
                        )
                    )
                ]
            );
    }
}
