using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.MemorySystem;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// Represents the effect on a single emotion targeted towards a single MemoryRole that a certain personality trait or attitude may have.
    /// </summary>
    /// <remarks>
    /// If an NPC has this attitude/personality and have a memory with this MemoryTag, then they will feel [Mult - more/less] [Emotion] towards all [Role]s.
    /// </remarks>
    /// <param name="Emotion">The emotion that the certain personality trait affects.</param>
    /// <param name="Mult">The multiplier applied to the emotion modifier if the emotion already exists.</param>
    /// <param name="Add">The base emotion modifier added if the emotion doesn't already exist.</param>
    /// <param name="Role">The role this EmotionReaction is targeted towards, e.g. the Target or Actor.</param>
    public record EmotionReaction(EmotionType Emotion, double Mult, double Add, MemoryRole Role);
}