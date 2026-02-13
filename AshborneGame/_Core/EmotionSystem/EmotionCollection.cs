using System.Diagnostics;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.EmotionSystem
{
    public class EmotionCollection
    {
        private Dictionary<EmotionTypes, EmotionTracker> _emotionTrackers;

        public EmotionCollection()
        {
            _emotionTrackers = CreateDictionaryOfEmotions();
        }

        private Dictionary<EmotionTypes, EmotionTracker> CreateDictionaryOfEmotions()
        {
            var collection = new Dictionary<EmotionTypes, EmotionTracker>();
            foreach (EmotionTypes emotion in Enum.GetValues(typeof(EmotionTypes)))
            {
                collection.Add(emotion, new EmotionTracker(emotion, 0));
            }
            return collection;
        }

        /// <summary>
        /// Gets the intensity of a specific emotion.
        /// </summary>
        /// <param name="emotion">The emotion to get the intensity for.</param>
        /// <returns>The intensity of the specified emotion.</returns>
        /// <exception cref="UnreachableException">Thrown if the specified emotion is not found in the collection.</exception>
        public int GetEmotionIntensity(EmotionTypes emotion)
        {
            if (_emotionTrackers.TryGetValue(emotion, out EmotionTracker tracker))
            {
                return tracker.Intensity;
            }
            throw new UnreachableException($"Somehow, emotion {emotion} not found in collection when getting the emotion intensity. We should never get here since we initialise the collection with all emotions.");
        }

        /// <summary>
        /// Sets the intensity of a specific emotion. Intensity is clamped between 0 and 100.
        /// </summary>
        /// <param name="emotion">The emotion to set the intensity for.</param>
        /// <param name="intensity">The intensity value to set, clamped between 0 and 100.</param>
        /// <exception cref="UnreachableException">Thrown if the specified emotion is not found in the collection.</exception>
        public void SetEmotionIntensity(EmotionTypes emotion, int intensity)
        {
            if (_emotionTrackers.TryGetValue(emotion, out EmotionTracker tracker))
            {
                tracker.SetIntensity(intensity);
                return;
            }
            throw new UnreachableException($"Somehow, emotion {emotion} not found in collection when setting the emotion intensity. We should never get here since we initialise the collection with all emotions.");
        }

        /// <summary>
        /// Adds a specified intensity to a specific emotion. The resulting intensity is clamped between 0 and 100.
        /// </summary>
        /// <param name="emotion">The emotion to add intensity to.</param>
        /// <param name="intensityToAdd">The intensity value to add, which can be negative.</param>
        /// <exception cref="UnreachableException">Thrown if the specified emotion is not found in the collection.</exception>
        public void ChangeEmotionIntensity(EmotionTypes emotion, int intensityToAdd)
        {
            if (_emotionTrackers.TryGetValue(emotion, out EmotionTracker tracker))
            {
                int newIntensity = tracker.Intensity + intensityToAdd;
                tracker.SetIntensity(newIntensity);
                return;
            }
            throw new UnreachableException($"Somehow, emotion {emotion} not found in collection when changing the emotion intensity. We should never get here since we initialise the collection with all emotions.");
        }
    }
}