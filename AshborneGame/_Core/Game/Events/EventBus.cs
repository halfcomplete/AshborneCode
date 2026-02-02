using System.Collections.Concurrent;

namespace AshborneGame._Core.Game.Events
{
    /// <summary>
    /// A thread-safe, strongly-typed event bus for publishing and subscribing to game events.
    /// Supports both synchronous and asynchronous handlers with disposable subscription tokens.
    /// </summary>
    /// <remarks>
    /// <para><b>Publishing events:</b></para>
    /// <code>
    /// // Simple publish
    /// EventBus.Publish(new PlayerMovedEvent(newLocation));
    /// 
    /// // Or using the extension method
    /// new PlayerMovedEvent(newLocation).Publish();
    /// </code>
    /// 
    /// <para><b>Subscribing to events:</b></para>
    /// <code>
    /// // Sync subscription with automatic cleanup via using
    /// using var token = EventBus.Subscribe&lt;PlayerMovedEvent&gt;(e => Console.WriteLine(e.Location));
    /// 
    /// // Async subscription
    /// var token = EventBus.SubscribeAsync&lt;PlayerMovedEvent&gt;(async e => await SaveLocationAsync(e.Location));
    /// // Later: token.Dispose();
    /// </code>
    /// </remarks>
    public static class EventBus
    {
        /// <summary>
        /// Internal record for storing both sync and async handlers together.
        /// </summary>
        private sealed class HandlerEntry
        {
            public Delegate Callback { get; }
            public bool IsAsync { get; }
            public EventToken Token { get; }

            public HandlerEntry(Delegate callback, bool isAsync, EventToken token)
            {
                Callback = callback;
                IsAsync = isAsync;
                Token = token;
            }
        }

        private static readonly ConcurrentDictionary<Type, List<HandlerEntry>> _subscribers = new();
        private static readonly object _lock = new();

        #region Subscribe (Sync)

        /// <summary>
        /// Subscribes to a game event with a strongly-typed synchronous callback.
        /// </summary>
        /// <typeparam name="TEvent">The event type to subscribe to.</typeparam>
        /// <param name="callback">The callback to invoke when the event is published.</param>
        /// <returns>A disposable token that unsubscribes when disposed.</returns>
        public static EventToken Subscribe<TEvent>(Action<TEvent> callback) where TEvent : IGameEvent
        {
            ArgumentNullException.ThrowIfNull(callback);

            var token = new EventToken(typeof(TEvent), callback, isAsync: false);
            var entry = new HandlerEntry(callback, isAsync: false, token);

            lock (_lock)
            {
                var list = _subscribers.GetOrAdd(typeof(TEvent), _ => new List<HandlerEntry>());
                list.Add(entry);
            }

            return token;
        }

        #endregion

        #region Subscribe (Async)

        /// <summary>
        /// Subscribes to a game event with a strongly-typed asynchronous callback.
        /// </summary>
        /// <typeparam name="TEvent">The event type to subscribe to.</typeparam>
        /// <param name="callback">The async callback to invoke when the event is published.</param>
        /// <returns>A disposable token that unsubscribes when disposed.</returns>
        public static EventToken SubscribeAsync<TEvent>(Func<TEvent, Task> callback) where TEvent : IGameEvent
        {
            ArgumentNullException.ThrowIfNull(callback);

            var token = new EventToken(typeof(TEvent), callback, isAsync: true);
            var entry = new HandlerEntry(callback, isAsync: true, token);

            lock (_lock)
            {
                var list = _subscribers.GetOrAdd(typeof(TEvent), _ => new List<HandlerEntry>());
                list.Add(entry);
            }

            return token;
        }

        #endregion

        #region Unsubscribe

        /// <summary>
        /// Unsubscribes using the provided token. Called automatically by EventToken.Dispose().
        /// </summary>
        /// <param name="token">The subscription token to remove.</param>
        internal static void Unsubscribe(EventToken token)
        {
            lock (_lock)
            {
                if (_subscribers.TryGetValue(token.EventType, out var list))
                {
                    list.RemoveAll(e => ReferenceEquals(e.Token, token));

                    if (list.Count == 0)
                    {
                        _subscribers.TryRemove(token.EventType, out _);
                    }
                }
            }
        }

