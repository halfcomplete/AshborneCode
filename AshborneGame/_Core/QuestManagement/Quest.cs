using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.QuestManagement
{
    public class Quest
    {
        public string Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; private set; } = QuestStatus.InProgress;
        private QuestProgressTracker _questProgress;
        private Action<GameStateManager> _onComplete;
        private Action<GameStateManager>? _onFail;

        public Quest(
            string name, 
            string description, 
            List<Func<GameStateManager, TimeSpan, bool>> completionCriteria, 
            Action<GameStateManager> onComplete, 
            List<Func<GameStateManager, TimeSpan, bool>>? failureCriteria = null, 
            Action<GameStateManager>? onFail = null)
        {
            Name = name;
            Id = SlugIdService.GenerateSlugId(name, "quest");
            Description = description;
            _questProgress = new QuestProgressTracker(completionCriteria, failureCriteria);
            _onComplete = onComplete;
            _onFail = onFail;
        }

        /// <summary>
        /// Ticks the quest progress, updating its status based on completion and failure criteria. Will invoke the appropriate callbacks on completion or failure.
        /// </summary>
        /// <param name="delta">The time elapsed since the last tick.</param>
        /// <param name="state">The current game state manager.</param>
        public void TickProgress(TimeSpan delta, GameStateManager state)
        {
            if (Status != QuestStatus.InProgress)
            {
                return;
            }

            if (_questProgress.IsQuestFailed(delta, state))
            {
                Status = QuestStatus.Failed;
                _onFail?.Invoke(state);
                return;
            }

            if (_questProgress.IsQuestComplete(delta, state))
            {
                Status = QuestStatus.Completed;
                _onComplete?.Invoke(state);
                return;
            }
        }
    }
}    
