using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SceneManagement;
using Moq;

namespace AshborneTests
{
    public class GlobalTestSetup
    {
        public GlobalTestSetup()
        {
            // This runs ONCE before ANY tests in this collection
            Console.WriteLine(">>> Running global test setup...");

            Player player = new Player();
            GameStateManager gameStateManager = new GameStateManager(player);
            AppEnvironment app = new AppEnvironment();
            InkRunner inkRunner = new Mock<InkRunner>(gameStateManager, player, app).Object;
            GameContext.Initialise(player, gameStateManager, new Mock<DialogueService>(inkRunner).Object, inkRunner, new Mock<GameEngine>(new Mock<IInputHandler>().Object, new Mock<IOutputHandler>().Object, app).Object);
        }
    }

    [CollectionDefinition("AshborneTests")]
    public class AshborneTestCollection : ICollectionFixture<GlobalTestSetup>
    {
        // Marker class - intentionally empty
    }
}
