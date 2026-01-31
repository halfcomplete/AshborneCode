using AshborneGame._Core.Game;

namespace AshborneGame._Core.QuestManagement
{
    public class QuestProgressTracker
    {
        private List<Func<GameStateManager, TimeSpan, bool>> _completionCriteria;
        private List<Func<GameStateManager, TimeSpan, bool>>? _failureCriteria;

        /// <summary>
        /// Creates a new QuestProgressTracker with the given completion and failure criteria.
        /// </summary>
        /// <param name="completionCriteria">A list of functions that determine if the quest is complete. Each function takes a GameStateManager and a TimeSpan and returns a bool.</param>
        /// <param name="failureCriteria">An optional list of functions that determine if the quest has failed. Each function takes a GameStateManager and a TimeSpan and returns a bool.</param>
        public QuestProgressTracker(List<Func<GameStateManager, TimeSpan, bool>> completionCriteria, List<Func<GameStateManager, TimeSpan, bool>>? failureCriteria = null)
        {
            _completionCriteria = completionCriteria;
            _failureCriteria = failureCriteria;
        }
        
        public bool IsQuestComplete(TimeSpan delta, GameStateManager gameStateManager)
        {
            return _completionCriteria.All(criteria => criteria(gameStateManager, delta));
        }
        
        public bool IsQuestFailed(TimeSpan delta, GameStateManager gameStateManager)
        {
            return _failureCriteria != null && _failureCriteria.Any(criteria => criteria(gameStateManager, delta));
        }
    }
}