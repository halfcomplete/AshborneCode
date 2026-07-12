using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// Represents a prototype EmotionModifier that hasn't yet been expanded to target a specific entity; rather, it only targets memory ROLES in a general manner.
    /// </summary>
    public record EmotionPotential(EmotionType Emotion, MemoryRole Role, double Value);
}