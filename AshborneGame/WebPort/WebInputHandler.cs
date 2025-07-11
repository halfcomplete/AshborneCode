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

        public string GetPlayerInput()
        {
            // Use a blocking wait to simulate sync return — okay in Blazor if called inside an async method
            return _getUserInputAsync().GetAwaiter().GetResult();
        }

        public int GetChoiceInput(int choiceCount)
        {
            return _getChoiceInputAsync(choiceCount).GetAwaiter().GetResult();
        }
    }
}
