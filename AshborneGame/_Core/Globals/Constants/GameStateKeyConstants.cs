
namespace AshborneGame._Core.Globals.Constants
{
    public readonly struct GameStateKey<T>
    {
        public string Key { get; }
        public GameStateKey(string key) => Key = key;
        public override string ToString() => Key;

        public static implicit operator string(GameStateKey<T> key) => key.Key;
    }

    public static class GameStateKeyConstants
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

            public static class Events
            {
                public const string SomeEventKey = "Events.SomeEventKey";
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
