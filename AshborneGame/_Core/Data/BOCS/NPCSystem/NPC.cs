using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
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

        public string? Greeting { get; }

        public string? DialogueFileName { get; init; }

        public NPC(string name, string? greeting, string? dialogueFileName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = "An NPC."; // Default description
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
        }

        public NPC(string name, string description, string? greeting, string? dialogueFileName = null)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Name cannot be null.");
            Description = description ?? throw new ArgumentNullException(nameof(description), "Description cannot be null.");
            Greeting = greeting;
            DialogueFileName = dialogueFileName;
        }

        public virtual void Talk(Player player)
        {
            player.CurrentNPCInteraction = this;
            if (DialogueFileName != null)
            {
                // Check if we're in a web environment by checking if the input handler supports async
                if (IOService.Input is WebPort.WebInputHandler)
                {
                    // Use async version for web - run directly on UI thread
                    _ = GameContext.DialogueService.StartDialogueAsync(DialogueFileName);
                }
                else
                {
                    // Use sync version for console
                    GameContext.DialogueService.StartDialogue(DialogueFileName);
                }
            }
            else if (Greeting != null)
            {
                IOService.Output.Write($"{Name}: {Greeting}");
            }
        }
    }
}
