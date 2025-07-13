using AshborneGame._Core.Game;
using AshborneGame.ConsolePort;
using AshborneGame._Core.Globals.Services;

namespace AshborneConsole
{
    internal class Program
    {
        static void Main(string[] args)
        {
            var input = new ConsoleInputHandler();
            var output = new ConsoleOutputHandler();

            var appEnvironment = new AppEnvironment
            {
                IsGithubPages = false,
                BaseApiUrl = "/"
            };
            var game = new GameEngine(input, output, appEnvironment);

            game.Start();
        }
    }
}
