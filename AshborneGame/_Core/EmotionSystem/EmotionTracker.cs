using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.EmotionSystem
{
    public class EmotionTracker
    {
        public EmotionTypes Emotion { get; init; }
        public int Intensity { get; private set; }
        
        public EmotionTracker(EmotionTypes emotion, int intensity)
        {
            Emotion = emotion;
            Intensity = intensity;
        }

        public void SetIntensity(int intensity)
        {
            if (intensity > 100)
            {
                intensity = 100;
            }
            else if (intensity < 0)
            {
                intensity = 0;
            }
            Intensity = intensity;
        }
    }
}