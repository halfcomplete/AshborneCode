using AshborneGame._Core._Player;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules
{
    public interface IDescribable  
    {
        List<(Func<GameStateManager, bool> condition, string description)> Conditions { get; }

        void AddCondition(Func<GameStateManager, bool> condition, string description)
        {
            Conditions.Add((condition, description));
        }

        string GetDescription(Player player, GameStateManager state)
        {
            foreach (var (condition, desc) in Conditions)
            {
                if (condition(state))
                    return desc;
            }

            return string.Empty;
        }
    }
}
