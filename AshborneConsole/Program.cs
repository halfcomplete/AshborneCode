using AshborneGame._Core.Game;
using AshborneGame.ConsolePort;

namespace AshborneConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = new ConsoleInputHandler();
            var output = new ConsoleOutputHandler();

            var game = new GameEngine(input, output);

            game.Start();
        }
    }
}
