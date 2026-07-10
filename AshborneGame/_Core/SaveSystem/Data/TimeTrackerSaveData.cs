using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.SaveSystem.Data
{
    public class TimeTrackerSaveData
    {
        public DefinitionID? CurrentLocation { get; set; }
        public int TicksSinceLastHourAdvance { get; set; }
        public int TicksInCurrentLocation { get; set; }
        public int TotalTicksInCurrentLocation { get; set; }
        public Dictionary<DefinitionID, int> LocationDurations { get; set; } = new();
        public List<TimeBasedTrigger> LocationTimeTriggers { get; set; } = new();
        public int TotalInGameHours { get; set; }
    }
}