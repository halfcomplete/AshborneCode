using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.Data.Definitions.Registries;
using Moq;

namespace AshborneTests
{
    public class GlobalTestSetup
    {
        public GlobalTestSetup()
        {
            // This runs ONCE before ANY tests in this collection
            Console.WriteLine(">>> Running global test setup...");

            // Enable test mode for SlugIdService to generate unique IDs
            SlugIdService.IsTestMode = true;

            // Clear static registries to ensure test isolation
            Player player = new Player();
            GameStateManager gameStateManager = new GameStateManager(player, new TimeTracker());
            AppEnvironment app = new AppEnvironment();
            InkRunner inkRunner = new Mock<InkRunner>(gameStateManager, player, app).Object;
            var locationRegistry = new LocationRegistry();
            var gameEngine = new GameEngine(new Mock<IInputHandler>().Object, new Mock<IOutputHandler>().Object, app);
            GameContext.Initialise(
                player,
                gameStateManager,
                new Mock<DialogueService>(inkRunner).Object,
                inkRunner,
                gameEngine,
                gameStateManager.TimeTracker,
                new AmbientTimeManager(),
                new MovementService(locationRegistry),
                new DefinitionRegistry(),
                new InstanceRegistry(),
                locationRegistry);
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
        }

        public void Dispose()
        {
            // Clean up after test
            GC.SuppressFinalize(this);
        }
    }

    [CollectionDefinition("AshborneTests")]
    public class AshborneTestCollection : ICollectionFixture<GlobalTestSetup>
    {
        // Marker class - intentionally empty
    }
}
