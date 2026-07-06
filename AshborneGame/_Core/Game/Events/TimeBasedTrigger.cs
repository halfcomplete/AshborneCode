using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.Events
{
    public class TimeBasedTrigger
    {
        public DefinitionID LocationID { get; }
        /// <summary>
        /// The time, in ticks, that the player must spend in the location before the trigger activates.
        /// </summary>
        public int Duration { get; }
        public IGameEvent EventToRaise { get; }
        public bool OneTime { get; }
        public bool Triggered { get; private set; }
        public Action? Effect { get; }

        public TimeBasedTrigger(DefinitionID locationID, int duration, IGameEvent eventToRaise, Action? effect = null, bool oneTime = true)
        {
            LocationID = locationID;
            Duration = duration;
            EventToRaise = eventToRaise;
            Effect = effect;
            OneTime = oneTime;
        }

        public bool CheckTrigger(DefinitionID? currentLocationID, int ticksSpentInCurrentLocation)
        {
            if (OneTime && Triggered) return false;

            if (currentLocationID == LocationID && ticksSpentInCurrentLocation >= Duration)
            {
                Triggered = true;
                return true;
            }

            return false;
        }
    }
}
