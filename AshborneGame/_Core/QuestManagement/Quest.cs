

using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.QuestManagement;
    public class Quest
    {
        private enum QuestStatus
        {
            Inactive,
            Active,
            Completed,
            Failed
        }
        public string Id { get; }
        public string Name { get; set; }
        public string Description { get; set; }
        private QuestStatus _status;

        public Quest(string name, string description)
        {
            Name = name;
            Id = SlugIdService.GenerateSlugId(name, "quest");
            Description = description;
            _status = QuestStatus.Inactive;
        }

        public void Activate()
        {
            if (_status == QuestStatus.Inactive)
            {
                _status = QuestStatus.Active;
            }
        }
    }

    
