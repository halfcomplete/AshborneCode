using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core.MemorySystem;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// A class representing the entire psychological state of a character, encompassing relationships, feelings and emotions.
    /// </summary>
    public sealed class PsychologicalState
    {
        private readonly Guid _ownerID;

        /// <summary>
        /// Represents the mapping of character or entity identifiers to their corresponding attitudes in the
        /// relationship system.
        /// </summary>
        /// <remarks>Each key in the dictionary is a unique identifier for a character or entity, and the
        /// associated value indicates the current attitude toward that entity. This collection can be used to track and
        /// update relationship states dynamically during gameplay.</remarks>
        public Dictionary<string, Attitude> Relationships = new Dictionary<string, Attitude>();

        public MemoryProfile MemoryProfile { get; init; }

        public PersonalityProfile Personality { get; init; } = new();
        
        public PsychologicalState(Guid ownerID)
        {
            _ownerID = ownerID;
            MemoryProfile = new(_ownerID, Personality);
        }

        /// <summary>
        /// Adds a relationship to this psychological state. If a relationship with the same entityId already exists, it will be updated with the new attitude. This method allows for dynamic management of relationships, enabling characters to form new connections or modify existing ones based on in-game interactions and events.
        /// </summary>
        /// <param name="entityId">The unique identifier of the character or entity.</param>
        /// <param name="attitude">The attitude to associate with the specified entity.</param>
        public void AddRelationship(string entityId, Attitude attitude)
        {
            Relationships[entityId] = attitude;
        }

        /// <summary>
        /// Removes the relationship associated with the specified entity identifier.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity whose relationship is to be removed. Cannot be null.</param>
        public void RemoveRelationship(string entityId)
        {
            Relationships.Remove(entityId);
        }

        /// <summary>
        /// Attempts to retrieve the attitude associated with the specified entity identifier.
        /// </summary>
        /// <param name="entityId">The unique identifier of the entity whose relationship attitude is to be retrieved.</param>
        /// <param name="attitude">When this method returns, contains the attitude associated with the specified entity identifier, if found;
        /// otherwise, <see langword="null"/>. This parameter is passed uninitialized.</param>
        /// <returns>true if the relationship attitude for the specified entity identifier was found; otherwise, false.</returns>
        public bool TryGetRelationship(string entityId, out Attitude? attitude)
        {
            return Relationships.TryGetValue(entityId, out attitude);
        }
    }
}
