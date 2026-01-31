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

            // Clear static registries to ensure test isolation
            LocationRegistry.Clear();

            Player player = new Player();
            GameStateManager gameStateManager = new GameStateManager(player);
            AppEnvironment app = new AppEnvironment();
            InkRunner inkRunner = new Mock<InkRunner>(gameStateManager, player, app).Object;
            GameContext.Initialise(player, gameStateManager, new Mock<DialogueService>(inkRunner).Object, inkRunner, new Mock<GameEngine>(new Mock<IInputHandler>().Object, new Mock<IOutputHandler>().Object, app).Object);
        }
    }

    /// <summary>
    /// Base class for tests that need fresh LocationRegistry state.
    /// Clears the registry before each test to ensure isolation.
    /// </summary>
    public abstract class IsolatedTestBase : IDisposable
    {
        protected IsolatedTestBase()
        {
            // Clear registry before each test
            LocationRegistry.Clear();
        }

        public void Dispose()
        {
            // Clean up after test
            LocationRegistry.Clear();
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("AshborneTests")]
    public class AshborneTestCollection : ICollectionFixture<GlobalTestSetup>
    {
        // Marker class - intentionally empty
    }
}
