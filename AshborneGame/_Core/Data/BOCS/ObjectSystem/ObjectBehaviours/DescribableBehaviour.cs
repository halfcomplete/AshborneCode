using System;
using System.Collections.Generic;
using System.Linq;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    /// <summary>
    /// Used by LocationNarrativeProfile, GameObjects, NPCs, etc. for conditional descriptions.
    /// </summary>
    public class DescribableBehaviour : IDescribable
    {
        /// <summary>
        /// List of (condition, description) pairs.
        /// </summary>
        public List<(Func<GameStateManager, bool> condition, string description)> Conditions { get; } = new();

        /// <summary>
        /// Adds a new conditional description.
        /// </summary>
        public void AddCondition(Func<GameStateManager, bool> condition, string description)
        {
            Conditions.Add((condition, description));
        }

        /// <summary>
        /// Returns all applicable descriptions for the current state, joined by space.
        /// </summary>
        public string GetDescription(Player player, GameStateManager state)
        {
            var applicable = Conditions
                .Where(c => c.condition(state))
                .Select(c => c.description)
                .ToList();

            return string.Join(" ", applicable);
        }
    }
}
