using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.QuestManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game
{
    public class QuestTracker
    {
        private List<Quest> _quests = new List<Quest>();

        public void AddQuest(Quest quest)
        {
            _quests.Add(quest);
        }

        public void TickQuestTimeTracking(int hoursPassed)
        {
            foreach (var quest in _quests)
            {
                if (quest.Status == QuestStatus.InProgress)
                {
                    quest.TickQuestTime(hoursPassed, GameContext.GameState);
                    switch (quest.Status)
                    {
                        case QuestStatus.Completed:
                            IOService.Output.DisplayDebugMessage($"[QuestTracker] Quest Completed: {quest.Name} (ID: {quest.ID})", Globals.Enums.ConsoleMessageTypes.INFO);
                            break;
                        case QuestStatus.Failed:
                            IOService.Output.DisplayDebugMessage($"[QuestTracker] Quest Failed: {quest.Name} (ID: {quest.ID})", Globals.Enums.ConsoleMessageTypes.WARNING);
                            break;
                    }
                }
            }
        }
    }
}
