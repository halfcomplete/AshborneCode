using System;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;


namespace AshborneGame._Core.Game.DescriptionHandling
{
    /// <summary>
    /// Fluent DSL for conditional descriptions. Usage examples:
    /// ConditionalDescription.If(...).ThenShow(...);
    /// ConditionalDescription.IfNot(...).ThenShow(...);
    /// ConditionalDescription.If(...).And().If(...).ThenShow(...);
    /// ConditionalDescription.If(...).Or().Group(ConditionalDescription.If(...).And().IfNot(...)).ThenShow(...);
    /// </summary>
    public class ConditionalDescription
    {
        public Func<Player, GameStateManager, bool> Predicate { get; private set; }
        public string Message { get; private set; } = string.Empty;
        public bool OneTime { get; private set; }
        private bool _started;
        private bool _messageAssigned;


        private ConditionalDescription() { }

        public static ConditionalDescription StartNew()
        {
            return new ConditionalDescription();
        }

        /// <summary>
        /// Starts the conditional description with a predicate that must be true.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ConditionalDescription If(Func<Player, GameStateManager, bool> predicate)
        {
            return StartConditional(predicate, ClauseTypes.If);
        }

        /// <summary>
        /// Starts the conditional description with a predicate that must be false.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ConditionalDescription IfNot(Func<Player, GameStateManager, bool> predicate)
        {
            return StartConditional(predicate, ClauseTypes.IfNot);
        }

        /// <summary>
        /// Combines the current predicate with the provided one using logical AND.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ConditionalDescription AndIf(Func<Player, GameStateManager, bool> predicate)
        {
            EnsureStarted();
            CombineCurrentPredicateWith(predicate, ClauseTypes.And);
            return this;
        }

        /// <summary>
        /// Combines the current predicate with a grouped conditional description using logical AND.
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ConditionalDescription AndIfAll(ConditionalDescription grouped)
        {
            ArgumentNullException.ThrowIfNull(grouped);
            EnsureStarted();
            var nextExpression = grouped.Predicate;
            if (nextExpression is null)
                throw new InvalidOperationException("Grouped condition must start with If/IfNot.");
            CombineCurrentPredicateWith(nextExpression, ClauseTypes.And);
            return this;
        }

        /// <summary>
        /// Combines the current predicate with the provided one using logical OR.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public ConditionalDescription OrIf(Func<Player, GameStateManager, bool> predicate)
        {
            EnsureStarted();
            CombineCurrentPredicateWith(predicate, ClauseTypes.Or);
            return this;
        }

        /// <summary>
        /// Combines the current predicate with a grouped conditional description using logical OR.
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ConditionalDescription OrIfAll(ConditionalDescription grouped)
        {
            ArgumentNullException.ThrowIfNull(grouped);
            EnsureStarted();
            var nextExpression = grouped.Predicate;
            if (nextExpression is null)
                throw new InvalidOperationException("Grouped condition must start with If/IfNot.");
            CombineCurrentPredicateWith(nextExpression, ClauseTypes.Or);
            return this;
        }

        /// <summary>
        /// Sets the message to show when the condition is met.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public ConditionalDescription ThenShow(string message)
        {
            EnsureStarted();
            if (string.IsNullOrWhiteSpace(message))
                throw new ArgumentException("Message cannot be null or empty", nameof(message));

            Message = message;
            _messageAssigned = true;
            return this;
        }

        /// <summary>
        /// Sets the description to be shown only once.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ConditionalDescription OnlyOnce()
        {
            if (!_messageAssigned)
                throw new InvalidOperationException("Call ThenShow(...) before OnlyOnce().");
            OneTime = true;
            return this;
        }

        /// <summary>
        /// Sets the description to be shown every time the condition is met.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public ConditionalDescription Everytime()
        {
            if (!_messageAssigned)
                throw new InvalidOperationException("Call ThenShow(...) before Everytime().");
            OneTime = false;
            return this;
        }

        /// <summary>
        /// Gets the description based on the evaluation of the predicate.
        /// </summary>
        /// <param name="oneTime"></param>
        /// <returns></returns>
        public string GetDescription(out bool oneTime)
        {
            
            if (!_messageAssigned)
            {
                oneTime = true;
                return string.Empty;
            }

            if (Predicate(GameContext.Player, GameContext.GameState))
            {
                var msg = Message ?? string.Empty;
                oneTime = OneTime;
                return msg;
            }

            oneTime = false;
            return string.Empty;
        }

        private ConditionalDescription StartConditional(Func<Player, GameStateManager, bool> predicate, ClauseTypes clauseType)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var expr = clauseType == ClauseTypes.If
                ? predicate
                : (p, g) => !predicate(p, g);


            if (!_started)
            {
                Predicate = expr;
                _started = true;
                return this;
            }
            
            throw new InvalidOperationException("Call If(...) or IfNot(...) only once at the beginning.");
        }

        /// <summary>
        /// Combines the current predicate with the provided one using the given clause type.
        /// </summary>
        /// <param name="next">The next predicate to combine the current one with.</param>
        /// <param name="clauseType">The type of clause to use for combining predicates.</param>
        /// <exception cref="InvalidOperationException"><Thrown when clauseType is not And or Or./exception>
        private void CombineCurrentPredicateWith(Func<Player, GameStateManager, bool> next, ClauseTypes clauseType)
        {
            var current = Predicate;
            if (current is null)
                throw new InvalidOperationException("No existing predicate to combine. Call If(...) or IfNot(...) first.");

            Predicate = clauseType switch
            {
                ClauseTypes.And => (p, g) => current(p, g) && next(p, g),
                ClauseTypes.Or => (p, g) => current(p, g) || next(p, g),
                _ => throw new InvalidOperationException("Unsupported operator state.")
            };
        }

        /// <summary>
        /// Ensures that the conditional description has been started with If or IfNot.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the conditional description has not been started with If or IfNot.</exception>
        private void EnsureStarted()
        {
            if (!_started)
                throw new InvalidOperationException("Call If(...) or IfNot(...) first.");
        }
    }
}