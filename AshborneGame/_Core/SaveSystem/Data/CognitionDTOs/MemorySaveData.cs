using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.MemorySystem;

namespace AshborneGame._Core.SaveSystem.Data.CognitionDTOs
{
    public class MemorySaveData
    {
        public double Intensity { get; set; }
        public double Strength { get; set; }
        public MemorySourceSaveData Cause { get; set; } = null!;
        public List<EmotionModifier> EmotionModifiers { get; set; } = new();
        public List<MemoryTag> MemoryTags { get; set; } = new();
        public int HourCreatedAt { get; set; }
        public int HourLastReinforcedAt { get; set; }
    }
}