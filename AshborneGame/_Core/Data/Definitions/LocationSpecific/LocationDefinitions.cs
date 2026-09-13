using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.LocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace AshborneGame._Core.Data.Definitions.LocationSpecific
{
    public static class LocationDefinitions
    {
        public static IReadOnlyList<LocationDefinition> All { get; } = 
        [
            Prologue.PrologueStart, 
            Dreamspace.EyePlatform, Dreamspace.PlatformEdge, Dreamspace.HallOfMirrors, Dreamspace.MirrorShardSublocation,

        ];

        public static class Prologue
        {
            public static LocationDefinition PrologueStart = new(
                DefinitionIDs.Locations.Prologue.PrologueStart,
                DefinitionIDs.Scenes.Prologue,
                new LocationNameAdapter("Prologue Start", "where it all began"),
                new DescriptionComposer(
                    new LookDescription(
                        "You find yourself in a dimly lit room, the walls adorned with ancient tapestries.",
                        "The room remains unchanged, but the shadows seem to dance more vividly now."),
                    new VisitDescription(
                        "You awaken in a mysterious chamber, the air thick with anticipation.",
                        "You return to the chamber, feeling a sense of déjà vu.",
                        "Once again, you are in the chamber, the atmosphere unchanged."),
                    new SensoryDescription(
                        "The scent of old parchment fills the air.",
                        "A faint whisper echoes through the room, though no one is there."),
                    new AmbientDescription().AddTimeBased(10, "The flickering candlelight casts eerie shadows on the walls.")
                ),
                objects: [],
                customCommands: new()
            );
        }

        public static class Dreamspace
        {
            public static LocationDefinition EyePlatform = new(
                DefinitionIDs.Locations.Dreamspace.EyePlatform,
                DefinitionIDs.Scenes.OssanethsDomain,
                new LocationNameAdapter("Eye Platform", "an eye-shaped platform"),
                new DescriptionComposer(
                    new LookDescription(
                        "You glance around uneasily. The eye you stand on is unblinking and unmoving. Black clouds cover the sky, and the occasional lightning flashes are bright white against an otherwise dull and dark background.",
                        "You look around once more. Nothing changes — but are the shards sharper now?"),
                    new VisitDescription(
                        "You feel sick and disoriented. It takes you a few moments to stabilise. Glancing around, you notice that you're standing on an eye-shaped platform overlooking a vast, swirling abyss. The air is thick with an otherworldly energy as mirrors and shards of glass spin wildly around you.",
                        "You are back on the platform. The eye beneath seems stronger now, the pupil having enlarged, as though it wants to see more. The abyss feels darker, heavier.",
                        "For the fourth time, you stand overlooking the mess of glass and mirrors. You almost grow tired of it. The vortex is at its strongest now. The void is at its darkest, deepest, and the mirrors reflect your ragged face. It is unrecognisable now.",
                        "You are once again on the eye platform. It remains unchanged. The vortex belows continues swirling, and the eye continues staring."),
                    new SensoryDescription(
                        "The platform beneath is an alien stone, black and white patterns etched into every part of the eye.",
                        "It's eerily quiet despite the chaos above and below. As though the eye is remembering, and commanding everything to be silent."),
                    new AmbientDescription().AddTimeBased(35, "The glass keeps on spinning around you. The eye does not blink."),
                    ConditionalDescription.StartNew()
                        // If the player has visited the Hall of Mirrors and this is their 1st, 2nd, or 4th visit to the Eye Platform
                        .If((player, gameState) =>
                        {
                            int hallOfMirrorsVisits = gameState.GetLocationVisitCount(DefinitionIDs.Locations.Dreamspace.HallOfMirrors);
                            int currentVisits = player.CurrentLocation.VisitCount;
                            if (hallOfMirrorsVisits > 0 && 
                                (currentVisits == 1 || currentVisits == 2 || currentVisits == 4))
                            {
                                return true;
                            }
                            return false;
                        })
                        .ThenShow("The glass also seems to reflect even deeper now, each questioning your very identity.")
                        .OnlyOnce(),
                    ConditionalDescription.StartNew()
                        // If the player has visited the Hall of Mirrors and this is their 3rd or later (>4) visit to the Eye Platform
                        .If((player, gameState) =>
                        {
                            int hallOfMirrorsVisits = gameState.GetLocationVisitCount(DefinitionIDs.Locations.Dreamspace.HallOfMirrors);
                            int currentVisits = player.CurrentLocation.VisitCount;
                            if (hallOfMirrorsVisits > 0 &&
                                (currentVisits == 3 || currentVisits > 4))
                            {
                                return true;
                            }
                            return false;
                        })
                        .ThenShow("However, the glass seems to reflect even deeper into you now, each questioning your very identity.")
                        .OnlyOnce()
                    //ConditionalDescription.StartNew()
                    //    .If((player, gameState) =>
                    //    {
                    //        int templeVisits = gameState.GetLocationVisitCount(DefinitionIDs.Locations.Dreamspace.TempleOfTheBoundOne);
                    //        bool talkedToBound = gameState.TryGetFlag(StateKeys.Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOne, out bool v2) && v2;
                    //        int visits = player.CurrentLocation.VisitCount;

                    //        if (templeVisits > 0 &&
                    //            talkedToBound &&
                    //            (visits == 1 || visits == 2 || visits == 4))
                    //        {
                    //            return true;
                    //        }
                    //        return false;
                    //    })
                    //    .ThenShow("However, now the swirl almost reminds you of the Bound One — chaotic, unnerving and unpredictable. You shiver. Maybe it's best not to think about him.")
                    //    .OnlyOnce(),
                    //ConditionalDescription.StartNew()
                    //    .If((player, gameState) =>
                    //    {
                    //        int templeVisits = gameState.GetLocationVisitCount(DefinitionIDs.Locations.Dreamspace.TempleOfTheBoundOne);
                    //        bool talkedToBound = gameState.TryGetFlag(StateKeys.Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOne, out bool v2) && v2;
                    //        int visits = player.CurrentLocation.VisitCount;

                    //        if (templeVisits > 0 && talkedToBound && (visits == 3 || visits > 4))
                    //        {
                    //            return true;
                    //        }
                    //        return false;
                    //    })
                ),
                objects: [],
                customCommands: new()
            );
        
            public static LocationDefinition PlatformEdge = new(
                DefinitionIDs.Locations.Dreamspace.PlatformEdge,
                DefinitionIDs.Scenes.OssanethsDomain,
                new LocationNameAdapter("platform edge", "the platform edge", new List<string> { "edge", "side" }),
                new DescriptionComposer(
                    new LookDescription(
                        "Peering over the edge, the void stretches endlessly. Fragments warp and twist as if reality itself is bending.",
                        "You peer over the edge once more. The darkness seems thicker than before. You feel your mind resisting the pull."),
                    new VisitDescription(
                        "You walk to the edge, careful and cautious. There, the platform ends abruptly: no smooth curves or edges, just solid ground suddenly giving way to black.",
                        "You stride back to the edge. The void seems darker and deeper now...",
                        "You are once more at the edge."),
                    new SensoryDescription(
                        "The air smells sharper here, like it's... metallic.",
                        "A low hum vibrates through your chest, syncing with your heartbeat."),
                    new AmbientDescription().AddTimeBased(20, "A shadow flickers at the edge of your vision, gone when you turn.")
                ),
                new(),
                new CustomCommandHandler().AddCustomCommand(
                    new CustomCommandPhrasing(["look over", "look down", "peer over", "peer down"], ["edge", "the edge"]),
                    () => "You stare into the rift. Vertigo strikes, but the depths reveal nothing.",
                    () => { }
                )
            );

            public static LocationDefinition HallOfMirrors = new(
                DefinitionIDs.Locations.Dreamspace.HallOfMirrors,
                DefinitionIDs.Scenes.OssanethsDomain,
                new LocationNameAdapter("Hall of Mirrors", "the Hall of Mirrors", new List<string> { "hall" }),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the hall. Everywhere, your reflection stares right back at you, each mirror containing an infinite universe of you's.",
                        "You look around the hall again. The mirrors remain ever so still, ever so silent."
                    ),
                    new VisitDescription(
                        "You enter the Hall of Mirrors. In front of you is a long, stretching hallway that seems to go on forever; the wall, floor, and ceilings are covered in mirrors. As you walk by, some reflections lag behind and others move before you. " +
                        "You are surprised to see that the Mask that was forced on to you just before is no longer on your face — instead, it leaves blank, featureless skin. Your identity. Gone.",
                        "You enter the Hall of Mirrors again. Nothing seems to have changed, but you think that the reflections are diverging further and further away from your real self.",
                        "For the fourth time, you enter the Hall of Mirrors. The reflections are increasingly clearer in some mirrors, while gone in others. For the first time, there are cracked mirrors dotted along the silver-lined hallway."),
                    new SensoryDescription(),
                    new AmbientDescription().AddTimeBased(22, "You stand still. Your reflections do not.")
                ),
                [],
                new CustomCommandHandler().AddCustomCommand(
                    new CustomCommandPhrasing(["reflect", "self-reflect"], []),
                    () => "You stare at the mirrors. Your reflections are everywhere, but none of them feel like you.",
                    () => { }
                ).AddCustomCommand(new CustomCommandPhrasing(
                    ["pick up", "grab", "take", "get"],
                    ["the shard", "shard", "the mirror shard",
                        "the shard of mirror", "the piece of mirror",
                        "the mirror piece"]),
                    () => $"You cannot do that from here. Try going closer to the shard.",
                    () => { }
                )
            );

            public static LocationDefinition MirrorShardSublocation = new(
                DefinitionIDs.Locations.Dreamspace.MirrorShardSublocation,
                DefinitionIDs.Scenes.OssanethsDomain,
                new LocationNameAdapter("mirror shard", "a shard of mirror lying on the floor", new List<string> { "shard", "mirror shard", "shard of glass", "shard of mirror", "shard of a mirror" }),
                new DescriptionComposer(
                    new LookDescription("You look at the shard. It is a small piece of a broken mirror, but it seems to reflect deeper than a normal mirror. You can see your reflection, but it feels... empty.",
                        "You look at the shard again. It still feels empty, but you can't shake the feeling that it is important."),
                    new VisitDescription("You walk up to the shard of mirror. It is small and broken, but it seems to reflect deeper than a normal mirror can. Perhaps storing it for later will be beneficial.",
                        "You walk up to the shard again. It still feels empty, but you can't shake the feeling that it is important.",
                        "You go to the shard again. It still feels empty, but you can't shake the feeling that it is important."),
                    new SensoryDescription("The shard lies still on the ground.", "It is eerily quiet here.")
                ),
                [],
                new CustomCommandHandler().AddCustomCommand(
                    new CustomCommandPhrasing(["pick up the", "take the", "grab the"], ["shard", "mirror shard", "shard of glass", "shard of mirror", "shard of a mirror"]),
                    () => "You pick up the shard. It feels cold and heavy in your hand.",
                    () =>
                    {
                        GameContext.Player.Inventory.TryAddItem(DefinitionIDs.Items.Magic.MirrorShard, 1);
                        WorldBuilder.RemoveParentChildRelationship(GameContext.LocationRegistry, DefinitionIDs.Locations.Dreamspace.HallOfMirrors, DefinitionIDs.Locations.Dreamspace.MirrorShardSublocation);
                    }
                )
            );
        }

        public static class OssuaryOfEyes
        {
            public static Definition WakingChamber = new(
                DefinitionIDs.Locations.OssuaryOfEyes.WakingChamber,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Waking Chamber", "the Waking Chamber"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Waking Chamber. The walls are adorned with intricate carvings, and a soft light emanates from an unknown source.",
                        "You look around the Waking Chamber again. The carvings seem to shift subtly, as if alive."
                    ),
                    new VisitDescription(
                        "You awaken in the Waking Chamber. The air is thick with anticipation, and you feel a sense of déjà vu.",
                        "You return to the Waking Chamber. The atmosphere remains unchanged, but you feel a growing unease."
                    ),
                    new SensoryDescription(
                        "The scent of incense fills the air, mingling with the faint aroma of old parchment.",
                        "A distant hum resonates through the chamber, vibrating through your very bones."
                    ),
                    new AmbientDescription().AddTimeBased(15, "The light flickers, casting dancing shadows on the walls.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition KeepersQuarters = new(
                DefinitionIDs.Locations.OssuaryOfEyes.KeepersQuarters,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Keeper's Quarters", "the Keeper's Quarters"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Keeper's Quarters. The room is filled with ancient tomes and artifacts, each telling a story of a bygone era.",
                        "You look around the Keeper's Quarters again. The artifacts seem to whisper secrets, though you cannot understand them."
                    ),
                    new VisitDescription(
                        "You enter the Keeper's Quarters. The air is thick with history, and you feel a weight of knowledge pressing down on you.",
                        "You return to the Keeper's Quarters. The atmosphere remains heavy, and you feel a sense of foreboding."
                    ),
                    new SensoryDescription(
                        "The scent of old books and wax fills the air.",
                        "A faint scratching sound echoes through the room, as if something unseen is moving."
                    ),
                    new AmbientDescription().AddTimeBased(20, "The shadows in the room seem to shift and dance, as if alive.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition HallOfLostThoughts = new(
                DefinitionIDs.Locations.OssuaryOfEyes.HallOfLostThoughts,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Hall of Lost Thoughts", "the Hall of Lost Thoughts"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Hall of Lost Thoughts. The walls are lined with mirrors, each reflecting a different version of yourself.",
                        "You look around the Hall of Lost Thoughts again. The reflections seem to shift and change, as if they are alive."
                    ),
                    new VisitDescription(
                        "You enter the Hall of Lost Thoughts. The air is thick with confusion, and you feel a sense of disorientation.",
                        "You return to the Hall of Lost Thoughts. The atmosphere remains unsettling, and you feel a growing sense of unease."
                    ),
                    new SensoryDescription(
                        "The scent of damp stone fills the air.",
                        "A low hum resonates through the hall, vibrating through your very being."
                    ),
                    new AmbientDescription().AddTimeBased(25, "The mirrors seem to ripple and distort, as if reality itself is bending.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition CloisterGardens = new(
                DefinitionIDs.Locations.OssuaryOfEyes.CloisterGardens,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Cloister Gardens", "the Cloister Gardens"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Cloister Gardens. The gardens are lush and vibrant, filled with exotic plants and flowers.",
                        "You look around the Cloister Gardens again. The plants seem to sway and move, as if they are alive."
                    ),
                    new VisitDescription(
                        "You enter the Cloister Gardens. The air is filled with the scent of blooming flowers, and you feel a sense of peace.",
                        "You return to the Cloister Gardens. The atmosphere remains serene, but you feel a growing sense of unease."
                    ),
                    new SensoryDescription(
                        "The scent of fresh flowers fills the air.",
                        "A gentle breeze rustles through the leaves, creating a soothing sound."
                    ),
                    new AmbientDescription().AddTimeBased(30, "The sunlight filters through the trees, casting dappled shadows on the ground.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition LowerVault = new(
                DefinitionIDs.Locations.OssuaryOfEyes.LowerVault,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Lower Vault", "the Lower Vault"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Lower Vault. The room is dimly lit, filled with ancient relics and artifacts.",
                        "You look around the Lower Vault again. The relics seem to whisper secrets, though you cannot understand them."
                    ),
                    new VisitDescription(
                        "You enter the Lower Vault. The air is thick with history, and you feel a weight of knowledge pressing down on you.",
                        "You return to the Lower Vault. The atmosphere remains heavy, and you feel a sense of foreboding."
                    ),
                    new SensoryDescription(
                        "The scent of old stone and dust fills the air.",
                        "A faint scratching sound echoes through the room, as if something unseen is moving."
                    ),
                    new AmbientDescription().AddTimeBased(30, "The shadows in the room seem to shift and dance, as if alive.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition CentralOssuary = new(
                DefinitionIDs.Locations.OssuaryOfEyes.CentralOssuary,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Central Ossuary", "the Central Ossuary"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Central Ossuary. The room is filled with ancient relics and bones, each telling a story of a long-forgotten past.",
                        "You look around the Central Ossuary again. The relics seem to whisper secrets, though you cannot understand them."
                    ),
                    new VisitDescription(
                        "You enter the Central Ossuary. The air is thick with history, and you feel a weight of knowledge pressing down on you.",
                        "You return to the Central Ossuary. The atmosphere remains heavy, and you feel a sense of foreboding."
                    ),
                    new SensoryDescription(
                        "The scent of old bones and incense fills the air.",
                        "A faint scratching sound echoes through the room, as if something unseen is moving."
                    ),
                    new AmbientDescription().AddTimeBased(35, "The shadows in the room seem to shift and dance, as if alive.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition Scriptorium = new(
                DefinitionIDs.Locations.OssuaryOfEyes.Scriptorium,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Scriptorium", "the Scriptorium"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Scriptorium. The room is filled with ancient manuscripts and scrolls, each containing knowledge from a bygone era.",
                        "You look around the Scriptorium again. The manuscripts seem to whisper secrets, though you cannot understand them."
                    ),
                    new VisitDescription(
                        "You enter the Scriptorium. The air is thick with history, and you feel a weight of knowledge pressing down on you.",
                        "You return to the Scriptorium. The atmosphere remains heavy, and you feel a sense of foreboding."
                    ),
                    new SensoryDescription(
                        "The scent of old parchment fills the air.",
                        "A faint scratching sound echoes through the room, as if something unseen is moving."
                    ),
                    new AmbientDescription().AddTimeBased(40, "The shadows in the room seem to shift and dance, as if alive.")
                ),
                [],
                new CustomCommandHandler()
            );

            public static Definition Observatory = new(
                DefinitionIDs.Locations.OssuaryOfEyes.Observatory,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Observatory", "the Observatory"),
                new DescriptionComposer(
                    new LookDescription(
                        "You look around the Observatory. The room is filled with telescopes and star charts, each pointing to the heavens above.",
                        "You look around the Observatory again. The stars seem to twinkle more brightly, as if they are alive."
                    ),
                    new VisitDescription(
                        "You enter the Observatory. The air is filled with the scent of old books and the faint hum of machinery.",
                        "You return to the Observatory. The atmosphere remains serene, but you feel a growing sense of unease."
                    ),
                    new SensoryDescription(
                        "The scent of old books and metal fills the air.",
                        "A low hum resonates through the room, vibrating through your very being."
                    ),
                    new AmbientDescription().AddTimeBased(45, "The telescopes seem to shift and move, as if they are alive.")
                ),
                [],
                new CustomCommandHandler()
            );
        }
    }
}
