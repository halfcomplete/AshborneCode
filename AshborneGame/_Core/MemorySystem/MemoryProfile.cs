using System;
using System.Collections.Generic;
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

        public void AddMemory(Memory memory)
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

                float similarity = CalculateSimilarity(memory, existingMemory);

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
        private static float CalculateSimilarity(Memory memory1, Memory memory2)
        {
            float cSim = CalculateCauseSimilarity(memory1, memory2);
            float tSim = CalculateTagSimilarity(memory1, memory2);
            float eSim = CalculateEmotionalSimilarity(memory1, memory2);

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
        /// <returns>A float between -1 and 1, where -1 means drastically different and 1 means the two are the exact same.</returns>
        private static float CalculateEmotionalSimilarity(Memory memory1, Memory memory2)
        {
            float similarity = 0;

            var emotions1 = memory1.EmotionModifiers;
            var emotions2 = memory2.EmotionModifiers;

            

            return similarity;
        }
    }
}
