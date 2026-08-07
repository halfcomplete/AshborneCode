using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.QuestManagement.Criteria.CriteriaDefinitions
{
    public record TimeCriteria : CriteriaDefinition
    {
        public override CriteriaDefinitionType Type { get => CriteriaDefinitionType.TimeCriteria; }

        private int _hoursPassedRequirement = 0;
        private int _totalHoursAtTimeOfCreation = 0;

        public TimeCriteria(int totalHours, int hoursPassed)
        {
            _totalHoursAtTimeOfCreation = totalHours;
            _hoursPassedRequirement = hoursPassed;
        }

        public override bool IsCompleted(Player player, GameStateManager gameState)
        {
            return (gameState.TimeTracker.TotalInGameHours - _totalHoursAtTimeOfCreation) > _hoursPassedRequirement;
        }
    }
}