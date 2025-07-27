
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;

namespace AshborneGame._Core.Data.BOCS.NPCSystem
{
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

            NPC npc = new NPC(name, description, dialogueFileName);
            return npc;
        }

        public static NPC CreateTradeableNPC(string name, string description, string greeting, string selfDescription)
        {
            if (string.IsNullOrWhiteSpace(name))
            {
                throw new ArgumentException("Name cannot be null or empty.", nameof(name));
            }
            if (string.IsNullOrWhiteSpace(description))
            {
                throw new ArgumentException("Description cannot be null or empty.", nameof(description));
            }

            NPC npc = new NPC(name, description, greeting, selfDescription);
            npc.AddBehaviour(typeof(IHasInventory), new TradeableNPCBehaviour());
            return npc;
        }

        public static NPC CreateDummy(string name, string description, int maxHealth)
        {
            var dummy = new NPC(name, description);
            dummy.AddBehaviour(typeof(ICanBeAttacked), new CanBeAttackedBehaviour(dummy, maxHealth));
            return dummy;
        }
    }
}
