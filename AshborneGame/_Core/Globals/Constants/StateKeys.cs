
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
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDreamspace_VisitedHallwayOfMirrors = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDreamspace_VisitedTempleOfTheBound = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedThroneRoom
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDreamspace_VisitedThroneRoom = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedThroneRoom");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDomain_SatOnThrone
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDomain_SatOnThrone = new("Flags.Player.Actions.In.OssanethDomain_SatOnThrone");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDomain_TouchedKnife
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDomain_TouchedKnife = new("Flags.Player.Actions.In.OssanethDomain_TouchedKnife");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDreamspace_VisitedBloodClocks = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks
                        /// </summary>
                        public static readonly GameStateKey<bool> OssanethDreamspace_TalkedToBoundOne = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks");
                    }
                }
            }
        }

        public static class Counters
        {
            public static class Player
            {
                /// <summary>
                /// Counters.Player.CurrentSceneNo
                /// </summary>
                public static readonly GameStateKey<int> CurrentSceneNo = new("Counters.Player.CurrentSceneNo");
                /// <summary>
                /// Counters.Player.CurrentActNo
                /// </summary>
                public static readonly GameStateKey<int> CurrentActNo = new("Counters.Player.CurrentActNo");
                /// <summary>
                /// Counters.Player.Prayers
                /// </summary>
                public static readonly GameStateKey<int> Prayers = new("Counters.Player.Prayers");
            }
        }

        public static class Labels
        {
            public static class Player
            {
                /// <summary>
                /// Labels.Player.Name
                /// </summary>
                public static readonly GameStateKey<string> Name = new("Labels.Player.Name");
                /// <summary>
                /// Labels.Player.Input
                /// </summary>
                public static readonly GameStateKey<string> Input = new("Labels.Player.Input");
            }
        }
    }
}
