using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.Events
{
    public class EventBus
    {
        private readonly Dictionary<string, List<Action<GameEvent>>> subscribers = new();

        public void Subscribe(string eventName, Action<GameEvent> callback)
        {
            if (!subscribers.ContainsKey(eventName)) subscribers[eventName] = new List<Action<GameEvent>>();
            subscribers[eventName].Add(callback);
        }

        public void Unsubscribe(string eventName, Action<GameEvent> callback)
        {
            subscribers[eventName].Remove(callback);
            if (subscribers[eventName].Count == 0) subscribers.Remove(eventName);
        }

        public void Call(GameEvent gameEvent)
        {
            if (subscribers.TryGetValue(gameEvent.Name, out var handlers))
            {
                foreach (var handler in handlers)
                {
                    handler(gameEvent);
                }
            }
        }

        public void Clear()
        {
            subscribers.Clear();
        }
    }
}
