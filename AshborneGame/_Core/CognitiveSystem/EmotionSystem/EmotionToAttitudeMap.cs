using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// A static class mapping the 7 different types of emotions to how an NPC's attitude toward someone would change if they
    /// felt such an emotion because of them. These values assume that the NPC's personality is maximally amplifying the value.
    /// In other words, they are the absolute maximum attitude change an NPC can have from a single memory.
    /// </summary>
    public static class EmotionToAttitudeMap
    {
        /// <summary>
        /// If the NPC feels [EmotionType] towards [Entity], then that NPC's attitude towards [Entity] should change by [Values * Intensity].
        /// </summary>
        public static readonly Dictionary<EmotionType, List<AttitudeReaction>> Reactions = new()
        {
            [EmotionType.Anger] = 
            {
                new(AttitudeFactor.Affection, -0.3),
                new(AttitudeFactor.Trust, -0.4),
                new(AttitudeFactor.Respect, -0.3),
            },
            [EmotionType.Contempt] = 
            {
                new(AttitudeFactor.Affection, -0.3),
                new(AttitudeFactor.Trust, -0.3),
                new(AttitudeFactor.Respect, -0.6),
                new(AttitudeFactor.Fear, -0.1),
            },
            [EmotionType.Disgust] = 
            {
                new(AttitudeFactor.Affection, -0.4),
                new(AttitudeFactor.Trust, -0.2),
                new(AttitudeFactor.Respect, -0.5),
                new(AttitudeFactor.Fear, -0.1),
            },
            [EmotionType.Fear] = 
            {
                new(AttitudeFactor.Affection, -0.2),
                new(AttitudeFactor.Trust, -0.2),
                new(AttitudeFactor.Fear, +0.7),
                new(AttitudeFactor.Dominance, -0.4)
            },
            [EmotionType.Happiness] = 
            {
                new(AttitudeFactor.Affection, +0.6),
                new(AttitudeFactor.Trust, +0.3),
                new(AttitudeFactor.Fear, -0.2),
                new(AttitudeFactor.Dominance, -0.1),
                new(AttitudeFactor.Respect, +0.3),
            },
            [EmotionType.Sadness] = 
            {
                new(AttitudeFactor.Affection, +0.2),
                new(AttitudeFactor.Trust, +0.15),
                new(AttitudeFactor.Fear, -0.2),
            },
            [EmotionType.Surprise] = 
            {
                // idk what to put here; what attitude changes does feeling 'surprised' at someone make???
            },
        };
    }
}