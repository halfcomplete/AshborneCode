
namespace AshborneGame._Core.Globals.Constants
{
    public readonly struct GameStateKey
    {
        public string Key { get; }
        public GameStateKey(string key) => Key = key;
        public override string ToString() => Key;

        public static implicit operator string(GameStateKey key) => key.Key;
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
                        public static readonly GameStateKey OssanethDreamspace_VisitedHallwayOfMirrors = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedHallwayOfMirrors");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound
                        /// </summary>
                        public static readonly GameStateKey OssanethDreamspace_VisitedTempleOfTheBound = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedTempleOfTheBound");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedThroneRoom
                        /// </summary>
                        public static readonly GameStateKey OssanethDreamspace_VisitedThroneRoom = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedThroneRoom");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks
                        /// </summary>
                        public static readonly GameStateKey OssanethDreamspace_VisitedBloodClocks = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks");
                        /// <summary>
                        /// Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks
                        /// </summary>
                        public static readonly GameStateKey OssanethDreamspace_TalkedToBoundOne = new("Flags.Player.Actions.In.OssanethDreamspace_VisitedBloodClocks");
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
                /// The player's current scene number.
                /// </summary>
                public static readonly GameStateKey CurrentSceneNo = new("Counters.Player.CurrentSceneNo");
                /// <summary>
                /// The player's current act number.
                /// </summary>
                public static readonly GameStateKey CurrentActNo = new("Counters.Player.CurrentActNo");
            }
        }
    }
}
