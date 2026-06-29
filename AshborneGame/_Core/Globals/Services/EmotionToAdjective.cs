using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Services
{
    public class EmotionToAdjective
    {
        /// <summary>
        /// Generates an adjective for an emotion type.
        /// </summary>
        public static string GetEmotionDescriptor(EmotionType emotionType)
        {
            return emotionType switch
            {
                EmotionType.Happiness => "happy",
                EmotionType.Sadness => "sad",
                EmotionType.Anger => "angry",
                EmotionType.Fear => "scared",
                EmotionType.Disgust => "disgusted",
                EmotionType.Surprise => "surprised",
                _ => throw new ArgumentOutOfRangeException(nameof(emotionType), $"Unhandled emotion type when calling GetEmotionDescriptor: {emotionType}")
            };
        }
    }
}