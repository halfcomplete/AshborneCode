using AshborneGame._Core._Player;

namespace AshborneGame._Core.Globals.Interfaces
{
    public interface ICommand
    {
        List<string> Names { get; }
        string Description { get; }
        Task<bool> TryExecute(List<string> arguments, Player player);
    }
}
