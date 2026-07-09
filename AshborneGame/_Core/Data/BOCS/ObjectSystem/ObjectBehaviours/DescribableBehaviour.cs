using System;
using System.Collections.Generic;
using System.Linq;
using AshborneGame._Core._Player;
using AshborneGame._Core.Game;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    /// <summary>
    /// Used by LocationNarrativeProfile, GameObjects, NPCs, etc. for conditional descriptions.
    /// </summary>
    public class DescribableBehaviour : Behaviour, IDescribable
    {
        /// <summary>
        /// List of (condition, description) pairs.
        /// </summary>
        public List<(Func<GameStateManager, bool> condition, string description)> Conditions { get; } = new();

        public DescribableBehaviour(List<(Func<GameStateManager, bool> condition, string description)> conditions)
        {
            Conditions = conditions;
        }
        public DescribableBehaviour()
        {

        }

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

        public override DescribableBehaviour DeepClone()
        {
            return new DescribableBehaviour(new(Conditions));
        }


        // TODO: make this serialisable by using data-driven conditions and descriptions, rather than code-driven
        //       This is also required to serialise Quests, which are currently not serialisable due to their use of code-driven conditions and descriptions
        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            throw new NotImplementedException("DescribableBehaviour does not support saving yet.");
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            throw new NotImplementedException("DescribableBehaviour does not support loading yet.");
        }
    }
}
