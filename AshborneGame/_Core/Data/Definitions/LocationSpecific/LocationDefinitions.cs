using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Services;
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
            #region Waking Chamber

            public static LocationDefinition WakingChamber = new(
                DefinitionIDs.Locations.OssuaryOfEyesLocs.WakingChamber,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Waking Chamber", "the Waking Chamber"),
                new DescriptionComposer(
                    new LookDescription(
                        "You take a look around. The chamber is circular and austere, built from dark stone worn smooth by time. At the centre, an enormous eye has been carved into the stone.",
                        "You examine the chamber once more. The stone is old, but you now notice that the room is not abandoned. Someone has swept the edges of the floor. The small basin contains clear, fresh water, and the remains of several candles sit neatly against the wall. The eye beneath your feet remains closed.",
                        "You look around the chamber again. There is little left to discover, but one detail continues to trouble you. The carved eye is slightly deeper than the surrounding stone, as though something once rested inside it - or as though it was carved from the inside outward. For a moment, you have the uncomfortable impression that it is waiting."
                    ),
                    new VisitDescription(
                        "For a few moments, there is nothing but darkness and the sound of your own breathing. Then, seemingly from the depths of your mind, a flat voice emerges." +
                        "\n\n\"You have awoken.\" A simple statement, nothing more, nothing less. " +
                        "Finally, your eyes adjust.\n\nYou are lying in a circular chamber of black stone, surrounded by walls that disappear into darkness above you. To your side, a small basin filled with water sits quietly alongside several unlit candles.",
                        "You return to the chamber where you first awoke. The room is exactly as you remember it: black stone, shallow water, somewhat eerie atmosphere. But you notice other things now. The candles have been cleaned. The basin has been refilled. Someone has been taking care of this room.",
                        "Yet again, you return to the chamber of your arrival. It no longer feels entirely unfamiliar. You are beginning to suspect that the chamber is not as empty as it first appeared.",
                        "You return to the chamber again. Nothing appears to have changed: the water lies still and the candles remain unlit."
                    ),
                    new SensoryDescription(
                        "The black stone reflects the faintest traces of light, giving the chamber a dim, colourless sheen.",
                        "A distant hum resonates through the chamber, vibrating through your very bones.",
                        "The stone beneath your feet is cold enough to numb your toes, while the shallow water is strangely warmer than the air.",
                        "The air smells faintly of wet stone, extinguished candles and something older that you cannot identify."
                    ),
                    new AmbientDescription()
                    .AddRandomTimeBased(
                        "A single drop of water falls somewhere beyond the walls, followed by a long silence.",
                        "One of the candles gives a faint hiss before becoming still again.",
                        "Somewhere below you, stone shifts with a low, distant groan.",
                        "The darkness above the chamber seems to deepen, though there is no visible change in the light.",
                        "You hear what sounds like a footstep somewhere beyond the exits.\n\nAnother follows.\n\nThen... nothing.",
                        "A faint current of air passes across the water, carrying the smell of damp earth.",
                        "The carved eye beneath you catches the light for an instant.",
                        "You wait for something else to happen.\n\nNothing does.",
                        "For a brief moment, you hear a voice somewhere beyond, but you cannot make out the words.",
                        "You become aware that you have been holding your breath. You let it go in a soft exhale."
                    )
                ),
                [],
                new CustomCommandHandler()
                .AddCustomCommand(
                    new CustomCommandPhrasing(["look at", "examine", "inspect"], ["eye", "the eye", "carved eye", "the carved eye"]),
                    () =>
                    {
                        IOService.Output.WriteNonDialogueLine("You crouch beside the carving.\n\nUp close, the eye is more detailed than you first realised. Fine lines radiate from the iris, each one cut so precisely that the stone almost appears soft beneath your fingers.\n\nThere is no pupil. Only a shallow, empty depression.");
                        GameContext.GameState.TryIncrementCounter(StateKeys.Counters.Player.TimesInspected.CarvedEye, 1);
                    }
                )
                .AddCustomCommand(
                    new CustomCommandPhrasing(["walk to", "move to", "go to", "look at", "examine", "inspect"], ["basin", "the basin", "water", "the water"]),
                    () => { IOService.Output.WriteNonDialogueLine("You peer into the basin. The water is clear and still, reflecting the dim light of the chamber. You can see your own reflection, but it seems... different. The eyes staring back at you are not quite your own. On the side, a small inscription catches your attention:\n\nREMEMBER WHAT YOU HAVE SEEN.\n\nThe lettering is worn, but the cuts are recent enough that they could not have been made centuries ago."); }
                )
                .AddCustomCommand(
                    new CustomCommandPhrasing(["drink", "sip", "taste"], ["water", "the water", "from the basin"]),
                    () => { IOService.Output.WriteNonDialogueLine("You take a sip of the water. It is cool and refreshing, with a faint mineral taste. For a moment, you feel a strange clarity in your mind, as though the water has washed away some of the fog."); }
                )
                .AddCustomCommand(
                    new CustomCommandPhrasing(["walk to", "move to", "go to", "look at", "examine", "inspect"], ["candles", "the candles"]),
                    () => { IOService.Output.WriteNonDialogueLine("You take a closer look at the candles sitting beside the wall. It's clear they've have been extinguished recently: their wicks are still blackened, and the wax around their bases has not yet collected dust.\n\nSomeone was here before you.\n\nNot long ago."); },
                )
            );

            #endregion Waking Chamber

            #region Keeper's Quarters

            public static LocationDefinition KeepersQuarters = new(
                DefinitionIDs.Locations.OssuaryOfEyesLocs.KeepersQuarters,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Keeper's Quarters", "the Keeper's Quarters"),
                new DescriptionComposer(
                    new LookDescription(
                        "You take a look around the Keeper's Quarters. The Keeper’s Quarters are narrow but comfortable, built from dark stone and timber. Shelves cover most of the walls, while a large desk sits beneath a collection of maps and diagrams.\n\nUnlike the rest of this place, almost everything here has been deliberately arranged.",
                        "You examine the room more carefully.\r\rThe shelves are filled with books, journals and records, each labelled in the same careful handwriting. Small boxes sit beneath them, each marked with a name, date or location.\n\nA collection of keys hangs beside the desk. There are far more of them than you expected.",
                        "You have seen the room enough times to recognise most of it.\n\nThe desk. The shelves. The fire. The maps.\n\nBut there are still details you cannot explain: some of the records are written in a handwriting that does not resemble the Keeper’s, snd several objects on the shelves look as though they did not come from here."
                    ),
                    new VisitDescription(
                        "You step into a room that feels strangely ordinary.\n\nThe floor is a dry, familiar wood. A small fire burns in a stone brazier, throwing warm light across shelves of books and carefully labelled boxes. A single heavy desk occupies one side of the room, its surface covered with papers, ink and objects whose purposes you cannot immediately identify.\n\nThe Keeper who lives here is seated behind the desk.\r\n\r\nHe looks up when you enter.",
                        "You return to the Keeper’s Quarters.\n\nThe room is warmer than the rest of this place, and the familiar smell of smoke and old paper reaches you before you have fully entered. The Keeper remains at his desk, surrounded by the same precise arrangement of books, records and objects.\n\nHe seems unsurprised to see you.",
                        "You enter the Keeper’s Quarters again.\n\nBy now, the room has become familiar. You recognise the desk, the shelves, the labelled boxes and the collection of keys beside the Keeper.\n\nYet the longer you spend here, the more you notice: there are records everywhere. Some concern the Ossuary. Some concern people. Some concern you.",
                        "You enter the Keeper’s Quarters.\n\nThe fire burns quietly. The papers remain arranged in their precise stacks. The Keeper is at his desk, reading.\r\n\r\nHe acknowledges your arrival without looking up."
                    ),
                    new SensoryDescription(
                        "The scent of old books and wax fills the air.",
                        "A faint scratching sound echoes through the room, as if something unseen is moving."
                    ),
                    new AmbientDescription()
                    .AddRandomTimeBased(
                        "The fire shifts in the brazier, sending a brief wave of warmth across the room.",
                        "The Keeper turns a page, and a drop of ink falls from the tip of his pen.",
                        "Somewhere in the shelves, a book settles with a quiet thump.",
                        "You hear the faint scrape of paper against paper.",
                        "The fire crackles.",
                        "The Keeper pauses over a line of writing. He reaches for another book without looking for it.",
                        "A key moves slightly on its hook.",
                        "You hear footsteps pass somewhere beyond the farside door.",
                        "The Keeper's pen stops. He looks towards farside the door. After a moment, he continues writing.",
                        "One of the candles gutters before recovering.",
                        "Dust moves through the firelight.",
                        "You hear something being dragged across stone somewhere outside.",
                        "The Keeper closes his book.\n\nHe watches you for a moment longer than necessary.\n\nThen he looks away."
                    )
                ),
                [],
                new CustomCommandHandler()
                .AddCustomCommand(
                    new CustomCommandPhrasing(
                        ["inspect", "examine", "look at", "look over", "look around", "look in", "look inside", "look under", "look within"],
                        ["books", "bookshelves", "the books", "the bookshelves", "desk", "the desk", "the books on the desk", "the maps", "map", "the map", "maps", "the maps on the desk", "the diagrams", "the keys", "keys", "the keys on the desk"]
                    ),
                    () => { IOService.Output.WriteNonDialogueLine("You can't see much from here. Maybe try going closer."); }
                )
                .AddCustomCommand(
                    new CustomCommandPhrasing(
                        ["talk to", "speak to", "converse with", "ask"],
                        ["the guy", "keeper", "the keeper", "the person"]
                    ),
                    () => 
                    {
                        // TODO: Add Keeper dialogue
                        //GameContext.DialogueService.StartDialogue("");
                    }
                )
            );

            public static class KeepersQuartersLocs
            {
                public static LocationDefinition KeepersQuartersDesk = new(
                    DefinitionIDs.Locations.OssuaryOfEyesLocs.KeepersQuartersLocs.Desk,
                    DefinitionIDs.Scenes.OssuaryOfEyes,
                    new LocationNameAdapter("desk", "the desk"),
                    new DescriptionComposer(
                        new LookDescription(
                            "You look at the desk. It is a large, sturdy piece of furniture, covered in papers, ink and objects whose purposes you cannot immediately identify.",
                            "You take a look at it again." // TODO: fill out
                        ),
                        new VisitDescription(
                            "You approach the desk with the Keeper seated behind it. The Keeper glances up at you curiously, but quickly returns to his work.",
                            "You return to the desk. The Keeper continues to write, his pen moving quickly across the page.",
                            "You approach the desk again. The Keeper looks up, his eyes meeting yours for a brief moment before returning to his writing."
                        ),
                        new SensoryDescription(
                            "The scent of ink and old paper fills the air.",
                            "A faint scratching sound echoes through the room, as if something unseen is moving."
                        ),
                        new AmbientDescription()
                        .AddRandomTimeBased("\"Well?\" The Keeper looks up from his work, his eyes fixated on you. After a while, he returns to his writing.")
                    ),
                    [],
                    new CustomCommandHandler()
                    .AddCustomCommand(new CustomCommandPhrasing(
                            ["inspect", "examine", "look at"],
                            ["papers", "the paper", "the papers", "writing", "the writing", "books", "the books", "maps", "the maps"]
                        ),
                        () =>
                        {
                            IOService.Output.WriteNonDialogueLine("You pick up one of the pieces of ");
                        }
                    )
                );
            }

            #endregion Keeper's Quarters

            /*

            public static LocationDefinition HallOfLostThoughts = new(
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

            */

            public static LocationDefinition CloisterGardens = new(
                DefinitionIDs.Locations.OssuaryOfEyesLocs.CloisterGardens,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("Cloister Gardens", "the Cloister Gardens"),
                new DescriptionComposer(
                    new LookDescription(
                        "You take a look around the garden.\n\nIt's a large stone courtyard filled with plants; paths divide the garden into several areas, with a fountain standing near the centre. Covered cloisters run along the surrounding walls, and several smaller passages lead into different parts of the garden.",
                        "You examine the garden more carefully.\n\nThere is a clear distinction between what has been planted and what has simply been allowed to grow. The central beds are carefully maintained, while the edges are increasingly overgrown. A number of tools have been left beside the paths: it's clear the Gardener spends a great deal of time here."
                    ),
                    new VisitDescription(
                        "You step beneath a stone archway and emerge into a garden.\n\n" +
                        "For the first time since entering the Ossuary, you see something growing: " + 
                        "plants cover the courtyard in carefully arranged beds, climbing trellises and clay pots. A narrow path winds between them beneath an open section of sky, while old cloisters surround the garden on three sides." +
                        "A woman kneels among the plants with a small knife in one hand.\nShe looks up at you for a moment but quickly returns to tending the garden.",

                        "You return to the Cloister Garden.\n\n" +
                        "The paths are familiar now. The plants have not changed much, although several have been trimmed since your last visit.\n" +
                        "The Gardener is working somewhere nearby. You can hear her moving between the beds.",

                        "You return to the garden.\n\n" +
                        "You have begun to recognise which plants belong where. The cultivated beds remain orderly, but the farther corners of the garden are still less controlled. Vines have pushed through cracks in the masonry, and roots have lifted sections of paving." +
                        "\n\nThe Gardener does not seem concerned.",
                    
                        "You enter the Cloister Garden.\n\n" +
                        "The plants continue to grow. Water runs through the narrow channels between the beds, and the paths remain clear. The Gardener is somewhere in the garden."
                    ),
                    new SensoryDescription(
                        "The scent of fresh flowers fills the air.",
                        "A gentle breeze rustles through the leaves, creating a soothing sound."
                    ),
                    new AmbientDescription()
                    .AddRandomTimeBased(
                        "Leaves move gently above the garden path.",
                        "Water continues running through the narrow channels between the beds.",
                        "Somewhere nearby, a gardening tool strikes stone.",
                        "A bird lands briefly on the edge of the fountain.\n\nThe bird disappears almost immediately.",
                        "A vine shifts against the wall with the wind.",
                        "Several leaves fall onto the path.",
                        "The Gardener hums quietly somewhere beyond the beds.",
                        "You hear the scrape of a shovel against earth.",
                        "Water drips from a leaf.",
                        "You hear the sound of roots shifting beneath the soil.",
                        "A gust of air passes through the cloisters.",
                        "The fountain briefly stops running.\n\nAfter a moment, the water starts running again.",
                        "A loose stone rolls somewhere beneath the vegetation.",
                        "From beneath the paving, you hear a faint hollow sound.\n\nYou look towards the sound, but nothing appears to have moved."
                    )
                ),
                [],
                new CustomCommandHandler()
                .AddCustomCommand(
                    new CustomCommandPhrasing(["tend", "tend to", "care for", "care", "take care of"], ["", "plants", "plant", "the plants"]),
                    () =>
                    {
                        
                    }
                )
            );

            /*
            
            public static LocationDefinition LowerVault = new(
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

            public static LocationDefinition CentralOssuary = new(
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

            public static LocationDefinition Scriptorium = new(
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

            public static LocationDefinition Observatory = new(
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
            
            */
        }
    }
}
