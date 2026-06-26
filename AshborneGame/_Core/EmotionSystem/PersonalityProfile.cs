using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.MemorySystem;

namespace AshborneGame._Core.EmotionSystem
{
    public struct PersonalityProfile
    {
        public double Curiosity { get; private set; }
        public double Compassion { get; private set; }
        public double Aggression { get; private set; }
    }

    public static class PersonalityTraitDefinitions
    {
        public static Dictionary<EmotionType, double> CuriosityDef = new Dictionary<EmotionType, double>
        {
            { EmotionType.Surprise, 1.4 },
            { EmotionType.Disgust, 0.8 },
            { EmotionType.Fear, 0.9 },
            { EmotionType.Contempt, 0.9 },
        };

        public static Dictionary<EmotionType, double> CompassionDef = new Dictionary<EmotionType, double>
        {
            { EmotionType.Happiness, 1.4 },
            { EmotionType.Sadness, 0.8 },
            { EmotionType.Anger, 0.7 },
            { EmotionType.Contempt, 0.7 },
            { EmotionType.Disgust, 0.7 },
        };

        public static Dictionary<EmotionType, double> AggressionDef = new Dictionary<EmotionType, double>
        {
            { EmotionType.Anger, 1.7 },
            { EmotionType.Contempt, 1.5 },
            { EmotionType.Disgust, 1.5 },
            { EmotionType.Fear, 0.8 },
            { EmotionType.Happiness, 0.8 },
            { EmotionType.Sadness, 0.85 },
        };
    }
}