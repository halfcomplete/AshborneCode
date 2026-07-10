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

namespace AshborneGame._Core.Data.Definitions
{
    public static class LocationDefinitions
    {
        public static IReadOnlyList<LocationDefinition> All { get; } = [Prologue.PrologueStart, Dreamspace.EyePlatform, Dreamspace.PlatformEdge, Dreamspace.HallOfMirrors, Dreamspace.MirrorShardSublocation];

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
    }
}
