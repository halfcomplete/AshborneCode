using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame.ConsolePort
{
    public class ConsoleInputHandler : IInputHandler
    {
        public Task<string> GetPlayerInput(string prompt = "What will you say?")
        {
            Console.WriteLine("");
            Console.WriteLine(prompt);
            Console.Write("> ");
            string input = Console.ReadLine() ?? string.Empty;
            return Task.FromResult(ParseNameInput(input));
        }

        public async Task<int> GetChoiceInput(int choiceCount)
        {
            await IOService.Output.WriteNonDialogueLine("What do you choose? ");
            while (true)
            {
                string input = Console.ReadLine() ?? "";
                if (int.TryParse(input, out int choice) && choice >= 1 && choice <= choiceCount)
                {
                    return choice;
                }

                await IOService.Output.DisplayFailMessage("Invalid choice. Enter a number between 1 and " + choiceCount);
            }
        }

        // Async versions for interface compatibility
        public async Task<string> GetPlayerInputAsync(string prompt = "What will you say?")
        {
            return await GetPlayerInput(prompt);
        }
        private string ParseNameInput(string input)
        {
            // Simple parser for name input, can be extended for other types
            var patterns = new[] {
                @"my name is\s+(.*)",
                @"i am\s+(.*)",
                @"i'm\s+(.*)",
                @"call me\s+(.*)",
                @"name's\s+(.*)",
                @"it's\s+(.*)"
            };
            foreach (var pattern in patterns)
            {
                var match = System.Text.RegularExpressions.Regex.Match(input, pattern, System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                if (match.Success) return match.Groups[1].Value.Trim();
            }
            // Fallback: take last word or all input
            var words = input.Split(' ', StringSplitOptions.RemoveEmptyEntries);
            return words.Length > 1 ? words.Last() : input.Trim();
        }

        public async Task<int> GetChoiceInputAsync(int choiceCount)
        {
            return await GetChoiceInput(choiceCount);
        }
    }
}