        #endregion

        #region Publish (Sync)

        /// <summary>
        /// Publishes an event to all subscribers. Sync handlers run immediately;
        /// async handlers are fired and awaited sequentially.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="gameEvent">The event instance to publish.</param>
        /// <remarks>
        /// If the event's OneTime property is true, all subscribers are removed after invocation.
        /// This method blocks until all handlers (including async) have completed.
        /// For fire-and-forget publishing of async handlers, use PublishFireAndForget.
        /// </remarks>
        public static void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            ArgumentNullException.ThrowIfNull(gameEvent);
            PublishAsync(gameEvent).GetAwaiter().GetResult();
        }

        #endregion

        #region Publish (Async)

        /// <summary>
        /// Publishes an event to all subscribers asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="gameEvent">The event instance to publish.</param>
        /// <returns>A task that completes when all handlers have finished.</returns>
        public static async Task PublishAsync<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            ArgumentNullException.ThrowIfNull(gameEvent);

            List<HandlerEntry> snapshot;

            lock (_lock)
            {
                if (!_subscribers.TryGetValue(typeof(TEvent), out var list))
                    return;

                // Take a snapshot to avoid issues if handlers modify subscriptions
                snapshot = new List<HandlerEntry>(list);

                // If OneTime, remove ALL subscribers now (before invoking)
                if (gameEvent.OneTime)
                {
                    _subscribers.TryRemove(typeof(TEvent), out _);
                }
            }

            foreach (var entry in snapshot)
            {
                try
                {
                    if (entry.IsAsync)
                    {
                        var asyncCallback = (Func<TEvent, Task>)entry.Callback;
                        await asyncCallback(gameEvent).ConfigureAwait(false);
                    }
                    else
                    {
                        var syncCallback = (Action<TEvent>)entry.Callback;
                        syncCallback(gameEvent);
                    }
                }
                catch (Exception ex)
                {
                    // Log but don't throw - one handler failure shouldn't break others
                    Console.WriteLine($"[EventBus] Handler error for {typeof(TEvent).Name}: {ex.Message}");
                }
            }
        }

        /// <summary>
        /// Publishes an event without waiting for async handlers to complete.
        /// Sync handlers still run immediately on the calling thread.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="gameEvent">The event instance to publish.</param>
        public static void PublishFireAndForget<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            ArgumentNullException.ThrowIfNull(gameEvent);
            _ = PublishAsync(gameEvent);
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Clears all subscriptions. Useful for testing or game reset.
        /// </summary>
        public static void Clear()
        {
            lock (_lock)
            {
                _subscribers.Clear();
            }
        }

        /// <summary>
        /// Gets the number of subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type to check.</typeparam>
        /// <returns>The number of active subscriptions.</returns>
        public static int GetSubscriberCount<TEvent>() where TEvent : IGameEvent
        {
            lock (_lock)
            {
                return _subscribers.TryGetValue(typeof(TEvent), out var list) ? list.Count : 0;
            }
        }

        /// <summary>
        /// Checks if there are any subscribers for a specific event type.
        /// </summary>
        /// <typeparam name="TEvent">The event type to check.</typeparam>
        /// <returns>True if there are subscribers, false otherwise.</returns>
        public static bool HasSubscribers<TEvent>() where TEvent : IGameEvent
        {
            return GetSubscriberCount<TEvent>() > 0;
        }

        #endregion
    }

    /// <summary>
    /// Extension methods for convenient event publishing.
    /// </summary>
    public static class GameEventExtensions
    {
        /// <summary>
        /// Publishes this event to the EventBus.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="gameEvent">The event to publish.</param>
        public static void Publish<TEvent>(this TEvent gameEvent) where TEvent : IGameEvent
        {
            EventBus.Publish(gameEvent);
        }

        /// <summary>
        /// Publishes this event to the EventBus asynchronously.
        /// </summary>
        /// <typeparam name="TEvent">The event type.</typeparam>
        /// <param name="gameEvent">The event to publish.</param>
        /// <returns>A task that completes when all handlers have finished.</returns>
        public static Task PublishAsync<TEvent>(this TEvent gameEvent) where TEvent : IGameEvent
        {
            return EventBus.PublishAsync(gameEvent);
        }
    }
}
