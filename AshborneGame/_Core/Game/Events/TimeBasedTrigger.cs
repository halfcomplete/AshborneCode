using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Game.Events
{
    public class LocationTimeTrigger
    {
        public string LocationName { get; }
        /// <summary>
        /// The time, in ticks, that the player must spend in the location before the trigger activates.
        /// </summary>
        public int Duration { get; }
        public IGameEvent EventToRaise { get; }
        public bool OneTime { get; }
        public bool Triggered { get; private set; }
        public Action? Effect { get; }

        public LocationTimeTrigger(string locationName, int duration, IGameEvent eventToRaise, Action? effect = null, bool oneTime = true)
        {
            LocationName = locationName;
            Duration = duration;
            EventToRaise = eventToRaise;
            Effect = effect;
            OneTime = oneTime;
        }

        public bool CheckTrigger(Location? currentLocation, int ticksSpentInCurrentLocation)
        {
            if (OneTime && Triggered) return false;

            if (currentLocation?.Name.ReferenceName == LocationName && ticksSpentInCurrentLocation >= Duration)
            {
                Triggered = true;
                return true;
            }

            return false;
        }
    }
}
