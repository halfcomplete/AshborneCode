using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    internal class DescribableBehaviour : IDescribable
    {
        public List<(Func<GameStateManager, bool> condition, string description)> Conditions { get; } = new();

        public void AddCondition(Func<GameStateManager, bool> condition, string description)
        {
            Conditions.Add((condition, description));
        }

        public string GetDescription(Player player, GameStateManager state)
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
