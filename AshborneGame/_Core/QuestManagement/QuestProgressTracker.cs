using AshborneGame._Core.Game;

namespace AshborneGame._Core.QuestManagement
{
    public class QuestProgressTracker
    {
        private List<QuestCriteria> _completionCriteria;
        private List<QuestCriteria>? _failureCriteria;

        /// <summary>
        /// Creates a new QuestProgressTracker with the given completion and failure criteria.
        /// </summary>
        /// <param name="completionCriteria">A list of functions that determine if the quest is complete. Each function takes a GameStateManager and a TimeSpan and returns a bool.</param>
        /// <param name="failureCriteria">An optional list of functions that determine if the quest has failed. Each function takes a GameStateManager and a TimeSpan and returns a bool.</param>
        public QuestProgressTracker(List<QuestCriteria> completionCriteria, List<QuestCriteria>? failureCriteria = null)
        {
            _completionCriteria = completionCriteria;
            _failureCriteria = failureCriteria;
        }
        
        /// <summary>
        /// Determines if the quest is complete based on the completion criteria.
        /// </summary>
        /// <param name="delta">The time elapsed since the last tick.</param>
        /// <param name="gameStateManager">The current game state manager.</param>
        /// <returns>True if the quest is complete; otherwise, false.</returns>
        public bool IsQuestComplete(TimeSpan delta, GameStateManager gameStateManager)
        {
            return _completionCriteria.All(c => c.Evaluate(gameStateManager, delta));
        }
        
        /// <summary>
        /// Determines if the quest has failed based on the failure criteria.
        /// </summary>
        /// <param name="delta">The time elapsed since the last tick.</param>
        /// <param name="gameStateManager">The current game state manager.</param>
        /// <returns>True if the quest has failed; otherwise, false.</returns>
        public bool IsQuestFailed(TimeSpan delta, GameStateManager gameStateManager)
        {
            return _failureCriteria?.Any(c => c.Evaluate(gameStateManager, delta)) ?? false;
        }

        /// <summary>
        /// Ticks the criteria, updating any time-based conditions.
        /// </summary>
        /// <param name="delta">The time elapsed since the last tick.</param>
        /// <param name="gameStateManager">The current game state manager.</param>
        public void TickCriteria(TimeSpan delta, GameStateManager gameStateManager)
        {
            _completionCriteria.ForEach(c => c.TickTimePassed(delta));
            _failureCriteria?.ForEach(c => c.TickTimePassed(delta));
        }
    }
}