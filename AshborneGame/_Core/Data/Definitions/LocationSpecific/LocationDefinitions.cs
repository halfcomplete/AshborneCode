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
            OssuaryOfEyes.WakingChamber, OssuaryOfEyes.KeepersQuarters, OssuaryOfEyes.CloisterGardens,
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
                    () => { IOService.Output.WriteNonDialogueLine("You take a closer look at the candles sitting beside the wall. It's clear they've have been extinguished recently: their wicks are still blackened, and the wax around their bases has not yet collected dust.\n\nSomeone was here before you.\n\nNot long ago."); }
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
                        "You take a look around the Keeper's Quarters. The Keeper's Quarters are narrow but comfortable, built from dark stone and timber. Shelves cover most of the walls, while a large desk sits beneath a collection of notes and diagrams. Behind, stuck onto the wall, is a display of various maps, the details of which you cannot make out clearly from here.\n\nUnlike the rest of this place, almost everything here has been deliberately arranged.",
                        "You examine the room more carefully.\r\rThe shelves are filled with books, journals and records, each labelled in the same careful handwriting. Small boxes sit beneath them, each marked with a name, date or location.\n\nA collection of keys hangs beside the desk. There are far more of them than you expected. Maybe you should take one.",
                        "You have seen the room enough times to recognise most of it.\n\nThe desk. The shelves. The fire. The maps.\n\nBut there are still details you cannot explain: some of the records are written in a handwriting that does not resemble the Keeper's, snd several objects on the shelves look as though they did not come from here."
                    ),
                    new VisitDescription(
                        "You step into a room that feels strangely ordinary.\n\nThe floor is a dry, familiar wood. A small fire burns in a stone brazier, throwing warm light across shelves of books and carefully labelled boxes. A single heavy desk occupies one side of the room, its surface covered with papers, ink and objects whose purposes you cannot immediately identify. Behind it, a collection of maps and diagrams is displayed on the wall.\n\nThe Keeper who lives here is seated behind the desk.\r\n\r\nHe looks up when you enter.",
                        "You return to the Keeper's Quarters.\n\nThe room is warmer than the rest of this place, and the familiar smell of smoke and old paper reaches you before you have fully entered. The Keeper remains at his desk, surrounded by the same precise arrangement of books, records and objects.\n\nHe seems unsurprised to see you.",
                        "You enter the Keeper's Quarters again.\n\nBy now, the room has become familiar. You recognise the desk, the shelves, the labelled boxes and the collection of keys beside the Keeper.\n\nYet the longer you spend here, the more you notice: there are records everywhere. Some concern the Ossuary. Some concern people. Some concern you.",
                        "You enter the Keeper's Quarters.\n\nThe fire burns quietly. The papers remain arranged in their precise stacks. The Keeper is at his desk, reading.\r\n\r\nHe acknowledges your arrival without looking up."
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
                        "A key moves slightly on its hook on the desk.",
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
                        ["inspect", "examine", "look at", "look over", "look around", "look in", "look inside", "look under", "look within", "take", "steal", "grab"],
                        ["books", "bookshelves", "the books", "the bookshelves", "desk", "the desk", "the books on the desk", "the maps", "map", "the map", "maps", "the maps on the desk", "the diagrams", "the keys", "keys", "the keys on the desk"]
                    ),
                    () => { IOService.Output.WriteNonDialogueLine("You can't see or do much from here. Maybe try going closer."); }
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
                            "You take a look at the Keeper's desk. It's old, but carefully maintained. Several stacks of paper cover its surface. Some are written in the Keeper's precise handwriting. Others contain diagrams, lists and symbols you cannot understand. Nothing appears to have been placed here accidentally."
                        ),
                        new VisitDescription(
                            "You approach the desk with the Keeper seated behind it. The Keeper glances up at you curiously, but quickly returns to his work. There are innumerous documents on the desk, some of which are written in a handwriting that is not the Keeper's. Maybe you should take a closer look at them.",
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
                            ["inspect", "examine", "look at", "read"],
                            ["papers", "the paper", "the papers", "writing", "the writing", "books", "the books"]
                        ),
                        () =>
                        {
                            IOService.Output.WriteNonDialogueLine(
                                "You pick up the closest piece of paper to you and read it.\n\n" + 
                                "With a start, you realise it's about you. But it contains remarkably little information: your name. Your arrival. Several observations. And a blank section labelled:\n\n" +
                                "CONSEQUENCE");
                        }
                    )
                    .AddCustomCommand(new CustomCommandPhrasing(
                        ["inspect", "examine", "look at", "read"],
                        ["maps", "the maps", "map", "the map"]
                        ),
                        () =>
                        {
                            // TODO: add visit time checking to make this description more personalised as to how much the player's explored already
                            // TODO: fill out w/ rest of locations asw
                            IOService.Output.WriteNonDialogueLine(
                                "You take a closer look at the maps on the wall.\n\n" +
                                "From here, you see that the maps are of this place and its surrounding areas. They are detailed and precise, labelling this strange location as \"The Ossuary of Eyes\".\n" +
                                "You notice that the room you're in is called the \"Keeper's Quarters\", while the room you woke up in is the \"Waking Chamber\".\n" + 
                                "You also notice that there are several other rooms on the maps, each with their own names and descriptions: the \"Cloister Gardens\" and the \"Lower Vault\" beneath, as well as the \"Central Ossuary\". But one location draws your attention the most: at the very top, named simply but labelled with a mysterious eye symbol: the \"Observatory\".\n\n" +
                                "You are sure that all these locations are important, particularly the Observatory. But you cannot yet understand why."
                            );
                        }
                    )
                    .AddCustomCommand(new CustomCommandPhrasing(
                        ["take", "grab", "steal", "pick up"],
                        ["key", "keys", "the keys"]),
                        () =>
                        {
                            IOService.Output.WriteNonDialogueLine(
                                "You reach for the keys hanging beside the desk. There are far more of them than you expected, each one labelled with a name, date or location. You take one at random and slip it into your pocket.\n\n" +
                                "You have no idea what it opens, but you feel a strange sense of anticipation."
                            );
                            // TODO: Add key to inventory
                            // TODO: initiate dialogue for when player takes key, make it dependent on trust and stuff, keeper may confront player
                        }
                    )
                );
            }

            public static LocationDefinition KeepersQuartersBookShelves = new(
                DefinitionIDs.Locations.OssuaryOfEyesLocs.KeepersQuartersLocs.Bookshelves,
                DefinitionIDs.Scenes.OssuaryOfEyes,
                new LocationNameAdapter("bookshelves", "the bookshelves"),
                new DescriptionComposer(
                    new LookDescription(
                        "You take a look at the Keeper's bookshelves. Hundreds of books occupy the shelves. Some are histories. Others appear to be catalogues, journals or records of individual people. The books are arranged according to a system you cannot immediately understand. Underneath the shelves, you see small boxes are stacked neatly, each labelled with a name, date or location. Most are unlocked except one, which you suspect might contain something of interest. If only you had a key."
                    ),
                    new VisitDescription(
                        "You approach the bookshelves. The Keeper glances up at you curiously, but quickly returns to his work.",
                        "You return to the bookshelves. The Keeper continues to write, his pen moving quickly across the page.",
                        "You approach the bookshelves again. The Keeper looks up, his eyes meeting yours for a brief moment before returning to his writing."
                    ),
                    new SensoryDescription(
                        "The scent of old paper and ink fills the air.",
                        "A faint scratching sound echoes through the room, as if something unseen is moving."
                    ),
                    new AmbientDescription()
                    .AddRandomTimeBased("\"You like reading?\" The Keeper asks, without looking up from his work. After a moment of silence on your part, he returns to writing.")
                ),
                [],
                new CustomCommandHandler()
                .AddCustomCommand(new CustomCommandPhrasing(
                    ["read", "open", "inspect", "examine", "look at", "take a look at", "take a closer look at", "skim through"],
                    ["a book", "the books", "some books", "books", "book"]),
                    () =>
                    {
                        IOService.Output.WriteNonDialogueLine(
                            "You take a book from the shelves and open it. The pages are filled with text, diagrams and illustrations. The book concerns an obscure event in the history of the Ossuary. The account is written as though the author witnessed it personally. The final entry ends abruptly. There is no explanation.\n\n" +
                            "However, you understand more than enough. You realise that these books are records of the past, present and future: they contain knowledge that has been gathered over centuries, and you feel a sense of awe at the Keeper's dedication to preserving it."
                        );
                    }
                )
                .AddCustomCommand(new CustomCommandPhrasing(
                    ["scan", "skim", "search"],
                    ["the shelves", "the books"]),
                    () =>
                    {
                        IOService.Output.WriteNonDialogueLine(
                            "You scan the shelves, looking for anything that might be of interest. You find sections concerning places, people, memories, masks, events and unresolved records. The section labelled Masks is considerably larger than you expected."
                        );
                    }
                )
                .AddCustomCommand(new CustomCommandPhrasing(
                    ["inspect", "examine", "look at", "take a closer look at", "take a look at"],
                    ["the boxes", "the small boxes", "boxes", "small boxes", "the labelled boxes", "labelled boxes"]),
                    () =>
                    {
                        IOService.Output.WriteNonDialogueLine(
                            "You take a closer look at the small boxes stacked beneath the shelves. Each one is labelled with a name, date or location, and contain a single object. A button. A broken key. A child's drawing. A coin. A piece of jewellery. A fragment of glass. However, one particularly intriguing box stands out: it's the only locked one. You need a key to open it."
                        );
                    }
                )
                .AddCustomCommand(new CustomCommandPhrasing(
                    ["open", "unlock", "inspect", "examine", "look at", "take a closer look at", "take a look at"],
                    ["the locked box", "locked box", "the box", "box"]),
                    () =>
                    {
                        if (GameContext.Player.Inventory.GetItemCount(DefinitionIDs.Items.OssuaryOfEyes.KeepersKey.Value) > 0)
                        {
                            IOService.Output.WriteNonDialogueLine(
                                // TODO: figure out exactly what happens here
                                "You take the key from your pocket and insert it into the lock of the box. With a satisfying click, the lock opens, and you lift the lid to reveal its contents.\n\nInside, you find a small, "
                            );
                        }
                        else
                        {
                            IOService.Output.WriteNonDialogueLine(
                                "You try to open the locked box, but it won't budge. You need a key to unlock it. Perhaps you can find one somewhere in the Keeper's Quarters."
                            );
                        }
                    }
                )
            );

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
