using AshborneGame._Core.EmotionSystem;
using AshborneGame._Core.Globals.Enums;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.MemorySystem
{
    public class MemoryProfile
    {
        private List<Memory> _memories;

        public MemoryProfile(List<Memory> memories)
        {
            _memories = memories;
        }

        public MemoryProfile()
        {
            _memories = new();
        }

        /// <summary>
        /// Adds a new memory to this MemoryProfile and reinforces existing similar memories.
        /// </summary>
        /// <param name="memory">The memory to be added.</param>
        /// <param name="totalInGameHours">The current total in-game hours. Used to determine memory similarity.</param>
        public void AddMemory(Memory memory, int totalInGameHours)
        {
            // Loop over each existing memory and check how similar it is to the newly added memory
            // Then reinforce that memory based on that similarity
            // For example, if the NPC remembers how the player stole from them before and the
            // player steals from them again,then the strength of the first theft should increase

            // TODO: Make contradicting memories (player stole from the NPC, player saved the NPC's life) reduce each other's strengths by a little bit.
            //       This will require a method of quantifying how much two memories contradict each other
            for (int i = 0; i < _memories.Count; i++)
            {
                Memory existingMemory = _memories[i];

                float similarity = CalculateSimilarity(memory, existingMemory, totalInGameHours);

                (memory, _memories[i]) = ReinforceMemories(memory, existingMemory, CalculateStrengthReinforcement(similarity));
            }
            _memories.Add(memory);
        }

        /// <summary>
        /// Calculates the amount that two memories with a certain similarity should increase their strengths by.
        /// </summary>
        private float CalculateStrengthReinforcement(float similarity)
        {
            float strengthReinforcement = 0;

            if (similarity > 0.3f)
            {
                strengthReinforcement += (float)Math.Pow(similarity - 0.3f, 2);
            }

            return strengthReinforcement;
        }

        private (Memory, Memory) ReinforceMemories(Memory memory, Memory existingMemory, float strengthReinforcement)
        {
            memory.Strength += strengthReinforcement;
            existingMemory.Strength += strengthReinforcement;
            return (memory, existingMemory);
        }

        /// <summary>
        /// Calculates the similarity between two memories.
        /// </summary>
        /// <remarks>
        /// Cause similarity has a weighting of 0.4
        /// <br>Tag similarity has a weighting of 0.3</br>
        /// <br>Emotional similarity has a weighting of 0.1</br>
        /// </remarks>
        /// <returns>A float between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static float CalculateSimilarity(Memory memory1, Memory memory2, int totalInGameHours)
        {
            float cSim = CalculateCauseSimilarity(memory1, memory2);
            float tSim = CalculateTagSimilarity(memory1, memory2);
            float eSim = CalculateEmotionalSimilarity(memory1, memory2, totalInGameHours);

            float similarity = (cSim * 0.5f) + (tSim * 0.4f) + (eSim * 0.1f);

            return similarity;
        }

        /// <summary>
        /// Calculates the similarity between two memories' causes.
        /// </summary>
        /// <returns>A float between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static float CalculateCauseSimilarity(Memory memory1, Memory memory2)
        {
            float similarity = 0;

            var cause1 = memory1.Cause;
            var cause2 = memory2.Cause;

            if (cause1 == cause2)
            {
                similarity = 1;
            }
            else if (cause1.Category == cause2.Category)
            {
                similarity = 0.65f;
            }

            return similarity;
        }

        /// <summary>
        /// Calculates the similarity between two memories' tags.
        /// </summary>
        /// <returns>A float between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static float CalculateTagSimilarity(Memory memory1, Memory memory2)
        {
            float similarity = 0;

            var tags1 = memory1.Tags;
            var tags2 = memory2.Tags;

            var intersection = tags1.Intersect(tags2);
            int commonCount = intersection.Count();

            var union = tags1.Union(tags2);
            int unionCount = union.Count();

            similarity = (float)commonCount / (float)unionCount;

            return similarity;
        }

        /// <summary>
        /// Calculates the similarity between two memory's EmotionModifiers.
        /// </summary>
        /// <returns>A float between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static float CalculateEmotionalSimilarity(Memory memory1, Memory memory2, int totalInGameHours)
        {
            float similarity = 0;

            var emotions1 = new List<EmotionModifier>(memory1.EmotionModifiers);
            var emotions2 = new List<EmotionModifier>(memory2.EmotionModifiers);

            HashSet<EmotionTypes> uniqueEmotions = new();

            foreach (var mod1 in emotions1)
            {
                uniqueEmotions.Add(mod1.Type);
                EmotionModifier? mod2 = emotions2.FirstOrDefault(m => m.Type == mod1.Type);

                // If there is an emotion modifier shared by both memory1 and memory2
                if (mod2 != null)
                {
                    similarity += 1 - Math.Abs(mod1.GetCurrentAmount(totalInGameHours) - mod2.GetCurrentAmount(totalInGameHours));
                }
            }

            foreach (var mod2 in emotions2)
            {
                uniqueEmotions.Add(mod2.Type);
                EmotionModifier? mod1 = emotions2.FirstOrDefault(m => m.Type == mod2.Type);

                // If there is an emotion modifier shared by both memory1 and memory2
                // This SHOULD always be false after the first round of checks but just in case
                if (mod1 != null)
                {
                    throw new UnreachableException($"MemoryProfile: discovered emotion modifier affecting {mod1.Type} in memory2.");
                }
            }

            similarity /= (float)uniqueEmotions.Count();

            return similarity;
        }
    }
}
