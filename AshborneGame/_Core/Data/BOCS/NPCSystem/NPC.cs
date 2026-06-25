using AshborneGame._Core._Player;
using AshborneGame._Core.EmotionSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame.WebPort;
using System.Security.Cryptography.X509Certificates;

namespace AshborneGame._Core.Data.BOCS.NPCSystem
{
    /// <summary>
    /// A BOCSGameObject specifically for NPCs in the game. Contains attributes and methods to manage the psychological state, name, greeting, synonyms and description of this NPC.
    /// </summary>
    public class NPC : BOCSGameObject, ISentientEntity
    {
        /// <summary>
        /// The name of the NPC. This is used for identification and display.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// A describer for the NPC, providing shallow details about their appearance or role. Often used with their name to give context.
        /// </summary>
        public string Description { get; }

        /// <summary>
        /// List of synonyms for the NPC's name that can be used to identify them.
        /// </summary>
        public List<string> Synonyms { get; }

        /// <summary>
        /// The default greeting that will be said if there is no valid dialogue file to initiate.
        /// </summary>
        public string? Greeting { get; }
        
        /// <summary>
        /// The file name of this NPC's dialogue file (not the full path, which is parsed from the file name).
        /// </summary>
        public string? DialogueFileName { get; init; }
        
        /// <summary>
        /// Represents the psychological state of this NPC. Encompasses emotions, feelings and relationships.
        /// </summary>
        public PsychologicalState PsychologicalState { get; }

        public NPC(string name, string? greeting, string? dialogueFileName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = "An NPC."; // Default description
            Synonyms = new List<string>();
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
            PsychologicalState = new(ID);
        }

        public NPC(string name, string description, string? greeting, string? dialogueFileName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Synonyms = new List<string>();
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
            PsychologicalState = new(ID);
        }

        /// <summary>
        /// Instantiates an new NPC object with a given name, description, default greeting, dialogue file and a list of synonyms.
        /// </summary>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="description">A short description of the NPC.</param>
        /// <param name="greeting">The NPC's default greeting.</param>
        /// <param name="dialogueFileName">the NPC's dialogue file name.</param>
        /// <param name="synonyms">A list of synonyms which can be used to refer to the NPC other than just its name.</param>
        /// <exception cref="ArgumentNullException"></exception>
        public NPC(string name, string description, string? greeting, string? dialogueFileName, List<string> synonyms)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Synonyms = synonyms ?? new List<string>();
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
            PsychologicalState = new(ID);
        }

        /// <summary>
        /// Checks if the given name matches this NPC's name or any of its synonyms.
        /// </summary>
        /// <param name="targetName">The name to check against.</param>
        /// <returns>True if the name matches the NPC's name or any synonym.</returns>
        public bool MatchesName(string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
                return false;

            string normalizedTarget = targetName.ToLowerInvariant().Trim();
            
            // Check against the main name
            if (Name.ToLowerInvariant().Contains(normalizedTarget))
                return true;
            
            // Check against synonyms
            foreach (string synonym in Synonyms)
            {
                if (synonym.ToLowerInvariant().Contains(normalizedTarget))
                    return true;
            }
            
            return false;
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
            player.CurrentNPCInteraction = this;
            if (DialogueFileName != null)
            {
                // Start the dialogue
                await GameContext.DialogueService.StartDialogue(DialogueFileName);
            }
            else if (Greeting != null)
            {
                // If there is no dialogue file, output the default greeting
                await IOService.Output.Write($"{Name}: {Greeting}");
            }
            else
            {
                // If there is no dialogue file nor greeting assume this NPC is silent to the player
                await IOService.Output.Write($"{Name} has nothing to say.");
            }
        }
    }
}
