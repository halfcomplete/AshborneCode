using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Globals.Services
{
    public class EmotionToAdjective
    {
        /// <summary>
        /// Generates an adjective for an emotion type.
        /// </summary>
        public static string GetEmotionDescriptor(EmotionTypes emotionType)
        {
            return emotionType switch
            {
                EmotionTypes.Happiness => "happy",
                EmotionTypes.Sadness => "sad",
                EmotionTypes.Anger => "angry",
                EmotionTypes.Fear => "scared",
                EmotionTypes.Disgust => "disgusted",
                EmotionTypes.Surprise => "surprised",
                _ => throw new ArgumentOutOfRangeException(nameof(emotionType), $"Unhandled emotion type when calling GetEmotionDescriptor: {emotionType}")
            };
        }
    }
}