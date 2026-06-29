using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    /// <summary>
    /// Represents the base Memory that an IMemorableGameEvent can create. This is used by the Memory & Emotion systems to develop a unique Memory for each NPC based on personality, attitudes and their existing emotions.
    /// </summary>
    public class MemoryDefinition
    {
        public double BaseIntensity { get; }

        /// <summary>
        /// Describes in broad terms what this Memory is about. Used by the Memory & Emotion systems to determine how an NPC should adapt this base MemoryDefinition to a unique Memory.
        /// </summary>
        public HashSet<MemoryTag> Tags { get; }
        
        public MemoryDefinition(double baseIntensity, HashSet<MemoryTag> tags)
        {
            BaseIntensity = baseIntensity;
            Tags = tags;
        }
    }
}