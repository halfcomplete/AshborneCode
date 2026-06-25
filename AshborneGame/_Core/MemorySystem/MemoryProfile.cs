using AshborneGame._Core.EmotionSystem;
using AshborneGame._Core.Game.Events;
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
        private Guid _ownerID;
        private List<Memory> _memories;

        public MemoryProfile(Guid ownerID, List<Memory> memories)
        {
            _ownerID = ownerID;
            _memories = memories;

            EventBus.Subscribe<GameEvents.System.TickEvent>(e => TickMemoryDecay(e.HoursPassed));
            EventBus.Subscribe<IMemorableGameEvent>(e => ReceiveMemorableEvent(e));
        }

        public MemoryProfile(Guid ownerID) : this(ownerID, new List<Memory>()) { }

        #region Receiving Memories

        /// <summary>
        /// Passes in a MemorableGameEvent that this MemoryProfile processes to either discard (can't be perceived) or stored.
        /// </summary>
        /// <remarks>
        /// The callback when an IMemorableGameEvent is published by the EventBus.
        /// </remarks>
        /// <param name="e">The IMemorableGameEvent to pass in.</param>
        public void ReceiveMemorableEvent(IMemorableGameEvent e)
        {
            if (!e.Witnesses.Contains(_ownerID))
            {
                return;
            }

            MemoryDefinition def = e.MemoryDefinition;

            List<EmotionModifier> modifiers = ParseMemoryDefinition(def, e.CurrentTotalHours);
            
            Memory newMemory = new(_ownerID, def.BaseIntensity, e, modifiers, def.Tags, e.CurrentTotalHours, e.CurrentTotalHours);

            
        }

        private static List<EmotionModifier> ParseMemoryDefinition(MemoryDefinition def, int currentTotalHours)
        {
            List<EmotionModifier> mods = new();

            foreach (var tag in def.Tags)
            {
                var tagDefinitions = MemoryTagDefinitions.Definitions[tag];

                foreach (var kvp in tagDefinitions)
                {
                    mods.Add(new EmotionModifier(kvp.Key, kvp.Value, currentTotalHours));
                }
            }

            return mods;
        }

        #endregion Receiving Memories

        #region Adding and Reinforcing Memories

        /// <summary>
        /// Adds a new memory to this MemoryProfile and reinforces existing similar memories.
        /// </summary>
        /// <param name="memory">The memory to be added.</param>
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

                double similarity = CalculateSimilarity(memory, existingMemory);

                (memory, _memories[i]) = ReinforceMemories(memory, existingMemory, CalculateStrengthReinforcement(similarity));
            }
            _memories.Add(memory);
        }

        /// <summary>
        /// Calculates the amount that two memories with a certain similarity should increase their strengths by.
        /// </summary>
        private static double CalculateStrengthReinforcement(double similarity)
        {
            double strengthReinforcement = 0;

            if (similarity > 0.3f)
            {
                strengthReinforcement += (double)Math.Pow(similarity - 0.3f, 2);
            }

            return strengthReinforcement;
        }

        /// <summary>
        /// Reinforces the strength of two memories given the amount of reinforce each by.
        /// </summary>
        /// <returns>A tuple (Memory, Memory) where the first item is the new memory and the second item is the existing memory.</returns>
        private static (Memory, Memory) ReinforceMemories(Memory memory, Memory existingMemory, double strengthReinforcement)
        {
            memory.Strength += strengthReinforcement;
            existingMemory.Strength += strengthReinforcement;
            return (memory, existingMemory);
        }

        #endregion Adding and Reinforcing Memories

        #region Similarity Calculations

        /// <summary>
        /// Calculates the similarity between two memories.
        /// </summary>
        /// <remarks>
        /// Cause similarity has a weighting of 0.4
        /// <br>Tag similarity has a weighting of 0.3</br>
        /// <br>Emotional similarity has a weighting of 0.1</br>
        /// </remarks>
        /// <returns>A double between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static double CalculateSimilarity(Memory memory1, Memory memory2)
        {
            double cSim = CalculateCauseSimilarity(memory1, memory2);
            double tSim = CalculateTagSimilarity(memory1, memory2);
            double eSim = CalculateEmotionalSimilarity(memory1, memory2);

            double similarity = (cSim * 0.5f) + (tSim * 0.4f) + (eSim * 0.1f);

            return similarity;
        }

        /// <summary>
        /// Calculates the similarity between two memories' causes.
        /// </summary>
        /// <returns>A double between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static double CalculateCauseSimilarity(Memory memory1, Memory memory2)
        {
            double similarity = 0;

            var cause1 = memory1.Cause;
            var cause2 = memory2.Cause;

            if (cause1 == cause2)
            {
                similarity = 1;
            }

            return similarity;
        }

        /// <summary>
        /// Calculates the similarity between two memories' tags.
        /// </summary>
        /// <returns>A double between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static double CalculateTagSimilarity(Memory memory1, Memory memory2)
        {
            var tags1 = memory1.Tags;
            var tags2 = memory2.Tags;

            var intersection = tags1.Intersect(tags2);
            int commonCount = intersection.Count();

            var union = tags1.Union(tags2);
            int unionCount = union.Count();

            double similarity = (double)commonCount / (double)unionCount;

            return similarity;
        }

        /// <summary>
        /// Calculates the similarity between two memory's EmotionModifiers.
        /// </summary>
        /// <returns>A double between 0 and 1, where 0 means maximally dissimilar and 1 means maximally similar (identical).</returns>
        private static double CalculateEmotionalSimilarity(Memory memory1, Memory memory2)
        {
            double similarity = 0;

            var emotions1 = new List<EmotionModifier>(memory1.EmotionModifiers);
            var emotions2 = new List<EmotionModifier>(memory2.EmotionModifiers);

            HashSet<EmotionType> uniqueEmotions = new();

            foreach (var mod1 in emotions1)
            {
                uniqueEmotions.Add(mod1.Type);
                EmotionModifier? mod2 = emotions2.FirstOrDefault(m => m.Type == mod1.Type);

                // If there is an emotion modifier shared by both memory1 and memory2
                if (mod2 != null)
                {
                    similarity += 1 - Math.Abs(mod1.InitialAmount - mod2.InitialAmount);
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

            similarity /= (double)uniqueEmotions.Count();

            return similarity;
        }

        #endregion Similarity Calculations

        #region API

        public List<Memory> GetMemories() => _memories;

        public List<Memory> GetActiveMemories() => _memories.Where(m => m.IsActive).ToList();

        public List<Memory> GetMemoriesByCause(IMemorableGameEvent cause) => _memories.Where(m => m.Cause == cause).ToList();

        public List<Memory> GetMemoriesByTag(MemoryTag tag) => _memories.Where(m => m.Tags.Contains(tag)).ToList();

        public List<Memory> GetMemoriesOrderedByStrength() => _memories.OrderByDescending(m => m.Strength).ToList();

        public bool RemembersEvent(IMemorableGameEvent cause) => _memories.Any(m => m.Cause == cause);

        #endregion API

        #region Memory Decay

        /// <summary>
        /// Ticks this MemoryProfile's memories so that their strength decays.
        /// </summary>
        /// <param name="hoursPassed">The number of hours passed since the last tick.</param>
        public void TickMemoryDecay(int hoursPassed)
        {
            // Decay the strength of every memory in this memory profile
            foreach (Memory mem in _memories)
            {
                double strengthDecay = CalculateStrengthDecay(mem.Intensity, hoursPassed);

                mem.Strength -= strengthDecay;

                if (mem.Strength < 0.00001)
                {
                    _memories.Remove(mem);
                }
            }
        }

        /// <summary>
        /// Calculates how much a memory's Strength should reduce by, given the memory's Intensity and the number of hours passed.
        /// </summary>
        /// <returns>A double representing how much the Strength should decrease by</returns>
        public static double CalculateStrengthDecay(double intensity, int hoursPassed)
        {
            double decayPerHour = (0.4 - 0.399) * Math.Pow(intensity, 0.5);
            return hoursPassed * decayPerHour;
        }

        #endregion Memory Decay

        #region Emotion Modifier Calculations

        public static List<EmotionModifier> CalculateInitialEmotionalModifiers(Memory memory, int currentHour)
        {
            List<MemoryTag> tags = new(memory.Tags);

            List<EmotionModifier> emotionModifiers = new();

            foreach (var tag in tags)
            {
                foreach (var mod in MemoryTagDefinitions.Definitions[tag])
                {
                    emotionModifiers.Add(new EmotionModifier(mod.Key, mod.Value, currentHour));
                }
            }

            return emotionModifiers;
        }

        #endregion Emotion Modifier Calculations
    }
}
