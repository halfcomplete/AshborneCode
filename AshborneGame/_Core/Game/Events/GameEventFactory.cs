namespace AshborneGame._Core.Game.Events
{
    public static class GameEventFactory
    {
        public static class OssanethsDomain
        {
            /// <summary>
            /// Creates a game event for when the outro sequence of Ossaneth's Domain is triggered.
            /// </summary>
            public static OssanethsDomainOutroTriggeredEvent CreateOssanethsDomainOutroTriggeredEvent() => new OssanethsDomainOutroTriggeredEvent();
            public sealed record OssanethsDomainOutroTriggeredEvent() : IGameEvent
            {
                public bool OneTime => true;
            }
        }
    }
}