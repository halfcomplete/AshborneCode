using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame.WebPort
{
    public class WebInputHandler : IInputHandler
    {
    private readonly Func<string, Task<string>> _getUserInputAsync;
        private readonly Func<int, Task<int>> _getChoiceInputAsync;

        public WebInputHandler(
            Func<string, Task<string>> getUserInputAsync,
            Func<int, Task<int>> getChoiceInputAsync)
        {
            _getUserInputAsync = getUserInputAsync;
            _getChoiceInputAsync = getChoiceInputAsync;
        }

        public async Task<string> GetPlayerInputAsync(string prompt = "What will you say?")
        {
            var input = await _getUserInputAsync(prompt);
            return ParseNameInput(input);
        }

        public async Task<int> GetChoiceInputAsync(int choiceCount)
        {
            return await _getChoiceInputAsync(choiceCount);
        }

        // Legacy sync methods for compatibility
        public Task<string> GetPlayerInput(string prompt = "What will you say?")
        {
            return GetPlayerInputAsync(prompt);
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

        public Task<int> GetChoiceInput(int choiceCount)
        {
            return GetChoiceInputAsync(choiceCount);
        }
    }
}
