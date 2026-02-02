namespace AshborneGame._Core.Game.Events
{
    public readonly struct EventSubscription
    {
        public Type EventType { get; }
        public Action<IGameEvent> Callback { get; }

        public EventSubscription(Type eventType, Action<IGameEvent> callback)
        {
            EventType = eventType;
            Callback = callback;
        }
    }
}