
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;

namespace AshborneGame._Core.Data.BOCS.NPCSystem
{
    /// <summary>
    /// A static class with public methods to create and return NPCs from preset "templates".
    /// </summary>
    public static class NPCFactory
    {
        /// <summary>
        /// Creates a talkable NPC with a name, description, and dialogue file to jump to when it is talked to.
        /// </summary>
        /// <param name="name">The NPC's name.</param>
        /// <param name="description">The NPC's description.</param>
        /// <param name="dialogueFileName">The dialogue file name to jump to when it is talked to, without a path nor extension.</param>
        /// <returns>The talkable NPC.</returns>
        /// <exception cref="ArgumentException">Thrown when either of the arguments are null or empty.</exception>
        public static NPC CreateTalkableNPC(string name, string description, string dialogueFileName)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }
            if (string.IsNullOrEmpty(dialogueFileName))
            {
                throw new ArgumentException("Dialogue file name cannot be null or empty.", nameof(dialogueFileName));
            }

            NPC npc = new NPC(name, description, null, dialogueFileName);
            return npc;
        }

        /// <summary>
        /// Creates a talkable NPC with a name, description, dialogue file, and synonyms.
        /// </summary>
        /// <param name="name">The NPC's name.</param>
        /// <param name="description">The NPC's description.</param>
        /// <param name="dialogueFileName">The dialogue file name to jump to when it is talked to, without a path nor extension.</param>
        /// <param name="synonyms">List of synonyms for the NPC's name.</param>
        /// <returns>The talkable NPC.</returns>
        /// <exception cref="ArgumentException">Thrown when either of the arguments are null or empty.</exception>
        public static NPC CreateTalkableNPC(string name, string description, string dialogueFileName, List<string> synonyms)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }
            if (string.IsNullOrEmpty(dialogueFileName))
            {
                throw new ArgumentException("Dialogue file name cannot be null or empty.", nameof(dialogueFileName));
            }

            NPC npc = new NPC(name, description, null, dialogueFileName, synonyms);
            return npc;
        }

        /// <summary>
        /// Instantiates a new NPC that has an inventory and can trade with the player. By default doesn't have a dialogue file, but one can be added.
        /// </summary>
        /// <param name="name">The name of the NPC.</param>
        /// <param name="description">A short description of the NPC.</param>
        /// <param name="greeting">The NPC's default greeting.</param>
        /// <param name="dialogueFileName">The file name of the NPC's dialogue. Null by default.</param>
        /// <returns>An NPC object.</returns>
        /// <exception cref="ArgumentException"></exception>
        public static NPC CreateTradeableNPC(string name, string description, string greeting, string? dialogueFileName = null)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            NPC npc = new NPC(name, description, greeting, dialogueFileName);
            npc.AddBehaviour(typeof(IHasInventory), new TradeableNPCBehaviour());
            return npc;
        }

        /// <summary>
        /// Creates a dummy NPC which the player can't talk to, but can hit.
        /// </summary>
        /// <param name="name">The name of the dummy.</param>
        /// <param name="description">A short description of the dummy.</param>
        /// <param name="maxHealth">The maximum health of the dummy.</param>
        /// <returns></returns>
        public static NPC CreateDummy(string name, string description, int maxHealth)
        {
            var dummy = new NPC(name, description);
            dummy.AddBehaviour(typeof(ICanBeAttacked), new CanBeAttackedBehaviour(dummy, maxHealth));
            return dummy;
        }
    }
}
