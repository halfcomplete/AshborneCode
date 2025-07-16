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
            IOService.Output.Write("What do you choose? ");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= choiceCount)
                {
                    return choice;
                }

                IOService.Output.DisplayFailMessage("Invalid choice. Enter a number between 1 and " + choiceCount);
            }
        }

        // Async versions for interface compatibility
        public async Task<string> GetPlayerInputAsync()
        {
            return await Task.FromResult(GetPlayerInput());
        }

        public async Task<int> GetChoiceInputAsync(int choiceCount)
        {
            return await Task.FromResult(GetChoiceInput(choiceCount));
        }
    }
}
