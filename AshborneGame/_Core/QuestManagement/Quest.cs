using AshborneGame._Core.Game;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Services;
using System;

namespace AshborneGame._Core.QuestManagement
{
    public class Quest
    {
        public string ID { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        public QuestStatus Status { get; private set; } = QuestStatus.InProgress;

        

        private Quest(
            string name, 
            string description,
            Action<GameStateManager> onComplete,
            Action<GameStateManager>? onFail)
        {
            Name = name;
            ID = SlugIdService.GenerateSlugId(name, "quest");
            Description = description;
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
