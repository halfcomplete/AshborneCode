using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame.ConsolePort
{
    public class ConsoleInputHandler : IInputHandler
    {
        public string GetPlayerInput()
        {
            Console.WriteLine("");
            Console.Write("> ");
            return Console.ReadLine() ?? string.Empty;
        }

        public int GetChoiceInput(int choiceCount)
        {
            IOService.Output.WriteLine("What do you choose?");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= choiceCount)
                {
                    return choice;
                }

                IOService.Output.WriteLine("Invalid choice. Enter a number between 1 and " + choiceCount);
            }
        }
    }
}
