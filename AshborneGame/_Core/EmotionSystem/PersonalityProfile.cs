using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.MemorySystem;

namespace AshborneGame._Core.EmotionSystem
{
    /// <summary>
    /// Represents who this NPC is. Generally stable over time. Affects how a particular NPC reacts to certain MemoryTags and a Memory's final intensity.
    /// </summary>
    public class PersonalityProfile
    {
        public double Curiosity { get; private set; }
        public double Compassion { get; private set; }
        public double Aggression { get; private set; }
    }
}