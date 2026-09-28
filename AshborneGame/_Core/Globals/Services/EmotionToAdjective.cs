using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Globals.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;

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
                _ => throw new InvalidEnumArgumentException($"Unhandled emotion type when calling GetEmotionDescriptor: {emotionType}")
            };
        }
    }
}