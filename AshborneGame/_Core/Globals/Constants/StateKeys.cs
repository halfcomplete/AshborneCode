
using System.Runtime.InteropServices;

namespace AshborneGame._Core.Globals.Constants
{
    public readonly struct GameStateKey<T>
    {
        public string Key { get; }
        public GameStateKey(string key) => Key = key;
        public override string ToString() => Key;

        public static implicit operator string(GameStateKey<T> key) => key.Key;
        public static implicit operator GameStateKey<T>(string key) => new(key);
    }

    public static class StateKeys
    {
        public static class Flags
        {
            public static class Player
            {
                public static class Actions
                {
                    public static class In
                    {
                        public static class OssanethsDomain
                        {
                            public static readonly GameStateKey<bool> TalkedToBoundOne = new("Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOne");
                            public static readonly GameStateKey<bool> TalkedToBoundOneHelp = new("Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOneHelp");
                            public static readonly GameStateKey<bool> TalkedToBoundOneSelf = new("Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOneSelf");
                            public static readonly GameStateKey<bool> TalkedToBoundOnePast = new("Flags.Player.Actions.In.OssanethsDomain.TalkedToBoundOnePast");
                        }
                    }
                }

                public static class Received
                {
                    public static readonly GameStateKey<bool> OssanethMask = new("Flags.Player.Received.OssanethMask");
                }
            }
        }

        public static class Counters
        {
            public static class Player
            {
                public static readonly GameStateKey<int> CurrentSceneNo = new("Counters.Player.CurrentSceneNo");
                public static readonly GameStateKey<int> CurrentActNo = new("Counters.Player.CurrentActNo");
                public static readonly GameStateKey<int> Prayers = new("Counters.Player.Prayers");

                public static class TimesEncountered
                {
                    public static readonly GameStateKey<int> Ossaneth = new("Counters.Player.TimesEncountered.Ossaneth");
                    public static readonly GameStateKey<int> Witnesses = new("Counters.Player.TimesEncountered.Witnesses");
                }

                // NOTE: TimesVisited counters have been removed.
                // Location visit counts are now tracked directly on Location.VisitCount
                // and accessed via GameStateManager.GetLocationVisitCount(locationId) or
                // from Ink via getLocationVisits("location-slug") / incLocationVisits("location-slug").
                // Location IDs are slug-based (e.g., "Locations.eye-platform").
            }
        }

        public static class Labels
        {
            public static class Player
            {
                public static readonly GameStateKey<string> Name = new("Labels.Player.Name");
                public static readonly GameStateKey<string> Input = new("Labels.Player.Input");
            }
        }
    }
}
