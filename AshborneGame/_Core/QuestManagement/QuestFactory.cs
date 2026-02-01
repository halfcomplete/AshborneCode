using AshborneGame._Core.Game;

namespace AshborneGame._Core.QuestManagement
{
    public static class QuestFactory
    {
        public static Quest CreateQuest(
            string name,
            string description,
            Action<GameStateManager> onComplete,
            Action<GameStateManager>? onFail = null,
            params QuestCriteria[] criteria)
        {
            List<QuestCriteria> completionCriteriaList = new List<QuestCriteria>();
            List<QuestCriteria> failureCriteriaList = new List<QuestCriteria>();
            foreach (var c in criteria)
            {
                if (c.IsCompletionCriteria)
                {
                    if (c.CriteriaFunction == null || c.TimeBasedCriteriaFunction == null)
                    {
                        throw new ArgumentException("Completion criteria must have a defined CriteriaFunction.");
                    }
                    completionCriteriaList.Add(c);
                }
                else
                {
                    if (c.CriteriaFunction == null || c.TimeBasedCriteriaFunction == null)
                    {
                        throw new ArgumentException("Failure criteria must have a defined CriteriaFunction.");
                    }
                    failureCriteriaList.Add(c);
                }
            }
            var questProgress = new QuestProgressTracker(completionCriteriaList, failureCriteriaList);
            return new Quest(name, description, onComplete, onFail, questProgress);
        }
    }
}