using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A class representing the entire psychological state of a character, encompassing relationships, feelings and emotions.
    /// </summary>
    public sealed class PsychologicalState
    {
        /// <summary>
        /// Represents the mapping of character or entity identifiers to their corresponding attitudes in the
        /// relationship system.
        /// </summary>
        /// <remarks>Each key in the dictionary is a unique identifier for a character or entity, and the
        /// associated value indicates the current attitude toward that entity. This collection can be used to track and
        /// update relationship states dynamically during gameplay.</remarks>
        public Dictionary<string, Attitude> Relationships = new Dictionary<string, Attitude>();

        public EmotionProfile EmotionalState { get; private set; } = new EmotionProfile();
        
        public PsychologicalState() { }

        public void AddRelationship(string entityId, Attitude attitude)
        {
            Relationships[entityId] = attitude;
        }

        public void RemoveRelationship(string entityId)
        {
            Relationships.Remove(entityId);
        }

        public bool TryGetRelationship(string entityId, out Attitude? attitude)
        {
            return Relationships.TryGetValue(entityId, out attitude);
        }
    }
}
