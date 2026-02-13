using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Game
{
    public static class GameContext
    {
        public static Player Player { get; private set; }
        public static GameStateManager GameState { get; private set; }
        public static DialogueService DialogueService { get; private set; }
        public static InkRunner InkRunner { get; private set; }
        public static GameEngine GameEngine { get; private set; }
        public static Random Random { get; } = new Random();

        public static void Initialise(Player player, GameStateManager gameState, DialogueService dialogueRunner, InkRunner inkRunner, GameEngine gameEngine)
        {
            Player = player;
            GameState = gameState;
            DialogueService = dialogueRunner;
            InkRunner = inkRunner;
            GameEngine = gameEngine;
        }
    }
}
