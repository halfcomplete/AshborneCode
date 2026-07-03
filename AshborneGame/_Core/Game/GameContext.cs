using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game
{
    public static class GameContext
    {
        public static IDefinitionRegistry DefinitionRegistry { get; private set; }
        public static IInstanceRegistry InstanceRegistry { get; private set; }

        public static Player Player { get; private set; }
        public static GameStateManager GameState { get; private set; }
        public static TimeTracker TimeTracker { get; private set; }
        public static DialogueService DialogueService { get; private set; }
        public static InkRunner InkRunner { get; private set; }
        public static GameEngine GameEngine { get; private set; }
        public static Random Random { get; } = new Random();
        public static BOCSFactory BOCSFactory { get; private set; }

        public static void Initialise(Player player, GameStateManager gameState, DialogueService dialogueRunner, InkRunner inkRunner, GameEngine gameEngine, TimeTracker timeTracker, IDefinitionRegistry def, IInstanceRegistry inst)
        {
            Player = player;
            GameState = gameState;
            DialogueService = dialogueRunner;
            InkRunner = inkRunner;
            TimeTracker = timeTracker;
            GameEngine = gameEngine;
            DefinitionRegistry = def;
            InstanceRegistry = inst;
            BOCSFactory = new(DefinitionRegistry, InstanceRegistry);
        }
    }
}
