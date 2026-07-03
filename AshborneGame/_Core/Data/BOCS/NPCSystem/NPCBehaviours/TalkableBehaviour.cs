using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class TalkableBehaviour
    {
        /// <summary>
        /// The default greeting that will be said if there is no valid dialogue file to initiate.
        /// </summary>
        public string? Greeting { get; }
        
        /// <summary>
        /// The file name of this NPC's dialogue file (not the full path, which is parsed from the file name).
        /// </summary>
        public string? DialogueFileName { get; init; }

        public NPC? ParentNPC { get; set; }

        public TalkableBehaviour(NPC parent, string? dialogueFileName, string? greeting = null)
        {
            ParentNPC = parent;
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
        }

        /// <summary>
        /// Initiates this NPC's given dialogue file. If there is none, then simply the Greeting is output. 
        /// <para>If there is no Greeting set, <c>$"{Name} has nothing to say."</c> is output.</para>
        /// </summary>
        /// <param name="player"></param>
        /// <returns></returns>
        public virtual async Task Talk(Player player)
        {
            // Set the player's current NPC interaction to this NPC object
            player.CurrentNPCInteraction = ParentNPC;
            if (DialogueFileName != null)
            {
                // Start the dialogue
                await GameContext.DialogueService.StartDialogue(DialogueFileName);
            }
            else if (Greeting != null)
            {
                // If there is no dialogue file, output the default greeting
                await IOService.Output.Write($"{Greeting} {(ParentNPC == null ? "" : ParentNPC.Name + " says.")}");
            }
            else
            {
                // If there is no dialogue file nor greeting assume this NPC is silent to the player
                await IOService.Output.Write($"{(ParentNPC == null ? "They have" : ParentNPC.Name + " has")} nothing to say.");
            }
        }
    }
}