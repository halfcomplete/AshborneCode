using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.MemorySystem
{
    /// <summary>
    /// Defines the base, NPC-agnostic emotional modifiers that a certain MemoryTag creates. These represent the 'default' reaction.
    /// </summary>
    public static class MemoryTagDefinitions
    {
        public static Dictionary<MemoryTag, Dictionary<EmotionType, double>> Definitions = new()
        {
            { MemoryTag.Betrayal, new() 
            {
                {EmotionType.Surprise, 0.4},
                {EmotionType.Anger, 0.8},
                {EmotionType.Happiness, -0.3}
            }},
            { MemoryTag.Gift, new() 
            {
                {EmotionType.Happiness, 0.6},
                {EmotionType.Surprise, 0.2},
                {EmotionType.Anger, -0.1}
            }},
            { MemoryTag.Sacrifice, new() 
            {
                {EmotionType.Happiness, 0.7},
                {EmotionType.Surprise, 0.4},
                {EmotionType.Sadness, 0.3},
            }},
        };
    }
}