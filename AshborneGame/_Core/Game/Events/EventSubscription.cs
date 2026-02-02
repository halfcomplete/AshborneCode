namespace AshborneGame._Core.Game.Events
{
    /// <summary>
    /// A disposable subscription token that automatically unsubscribes from the EventBus
    /// when disposed. Use with 'using' statements or store and call Dispose() manually.
    /// </summary>
    /// <remarks>
    /// Thread-safe: Disposal is idempotent and can be called multiple times safely.
    /// </remarks>
    public sealed class EventToken : IDisposable
    {
        private readonly Type _eventType;
        private readonly Delegate _callback;
        private readonly bool _isAsync;
        private int _disposed;

        internal EventToken(Type eventType, Delegate callback, bool isAsync)
        {
            _eventType = eventType;
            _callback = callback;
            _isAsync = isAsync;
        }

        /// <summary>
        /// The type of event this token is subscribed to.
        /// </summary>
        public Type EventType => _eventType;

        /// <summary>
        /// Whether this subscription has been disposed (unsubscribed).
        /// </summary>
        public bool IsDisposed => Volatile.Read(ref _disposed) == 1;

        /// <summary>
        /// Whether this is an async callback subscription.
        /// </summary>
        public bool IsAsync => _isAsync;

        internal Delegate Callback => _callback;

        /// <summary>
        /// Unsubscribes from the event. Safe to call multiple times.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                EventBus.Unsubscribe(this);
            }
        }
    }

    /// <summary>
    /// A composite token that manages multiple subscriptions as a single unit.
    /// Disposing this token disposes all contained subscriptions.
    /// </summary>
    public sealed class CompositeEventToken : IDisposable
    {
        private readonly List<EventToken> _tokens = new();
        private int _disposed;

        /// <summary>
        /// Adds a token to this composite. The token will be disposed when this composite is disposed.
        /// </summary>
        public void Add(EventToken token)
        {
            if (Volatile.Read(ref _disposed) == 1)
            {
                token.Dispose();
                return;
            }

            lock (_tokens)
            {
                if (Volatile.Read(ref _disposed) == 1)
                {
                    token.Dispose();
                    return;
                }
                _tokens.Add(token);
            }
        }

        /// <summary>
        /// Disposes all contained tokens.
        /// </summary>
        public void Dispose()
        {
            if (Interlocked.CompareExchange(ref _disposed, 1, 0) == 0)
            {
                lock (_tokens)
                {
                    foreach (var token in _tokens)
                    {
                        token.Dispose();
                    }
                    _tokens.Clear();
                }
            }
        }
    }
}