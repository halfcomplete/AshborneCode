
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
                public static class In
                {
                    public static class OssuaryOfEyes
                    {
                        public static class TalkedTo
                        {
                            public static readonly GameStateKey<bool> Ossaneth = new("Flags.Player.In.OssuaryOfEyes.TalkedTo.Ossaneth");
                            public static readonly GameStateKey<bool> OssanethHelp = new("Flags.Player.In.OssuaryOfEyes.TalkedTo.OssanethHelp");
                            public static readonly GameStateKey<bool> OssanethSelf = new("Flags.Player.In.OssuaryOfEyes.TalkedTo.OssanethSelf");
                            public static readonly GameStateKey<bool> OssanethPast = new("Flags.Player.In.OssuaryOfEyes.TalkedTo.OssanethPast");
                        }
                        public static readonly GameStateKey<bool> SteppedOnPlant = new("Flags.Player.In.OssuaryOfEyes.SteppedOnPlant");
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
                    public static readonly GameStateKey<int> Gardener = new("Counters.Player.TimesEncountered.Gardener");
                    public static readonly GameStateKey<int> Keeper = new("Counters.Player.TimesEncountered.Keeper");
                }

                public static class TimesInspected
                {
                    public static readonly GameStateKey<int> CarvedEye = new("Counters.Player.TimesInspected.CarvedEye");
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
