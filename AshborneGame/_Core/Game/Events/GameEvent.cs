using System.Collections.Generic;

namespace AshborneGame._Core.Game.Events
{
    public class GameEvent
    {
        public string Name { get; }
        public Dictionary<string, object> Data { get; }

        public GameEvent(string name, Dictionary<string, object>? data = null)
        {
            Name = name;
            Data = data ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// Retrieves data about the event
        /// </summary>
        public T Get<T>(string key)
        {
            return Data.TryGetValue(key, out var value) ? (T)value : default!;
        }

        public bool Has(string key) => Data.ContainsKey(key);
    }
}
