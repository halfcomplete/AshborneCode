using AshborneGame._Core.Game;

namespace AshborneGame._Core.QuestManagement
{
    public class QuestProgressTracker
    {
        private QuestCriteria _completionCriteria;
        private QuestCriteria? _failureCriteria;

        /// <summary>
        /// Creates a new QuestProgressTracker with the given completion and failure criteria.
        /// </summary>
        /// <param name="completionCriteria">A list of functions that determine if the quest is complete. Each function takes a GameStateManager and a TimeSpan and returns a bool.</param>
        /// <param name="failureCriteria">An optional list of functions that determine if the quest has failed. Each function takes a GameStateManager and a TimeSpan and returns a bool.</param>
        public QuestProgressTracker(QuestCriteria completionCriteria, QuestCriteria? failureCriteria = null)
        {
            _completionCriteria = completionCriteria;
            _failureCriteria = failureCriteria;
        }
        
        public bool IsQuestComplete(TimeSpan delta, GameStateManager gameStateManager)
        {
            _completionCriteria.TickTimePassed(delta);
            return _completionCriteria.Evaluate(gameStateManager, delta);
        }
        
        public bool IsQuestFailed(TimeSpan delta, GameStateManager gameStateManager)
        {
            _failureCriteria?.TickTimePassed(delta);
            return _failureCriteria?.Evaluate(gameStateManager, delta) ?? false;
        }
    }
}