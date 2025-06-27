using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using Moq;

namespace AshborneTests
{
    public class GlobalTestSetup
    {
        public GlobalTestSetup()
        {
            // This runs ONCE before ANY tests in this collection
            Console.WriteLine(">>> Running global test setup...");

            IOService.Initialise(new Mock<IInputHandler>().Object, new Mock<IOutputHandler>().Object);
            Player player = new Player();
            GameContext.Initialise(player, new GameStateManager(player), new Mock<DialogueService>().Object, new Mock<GameEngine>().Object);
        }
    }

    [CollectionDefinition("AshborneTests")]
    public class AshborneTestCollection : ICollectionFixture<GlobalTestSetup>
    {
        // Marker class - intentionally empty
    }
}
