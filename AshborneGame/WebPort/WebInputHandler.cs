using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame.WebPort
{
    public class WebInputHandler : IInputHandler
    {
        private readonly Func<Task<string>> _getUserInputAsync;
        private readonly Func<int, Task<int>> _getChoiceInputAsync;

        public WebInputHandler(
            Func<Task<string>> getUserInputAsync,
            Func<int, Task<int>> getChoiceInputAsync)
        {
            _getUserInputAsync = getUserInputAsync;
            _getChoiceInputAsync = getChoiceInputAsync;
        }

        public async Task<string> GetPlayerInputAsync()
        {
            return await _getUserInputAsync();
        }

        public async Task<int> GetChoiceInputAsync(int choiceCount)
        {
            return await _getChoiceInputAsync(choiceCount);
        }

        // Legacy sync methods for compatibility
        public string GetPlayerInput()
        {
            return GetPlayerInputAsync().GetAwaiter().GetResult();
        }

        public int GetChoiceInput(int choiceCount)
        {
            return GetChoiceInputAsync(choiceCount).GetAwaiter().GetResult();
        }
    }
}
