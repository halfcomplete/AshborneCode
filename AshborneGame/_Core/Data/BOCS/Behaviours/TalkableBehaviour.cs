using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AshborneGame._Core.Data.BOCS.Behaviours
{
    public class TalkableBehaviour : Behaviour
    {
        /// <summary>
        /// The default greeting that will be said if there is no valid dialogue file to initiate.
        /// </summary>
        public string? Greeting { get; }

        /// <summary>
        /// The file name of this NPC's dialogue file (not the full path, which is parsed from the file name).
        /// </summary>
        public string? DialogueFileName { get; init; }


        public TalkableBehaviour(string? greeting, string? dialogueFile = null)
        {
            Greeting = greeting;
            DialogueFileName = dialogueFile;
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
            player.CurrentNPCInteraction = Owner;
            if (DialogueFileName != null)
            {
                // Start the dialogue
                await GameContext.DialogueService.StartDialogue(DialogueFileName);
            }
            else if (Greeting != null)
            {
                // If there is no dialogue file, output the default greeting
                await IOService.Output.Write($"{Greeting} {(Owner == null ? "" : Owner.Name + " says.")}");
            }
            else
            {
                // If there is no dialogue file nor greeting assume this NPC is silent to the player
                await IOService.Output.Write($"{(Owner == null ? "They have" : Owner.Name + " has")} nothing to say.");
            }
        }

        public override TalkableBehaviour DeepClone()
        {
            return new TalkableBehaviour(Greeting, DialogueFileName);
        }
    }
}