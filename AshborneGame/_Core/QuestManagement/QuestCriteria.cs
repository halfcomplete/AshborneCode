using AshborneGame._Core.Game;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.QuestManagement
{
    // TODO: Add support for time-based criteria (e.g., "if 5 minutes have passed").
    public class QuestCriteria
    {
        public Func<GameStateManager, bool> CriteriaFunction { get; private set; }
        public Func<int, bool>? TimeBasedCriteriaFunction { get; private set; }

        // The current criteria being built. This is used internally during the building process before defining whether it's a completion or failure criteria.
        public Func<GameStateManager, bool>? WipCriteria { get; private set; } = null;

        private Action<GameStateManager> _onComplete;
        private Action<GameStateManager>? _onFail;

        public bool IsCompletionCriteria { get; private set; } = true;

        private int _hoursPassed = 0;

        private enum ClauseTypes
        {
            If,
            IfNot,
            And,
            Or
        }

        public bool OneTime { get; private set; }
        private bool _started;

        /// <summary>
        /// Evaluates the criteria against the provided GameStateManager and hours passed.
        /// </summary>
        /// <param name="gameStateManager">The game state manager to evaluate the criteria against.</param>
        /// <param name="hoursPassed">The number of hours that have passed.</param>
        /// <returns>True if the criteria is met; otherwise, false.</returns>
        /// <exception cref="UnreachableException">Raised when the criteria evaluation reaches an unreachable code path, which should never happen..</exception>
        public bool Evaluate(GameStateManager gameStateManager, int hoursPassed)
        {
            if (CriteriaFunction != null && CriteriaFunction.Invoke(gameStateManager))
            {
                if (TimeBasedCriteriaFunction == null)
                    return true;
                else if (TimeBasedCriteriaFunction.Invoke(hoursPassed))
                    return true;
            }

            if (TimeBasedCriteriaFunction != null && TimeBasedCriteriaFunction.Invoke(hoursPassed))
            {
                if (CriteriaFunction == null)
                    return true;
                else if (CriteriaFunction.Invoke(gameStateManager))
                    throw new UnreachableException("Somehow, the first time we checked, CriteriaFunction failed. The second time, it succeeded. We should never reach this code.");
            }

            return false;
        }

        public void ResetTimePassed()
        {
            _hoursPassed = 0;
        }

        public void TickTimePassed(int delta)
        {
            _hoursPassed += delta;
        }

        #region Builder Methods

        /// <summary>
        /// Starts the criteria with a predicate that must be true.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public QuestCriteria If(Func<GameStateManager, bool> predicate)
        {
            return StartCriteria(predicate, ClauseTypes.If);
        }

        /// <summary>
        /// Starts the criteria with a time-passed check.
        /// </summary>
        /// <param name="hours">The number of hours that must have passed.</param>
        /// <returns></returns>
        public QuestCriteria IfTimeHasPassed(int hours)
        {
            return If(g => _hoursPassed >= hours);
        }

        /// <summary>
        /// Starts the criteria with a predicate that must be false.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public QuestCriteria IfNot(Func<GameStateManager, bool> predicate)
        {
            return StartCriteria(predicate, ClauseTypes.IfNot);
        }

        /// <summary>
        /// Combines the current criteria with the provided one using logical AND.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public QuestCriteria AndIf(Func<GameStateManager, bool> predicate)
        {
            EnsureStarted();
            CombineCurrentPredicateWith(predicate, ClauseTypes.And);
            return this;
        }

        /// <summary>
        /// Combines the current criteria with a time-passed check using logical AND.
        /// </summary>
        /// <param name="hours">The number of hours that must have passed.</param>
        /// <returns></returns>
        public QuestCriteria AndIfTimeHasPassed(int hours)
        {
            return AndIf(g => _hoursPassed >= hours);
        }

        /// <summary>
        /// Combines the current predicate with a grouped QuestCriteria completion criteria using logical AND.
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public QuestCriteria AndIfAll(QuestCriteria grouped)
        {
            ArgumentNullException.ThrowIfNull(grouped);
            EnsureStarted();
            var nextExpression = grouped.WipCriteria;
            if (nextExpression is null)
                throw new InvalidOperationException("Grouped criteria must start with If/IfNot.");
            CombineCurrentPredicateWith(nextExpression, ClauseTypes.And);
            return this;
        }

        /// <summary>
        /// Combines the current predicate with the provided one using logical OR.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public QuestCriteria OrIf(Func<GameStateManager, bool> predicate)
        {
            EnsureStarted();
            CombineCurrentPredicateWith(predicate, ClauseTypes.Or);
            return this;
        }

        /// <summary>
        /// Combines the current predicate with a grouped QuestCriteria completion criteria using logical OR.
        /// </summary>
        /// <param name="grouped"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public QuestCriteria OrIfAll(QuestCriteria grouped)
        {
            ArgumentNullException.ThrowIfNull(grouped);
            EnsureStarted();
            var nextExpression = grouped.WipCriteria;
            if (nextExpression is null)
                throw new InvalidOperationException("Grouped criteria must start with If/IfNot.");
            CombineCurrentPredicateWith(nextExpression, ClauseTypes.Or);
            return this;
        }

        /// <summary>
        /// Sets this QuestCriteria as a completion criteria.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        /// <exception cref="ArgumentException"></exception>
        public QuestCriteria ThenProgressThisQuest()
        {
            EnsureStarted();
            IsCompletionCriteria = true;
            CriteriaFunction = WipCriteria ?? throw new InvalidOperationException("No criteria function defined. Call If(...) or IfNot(...) then a completion/failure indicator to define criteria.");
            return this;
        }

        public QuestCriteria ThenFailThisQuest()
        {
            EnsureStarted();
            IsCompletionCriteria = false;
            CriteriaFunction = WipCriteria ?? throw new InvalidOperationException("No criteria function defined. Call If(...) or IfNot(...) then a completion/failure indicator to define criteria.");
            return this;
        }

        public bool Evaluate(GameStateManager gameStateManager)
        {
            if (CriteriaFunction is null)
                throw new InvalidOperationException("No criteria function defined. Call If(...) or IfNot(...) then a completion/failure indicator to define criteria.");
            return CriteriaFunction.Invoke(gameStateManager);
        }

        private QuestCriteria StartCriteria(Func<GameStateManager, bool> predicate, ClauseTypes clauseType)
        {
            ArgumentNullException.ThrowIfNull(predicate);
            var expr = clauseType == ClauseTypes.If
                ? predicate
                : (g) => !predicate(g);


            if (!_started)
            {
                WipCriteria = expr;
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
        private void CombineCurrentPredicateWith(Func<GameStateManager, bool> next, ClauseTypes clauseType)
        {
            var current = WipCriteria;
            if (current is null)
                throw new InvalidOperationException("No existing predicate to combine. Call If(...) or IfNot(...) first.");

            WipCriteria = clauseType switch
            {
                ClauseTypes.And => (g) => current(g) && next(g),
                ClauseTypes.Or => (g) => current(g) || next(g),
                _ => throw new InvalidOperationException("Unsupported operator state.")
            };
        }

        /// <summary>
        /// Ensures that the QuestCriteria completion criteria has been started with If or IfNot.
        /// </summary>
        /// <exception cref="InvalidOperationException">Thrown when the QuestCriteria completion criteria has not been started with If or IfNot.</exception>
        private void EnsureStarted()
        {
            if (!_started)
                throw new InvalidOperationException("Call If(...) or IfNot(...) first.");
        }

        #endregion
    }
}
