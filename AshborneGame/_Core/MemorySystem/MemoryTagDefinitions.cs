using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.MemorySystem
{
    public static class MemoryTagDefinitions
    {
        public static Dictionary<MemoryTag, Dictionary<EmotionType, double>> Definitions = new()
        {
            { MemoryTag.Betrayal, new() 
            {
                {EmotionType.Surprise, 0.4},
                {EmotionType.Anger, 0.8},
            }},
            { MemoryTag.Generosity, new() 
            {
                {EmotionType.Happiness, 0.6},
                {EmotionType.Surprise, 0.2},
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