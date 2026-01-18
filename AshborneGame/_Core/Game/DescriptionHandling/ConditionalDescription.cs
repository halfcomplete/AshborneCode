using System;
using System.Collections.Generic;
using System.Linq;
using AshborneGame._Core._Player;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class ConditionalDescription
    {
        // A private nested class to store each clause
        private class Entry
        {
            public Func<Player, GameStateManager, bool> Predicate { get; set; }
            public string Message { get; set; }
            public bool OneTime { get; set; }
        }

        // Our internal store of clauses
        private readonly List<Entry> _entries = new();

        // You can start a new builder with Create()
        private ConditionalDescription() { }
        public static ConditionalDescription Create() => new ConditionalDescription();

        /// <summary>
        /// Takes a base predicate that will be evaluated.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException"></exception>
        public ConditionalDescription When(Func<Player, GameStateManager, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate));
            var e = new Entry { Predicate = predicate };
            _entries.Add(e);
            return this;
        }

        /// <summary>
        /// Adds a new predicate to the chain using the 'And' operator.
        /// </summary>
        /// <param name="next">The predicate to check next with the 'And' operator.</param>
        /// <returns>The current conditional chain.</returns>
        /// <exception cref="InvalidOperationException">Thrown when 'And' is the first method in the chain.</exception>
        public ConditionalDescription And(Func<Player, GameStateManager, bool> next)
        {
            if (_entries.Count == 0)
                throw new InvalidOperationException("Call When() before And()");
            var last = _entries.Last();
            var prev = last.Predicate;
            last.Predicate = (p, g) => prev(p, g) && next(p, g);
            return this;
        }

        /// <summary>
        /// Adds a new predicate to the chain using the 'Or' operator.
        /// </summary>
        /// <param name="next">The predicate to check next with the 'Or' operator.</param>
        /// <returns>The current conditional chain.</returns>
        /// <exception cref="InvalidOperationException">Thrown when 'Or' is the first method in the chain.</exception>
        public ConditionalDescription Or(Func<Player, GameStateManager, bool> next)
        {
            if (_entries.Count == 0)
                throw new InvalidOperationException("Call When() before Or()");
            var last = _entries.Last();
            var prev = last.Predicate;
            last.Predicate = (p, g) => prev(p, g) || next(p, g);
            return this;
        }

        /// <summary>
        /// Inverts the previous predicate.
        /// </summary>
        /// <returns>The current conditional chain.</returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ConditionalDescription Not()
        {
            if (_entries.Count == 0)
                throw new InvalidOperationException("Call When(...) before Not()");
            var last = _entries.Last();
            var prev = last.Predicate;
            last.Predicate = (p, g) => !prev(p, g);
            return this;
        }


        // Assign the message for the last When()
        public ConditionalDescription Show(string message)
        {
            if (_entries.Count == 0)
                throw new InvalidOperationException("Call When(...) before Show(...)");
            if (string.IsNullOrEmpty(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));
            _entries.Last().Message = message;
            return this;
        }

        // Mark the last clause as one-time
        public ConditionalDescription Once()
        {
            if (_entries.Count == 0)
                throw new InvalidOperationException("Call When(...) before Once()");
            _entries.Last().OneTime = true;
            return this;
        }

        // Mark the last clause as always (never removed)
        public ConditionalDescription Always()
        {
            if (_entries.Count == 0)
                throw new InvalidOperationException("Call When(...) before Always()");
            _entries.Last().OneTime = false;
            return this;
        }

        // Evaluate and return the first matching message
        public string GetDescription()
        {
            foreach (var entry in _entries.ToList())  // clone so we can remove safely
            {
                if (entry.Predicate(GameContext.Player, GameContext.GameState))
                {
                    var msg = entry.Message ?? string.Empty;
                    if (entry.OneTime)
                        _entries.Remove(entry);
                    return msg;
                }
            }
            return string.Empty;
        }
    }
}
