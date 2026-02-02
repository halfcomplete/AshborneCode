using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.SceneManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.Events
{
    public class LocationTimeTrigger
    {
        public string LocationName { get; }
        public TimeSpan Duration { get; }
        public IGameEvent EventToRaise { get; }
        public bool OneTime { get; }
        public bool Triggered { get; private set; }
        public Action? Effect { get; }

        public LocationTimeTrigger(string locationName, TimeSpan duration, IGameEvent eventToRaise, Action? effect = null, bool oneTime = true)
        {
            LocationName = locationName;
            Duration = duration;
            EventToRaise = eventToRaise;
            Effect = effect;
            OneTime = oneTime;
        }

        public bool CheckTrigger(ILocation? currentLocation, TimeSpan currentLocationTime)
        {
            if (OneTime && Triggered) return false;

            if (currentLocation?.Name.ReferenceName == LocationName && currentLocationTime >= Duration)
            {
                Triggered = true;
                return true;
            }

            return false;
        }
    }
}
