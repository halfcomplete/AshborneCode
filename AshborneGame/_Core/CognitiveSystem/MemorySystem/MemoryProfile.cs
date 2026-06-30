using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using System.Diagnostics;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public class MemoryProfile
    {
        private Guid _ownerID;
        private PersonalityProfile _personality;
        private Dictionary<Guid, Attitude> _relationships;
        private List<Memory> _memories;

        public MemoryProfile(Guid ownerID, PersonalityProfile personality, Dictionary<Guid, Attitude> relationships, List<Memory> memories)
        {
            _ownerID = ownerID;
            _personality = personality;
            _relationships = relationships;
            _memories = memories;

            EventBus.Subscribe<GameEvents.System.TickEvent>(e => TickMemoryDecay(e.HoursPassed));
            EventBus.Subscribe<IMemorableGameEvent>(e => ReceiveMemorableEvent(e));
            EventBus.Subscribe<GameEvents.Memory.StrengthenedEvent>(e => ReceiveMemoryStrengthenedEvent(e));
        }

        public MemoryProfile(Guid ownerID, PersonalityProfile personality, Dictionary<Guid, Attitude> relationships) : this(ownerID, personality, relationships, new List<Memory>()) { }

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
            if (!ShouldReceiveMemorableEvent(e))
            {
                return;
            }

            MemoryDefinition def = e.MemoryDefinition;

            Dictionary<EmotionModifier, EmotionAccumulator> initialModifiers = GetInitialEmotionModifiers(def.Tags, e.CurrentTotalHours);
            double initialIntensity = CalculateActualIntensity(def.BaseIntensity, e);

            Dictionary<EmotionModifier, EmotionAccumulator> newModifiers = ApplyPersonalityReactionsToEmotionModifiers(def, initialModifiers);
            newModifiers = ApplyAttitudeToEmotionalModifiers(e, newModifiers);

            List<EmotionModifier> accumulatedModifiers = ApplyAccumulatorsToEmotionModifiers(newModifiers);

            List<EmotionModifier> finalModifiers = CombineLikeEmotionModifiers(accumulatedModifiers);
            double newIntensity = initialIntensity;

            Memory newMemory = new(_ownerID, newIntensity, e, finalModifiers, def.Tags, e.CurrentTotalHours, e.CurrentTotalHours);

            AddMemory(newMemory);
            ApplyMemoryInfluenceToRelationships(newMemory);
        }

        public void StrengthenMemory(IMemorableGameEvent cause, double strengthDelta, int currentTotalHours)
        {
            EventBus.Publish(new GameEvents.Memory.StrengthenedEvent(currentTotalHours, _ownerID, cause, strengthDelta));
        }

        // TODO: Do we really need events for all this? Why not direct calling?
        private void ReceiveMemoryStrengthenedEvent(GameEvents.Memory.StrengthenedEvent e)
        {
            if (e.OwnerId != _ownerID)
            {
                return;
            }

            foreach (Memory memory in _memories.Where(m => m.Cause == e.Cause))
            {
                memory.Strength += e.StrengthDelta;
            }
        }

        /// <summary>
        /// Takes a list of EmotionModifiers and combines ones with the same emotion type.
        /// </summary>
        /// <returns>A list of EmotionModifiers where each one has a unique emotion type.</returns>
        private static List<EmotionModifier> CombineLikeEmotionModifiers(List<EmotionModifier> modifiers)
        {
            List<EmotionModifier> combinedModifiers = [];

            // sort by emotion type
            // then iterate through and combine like emotion types
            var sorted = modifiers.OrderBy(m => m.Type).ToList();

            EmotionType currentEmotion = sorted.Count > 0 ? sorted[0].Type : EmotionType.Anger;
            double currentAmount = 0;

            foreach (EmotionModifier mod in sorted)
            {
                // if this is a new emotion type that we've encountered
                if (mod.Type != currentEmotion)
                {
                    combinedModifiers.Add(new(currentEmotion, currentAmount));
                    currentAmount = 0;
                }

                currentEmotion = mod.Type;
                currentAmount += mod.InitialAmount;
            }

            combinedModifiers.Add(new(currentEmotion, currentAmount));

            return combinedModifiers;
        }

        /// <summary>
        /// Applies the changes that an EmotionModifier's associated EmotionAccumulator defines.
        /// </summary>
        /// <returns>A list of EmotionModifiers with their associated EmotionAccumulators applied.</returns>
        private static List<EmotionModifier> ApplyAccumulatorsToEmotionModifiers(Dictionary<EmotionModifier, EmotionAccumulator> modifiers)
        {
            List<EmotionModifier> newModifiers = [];

            foreach (var kvp in modifiers)
            {
                var newMod = ApplyAccumulatorToEmotionModifier(kvp.Key, kvp.Value);
                newModifiers.Add(newMod);
            }

            return newModifiers;
        }

        /// <summary>
        /// Applies the changes that a single EmotionModifier's associated EmotionAccumulator defines.
        /// </summary>
        /// <param name="mod">The EmotionModifier to apply the changes to.</param>
        /// <param name="accumulator">The EmotionAccumulator to get the changes from.</param>
        /// <returns>A single EmotionModifier with its EmotionAccumulator applied.</returns>
        private static EmotionModifier ApplyAccumulatorToEmotionModifier(EmotionModifier mod, EmotionAccumulator accumulator)
        {
            double newInitialAmount = (mod.InitialAmount + accumulator.TotalAdd);

            if (accumulator.TotalMult >= 0)
            {
                newInitialAmount *= accumulator.TotalMult;
            }
            else
            {
                newInitialAmount /= accumulator.TotalMult;
            }

            return new(mod.Type, newInitialAmount);
        }
        
        private Dictionary<EmotionModifier, EmotionAccumulator> ApplyPersonalityReactionsToEmotionModifiers(MemoryDefinition def, Dictionary<EmotionModifier, EmotionAccumulator> initialModifiers)
        {
            // Loop over each MemoryTag in the given MemoryDefinition
            foreach (MemoryTag tag in def.Tags)
            {
                // Get the reactions from each personality trait that this MemoryTag defines
                Dictionary<PersonalityTrait, List<PersonalityReaction>> PersonalityReactions = MemoryTagDefinitions.Definitions[tag].Definition.PersonalityModifiers;

                // Loop over each personality trait in PersonalityReactions
                // personalityTrait is an enumeration (either Curiosity, Compassion or Aggression)
                foreach (var (personalityTrait, personalityReactions) in PersonalityReactions)
                {
                    // Loop over every reaction in this MemoryTag's personalityTrait's personalityReactions
                    // Each 'reaction' contains the emotion that is affected and a mult and add value
                    foreach (PersonalityReaction reaction in personalityReactions)
                    {
                        // Track whether this reaction affects an emotion that is already in the given initialModifiers
                        bool seen = false;

                        // For every EmotionModifier and EmotionAccumulator in the given initialModifieres, 
                        // check if the modified emotion is the same as this reaction's emotion.
                        foreach (var (modifier, accumulator) in initialModifiers)
                        {
                            if (modifier.Type == reaction.emotion)
                            {
                                // If both this reaction and the current emotion modifier being checked modify the same emotion,
                                // then add to the associated EmotionAccumulator's TotalMult and mark seen as true.
                                accumulator.TotalMult = CalculateEmotionAccumulatorTotalMult(accumulator.TotalMult, _personality.PersonalityTraits[personalityTrait], reaction.mult);
                                seen = true;
                            }
                        }

                        // If there were no initial emotion modifiers that modify reaction.emotion then add a new EmotionModifier
                        if (!seen)
                        {
                            EmotionModifier modifier = new(reaction.emotion, reaction.add * _personality.PersonalityTraits[personalityTrait]);
                            initialModifiers.Add(modifier, new EmotionAccumulator());
                        }
                    }
                }
            }

            return initialModifiers;
        }
        
        /// <summary>
        /// Answers the question: "Given the current TotalMult of an EmotionAccumulator, the influence
        /// a certain PersonalityTrait has on this NPC, and the multiplier that the PersonalityReaction has, what should the new TotalMult be?"
        /// </summary>
        /// <remarks>
        /// Assumes that the given personalityTraitInfluence is of the correct PersonalityTrait.
        /// </remarks>
        private static double CalculateEmotionAccumulatorTotalMult(double currentTotalMult, double personalityTraitInfluence, double reactionMult)
        {
            return currentTotalMult * (1 + personalityTraitInfluence * Math.Abs(reactionMult-1));
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

        /// <summary>
        /// Recursively iterates through every EmotionModifier in every Memory and sums up their modifications
        /// to calculate the total intensity this NPC is experiencing of a particular emotion at this time.
        /// </summary>
        /// <returns>An unclamped double representing the intensity of the given emotion that this NPC is experiencing.</returns>
        public double GetTotalEmotionIntensity(EmotionType emotionType)
        {
            double total = 0;

            foreach (Memory memory in _memories)
            {
                List<EmotionModifier> emotionModifiers = memory.EmotionModifiers;

                var targetMods = emotionModifiers.Where(m => m.Type == emotionType).ToList();

                foreach (EmotionModifier modifier in targetMods)
                {
                    total += modifier.InitialAmount * memory.Influence;
                }
            }

            return total;
        }

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
            foreach (Memory mem in _memories.ToList())
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

        #region Initial Memory Calculations

        /// <summary>
        /// Takes a Hashset of MemoryTags and returns the base EmotionModifiers that the given MemoryTags define.
        /// </summary>
        /// <param name="tags">The MemoryTags to parse and interpret.</param>
        /// <param name="currentTotalHours">The current total hours of the game.</param>
        /// <returns></returns>
        private static Dictionary<EmotionModifier, EmotionAccumulator> GetInitialEmotionModifiers(HashSet<MemoryTag> tags, int currentTotalHours)
        {
            Dictionary<EmotionModifier, EmotionAccumulator> mods = new();

            foreach (MemoryTag tag in tags)
            {
                MemoryTagDefinition tagDefinition = MemoryTagDefinitions.Definitions[tag].Definition;

                foreach (var baseMod in tagDefinition.BaseEmotionalModifiers)
                {
                    mods[new EmotionModifier(baseMod.Key, baseMod.Value)] = new EmotionAccumulator();
                }
            }

            return mods;
        }
        
        private bool ShouldReceiveMemorableEvent(IMemorableGameEvent e)
        {
            return e.Participants.Any(participant => participant.EntityId == _ownerID);
        }

        private double CalculateActualIntensity(double baseIntensity, IMemorableGameEvent e)
        {
            double intensity = baseIntensity;

            double personalityBias = GetPersonalityIntensityBias(e);
            intensity += personalityBias;

            foreach (MemoryParticipant participant in e.Participants)
            {
                if (!_relationships.TryGetValue(participant.EntityId, out Attitude? attitude) || attitude is null)
                {
                    continue;
                }

                double relationshipWeight = GetParticipantIntensityWeight(participant.Roles);
                intensity += GetAttitudeIntensityImpact(attitude) * relationshipWeight;
            }

            return Math.Clamp(intensity, 0, 1);
        }

        private double GetPersonalityIntensityBias(IMemorableGameEvent e)
        {
            double bias = 0;

            foreach (MemoryTag tag in e.MemoryDefinition.Tags)
            {
                if (!MemoryTagDefinitions.Definitions.TryGetValue(tag, out IMemoryTag? tagDefinition) || tagDefinition is null)
                {
                    continue;
                }

                foreach (var (trait, reactions) in tagDefinition.Definition.PersonalityModifiers)
                {
                    double traitStrength = _personality.PersonalityTraits.TryGetValue(trait, out double value) ? value : 0;
                    double reactionBias = reactions.Sum(reaction => (reaction.mult - 1) + reaction.add);
                    bias += traitStrength * reactionBias * 0.1;
                }
            }

            return bias;
        }

        private Dictionary<EmotionModifier, EmotionAccumulator> ApplyAttitudeToEmotionalModifiers(IMemorableGameEvent e, Dictionary<EmotionModifier, EmotionAccumulator> mods)
        {
            foreach (MemoryParticipant participant in e.Participants)
            {
                if (!_relationships.TryGetValue(participant.EntityId, out Attitude? attitude) || attitude is null)
                {
                    continue;
                }

                double participantBias = GetAttitudeEmotionBias(attitude) * GetParticipantEmotionWeight(participant.Roles);

                foreach (var mod in mods)
                {
                    mod.Value.TotalAdd += participantBias;
                }
            }

            return mods;
        }

        private static double GetParticipantIntensityWeight(List<MemoryRole> roles)
        {
            double weight = 0;

            foreach (MemoryRole role in roles)
            {
                weight += role switch
                {
                    MemoryRole.Actor => 0.35,
                    MemoryRole.Target => 0.25,
                    MemoryRole.Victim => 0.45,
                    MemoryRole.Beneficiary => 0.2,
                    MemoryRole.Witness => 0.1,
                    _ => 0
                };
            }

            return Math.Clamp(weight, 0, 1);
        }

        private static double GetParticipantEmotionWeight(List<MemoryRole> roles)
        {
            double weight = 0;

            foreach (MemoryRole role in roles)
            {
                weight += role switch
                {
                    MemoryRole.Actor => 0.3,
                    MemoryRole.Target => 0.25,
                    MemoryRole.Victim => 0.4,
                    MemoryRole.Beneficiary => 0.2,
                    MemoryRole.Witness => 0.1,
                    _ => 0
                };
            }

            return Math.Clamp(weight, 0, 1);
        }

        private static double GetAttitudeIntensityImpact(Attitude attitude)
        {
            return ((-attitude.Affection) * 0.3) + ((-attitude.Trust) * 0.25) + ((-attitude.Respect) * 0.15) + (attitude.Fear * 0.2) + ((-attitude.Dominance) * 0.1);
        }

        private static double GetAttitudeEmotionBias(Attitude attitude)
        {
            return ((-attitude.Affection) * 0.15) + ((-attitude.Trust) * 0.2) + ((-attitude.Respect) * 0.1) + (attitude.Fear * 0.2) + ((-attitude.Dominance) * 0.05);
        }

        // TODO: fix attitude of victim TO owner should be changing, not other way round
        private void ApplyMemoryInfluenceToRelationships(Memory memory)
        {
            foreach (MemoryParticipant participant in memory.Cause.Participants)
            {
                if (participant.EntityId == _ownerID)
                {
                    continue;
                }

                if (!_relationships.TryGetValue(participant.EntityId, out Attitude? attitude) || attitude is null)
                {
                    continue;
                }

                double influence = memory.Influence * GetParticipantIntensityWeight(participant.Roles) * 0.05;
                if (participant.Roles.Contains(MemoryRole.Victim) || participant.Roles.Contains(MemoryRole.Target))
                {
                    attitude.Affection -= influence;
                    attitude.Trust -= influence * 0.75;
                }
                else if (participant.Roles.Contains(MemoryRole.Beneficiary))
                {
                    attitude.Affection += influence;
                    attitude.Trust += influence * 0.5;
                }
            }
        }

        #endregion Initial Memory Calculations
    }
}
