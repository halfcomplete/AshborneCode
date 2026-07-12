using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.CognitiveSystem.AttitudeSystem
{
    public record AttitudeRoleEmotionRule
    {
        public MemoryRole Role { get; init; }

        public EmotionReaction Reaction { get; init; }

        public AttitudeRoleEmotionRule(MemoryRole role, EmotionType emotionType, double mult, double add)
        {
            Role = role;
            Reaction = new(emotionType, mult, add, role);
        }
    }
}