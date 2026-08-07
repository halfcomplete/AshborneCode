using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.QuestManagement.Criteria
{
    public abstract record CriteriaDefinition
    {
        public abstract CriteriaDefinitionType Type { get; }

        public abstract bool IsCompleted(Player player, GameStateManager gameState);
    }
}
