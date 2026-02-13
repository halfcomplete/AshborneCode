using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame.WebPort;

namespace AshborneGame._Core.Data.BOCS.NPCSystem
{
    public class NPC : BOCSGameObject
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

        public string? Greeting { get; }

        public string? DialogueFileName { get; init; }

        public NPC(string name, string? greeting, string? dialogueFileName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = "An NPC."; // Default description
            Synonyms = new List<string>();
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
        }

        public NPC(string name, string description, string? greeting, string? dialogueFileName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Synonyms = new List<string>();
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
        }

        public NPC(string name, string description, string? greeting, string? dialogueFileName, List<string> synonyms)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Synonyms = synonyms ?? new List<string>();
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
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

        public virtual async Task Talk(Player player)
        {
            player.CurrentNPCInteraction = this;
            if (DialogueFileName != null)
            {
                await GameContext.DialogueService.StartDialogue(DialogueFileName);
            }
            else if (Greeting != null)
            {
                await IOService.Output.Write($"{Name}: {Greeting}");
            }
        }
    }
}
