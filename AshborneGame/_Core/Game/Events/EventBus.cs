using System;
using System.Collections.Generic;
using AshborneGame._Core.Game.Events;

namespace AshborneGame._Core.Game.Events
{
    public static class EventBus
    {
        private static readonly Dictionary<Type, List<Action<IGameEvent>>> subscribers = new();

        /// <summary>
        /// Subscribes to a game event with a callback to be invoked when the event is called.
        /// </summary>
        /// <typeparam name="TEvent">The type of the game event to subscribe to.</typeparam>
        /// <param name="callback">The callback to invoke when the event is called.</param>
        public static EventSubscription Subscribe<TEvent>(Action<IGameEvent> callback) where TEvent : IGameEvent
        {
            if (!subscribers.ContainsKey(typeof(TEvent))) subscribers[typeof(TEvent)] = new List<Action<IGameEvent>>();
            subscribers[typeof(TEvent)].Add(callback);
            return new EventSubscription(typeof(TEvent), callback);
        }

        /// <summary>
        /// Unsubscribes from a game event with the specified callback.
        /// </summary>
        /// <param name="subscription">The subscription to remove.</param>
        public static void Unsubscribe(EventSubscription subscription)
        {
            if (subscribers.TryGetValue(subscription.EventType, out var list))
            {
                list.Remove(subscription.Callback);

                if (list.Count == 0)
                    subscribers.Remove(subscription.EventType);
            }
        }

        /// <summary>
        /// Publishes a game event to all subscribed handlers.
        /// </summary>
        /// <typeparam name="TEvent">The type of the game event to publish.</typeparam>
        /// <param name="gameEvent">The game event instance to publish.</param>
        public static void Publish<TEvent>(TEvent gameEvent) where TEvent : IGameEvent
        {
            if (subscribers.TryGetValue(typeof(TEvent), out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(gameEvent);
                }
            }

            if (gameEvent.OneTime)
            {
                subscribers.Remove(typeof(TEvent));
            }
        }

        public static void Clear()
        {
            subscribers.Clear();
        }
    }
}
