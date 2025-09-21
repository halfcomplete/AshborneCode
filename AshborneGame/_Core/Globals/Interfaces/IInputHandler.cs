
namespace AshborneGame._Core.Globals.Interfaces
{
    public interface IInputHandler
    {
    string GetPlayerInput(string prompt = "What will you say?");
        int GetChoiceInput(int choiceCount);
        
        // Async versions for web compatibility
        Task<string> GetPlayerInputAsync(string prompt = "What will you say?");
        Task<int> GetChoiceInputAsync(int choiceCount);
    }
}
