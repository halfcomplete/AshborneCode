
namespace AshborneGame._Core.Globals.Interfaces
{
    public interface IInputHandler
    {
        string GetPlayerInput();
        int GetChoiceInput(int choiceCount);
        
        // Async versions for web compatibility
        Task<string> GetPlayerInputAsync();
        Task<int> GetChoiceInputAsync(int choiceCount);
    }
}
