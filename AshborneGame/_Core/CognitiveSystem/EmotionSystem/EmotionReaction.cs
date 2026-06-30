using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.CognitiveSystem.EmotionSystem
{
    /// <summary>
    /// Represents the effect on a single emotion that a certain personality trait or attitude may have.
    /// </summary>
    /// <param name="emotion">The emotion that the certain personality trait affects.</param>
    /// <param name="mult">The multiplier applied to the emotion modifier if the emotion already exists.</param>
    /// <param name="add">The base emotion modifier added if the emotion doesn't already exist.</param>
    public record EmotionReaction(EmotionType emotion, double mult, double add);
}