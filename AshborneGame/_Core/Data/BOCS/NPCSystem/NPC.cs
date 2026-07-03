using AshborneGame._Core._Player;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.Data.BOCS.NPCSystem
{
    /// <summary>
    /// A BOCSGameObject specifically for NPCs in the game. Contains attributes and methods to manage the psychological state, name, greeting, synonyms and description of this NPC.
    /// </summary>
    public class NPC : BOCSGameObject
    {
        /// <summary>
        /// The name of the NPC. This is used for identification and display.
        /// </summary>
        public override string Name { get; }

        /// <summary>
        /// A describer for the NPC, providing shallow details about their appearance or role. Often used with their name to give context.
        /// </summary>
        public override string Description { get; }

        public NPC(string name)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = "An NPC."; // Default description
            Synonyms = new List<string>();
        }

        public NPC(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Synonyms = new List<string>();
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
        public NPC(string name, string description, List<string> synonyms)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Synonyms = synonyms ?? new List<string>();
        }

        /// <summary>
        /// Checks if the given name matches this NPC's name or any of its synonyms.
        /// </summary>
        /// <param name="targetName">The name to check against.</param>
        /// <returns>True if the name matches the NPC's name or any synonym.</returns>
        public bool MatchesName(string targetName)
        {
            if (string.IsNullOrWhiteSpace(targetName))
            {
                return false;
            }

            string normalizedTarget = targetName.ToLowerInvariant().Trim();
            
            // Check against the main name
            if (Name.ToLowerInvariant().Contains(normalizedTarget))
            {
                return true;
            }

            // Check against synonyms
            foreach (string synonym in Synonyms)
            {
                if (synonym.ToLowerInvariant().Contains(normalizedTarget))
                {
                    return true;
                }
            }
            
            return false;
        }
    }
}
