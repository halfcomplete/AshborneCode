using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags;
using System.Diagnostics;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Data.Definitions;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public class MemoryProfile
    {
        private DefinitionID _ownerID;
        private PersonalityProfile _personality;
        private Dictionary<DefinitionID, Attitude> _relationships;
        private List<Memory> _memories;

        public MemoryProfile(DefinitionID ownerID, PersonalityProfile personality, Dictionary<DefinitionID, Attitude> relationships, List<Memory> memories)
        {
            _ownerID = ownerID;
            _personality = personality;
            _relationships = relationships;
            _memories = memories;

            EventBus.Subscribe<GameEvents.System.TickEvent>(e => TickMemoryDecay(e.HoursPassed));
            EventBus.Subscribe<IMemorableGameEvent>(e => ReceiveMemorableEvent(e));
        }

        public MemoryProfile(DefinitionID ownerID, PersonalityProfile personality, Dictionary<DefinitionID, Attitude> relationships) 
            : this(ownerID, personality, relationships, new List<Memory>()) { }

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
            ReceiveMemorySource(e);
        }

        /// <summary>
        /// Creates a memory without requiring a concrete IMemorableGameEvent.
        /// </summary>
        public Memory? ReceiveSyntheticMemory(MemoryDefinition memoryDefinition, int currentTotalHours, DefinitionID locationID, List<MemoryParticipant>? participants = null)
        {
            List<MemoryParticipant> sourceParticipants = participants ?? new();
            SyntheticMemorySource source = new(currentTotalHours, memoryDefinition, sourceParticipants, locationID);

            return ReceiveMemorySource(source);
        }

        public Memory? ReceiveMemorySource(IMemorySource source)
        {
            if (!ShouldReceiveMemorySource(source))
            {
                return null;
            }

            MemoryDefinition def = source.MemoryDefinition;

            Dictionary<EmotionPotential, EmotionAccumulator> initialPotentials = GetInitialEmotionPotentials(def.Tags);
            double intensity = CalculateActualIntensity(def.BaseIntensity, source);

            Dictionary<EmotionPotential, EmotionAccumulator> newPotentials = ApplyPersonalityReactionsToEmotionModifiers(def, initialPotentials);
            Dictionary<EmotionModifier, EmotionAccumulator> emotionModifiers = ExpandEmotionPotentialsIntoEmotionModifiers(source.Participants, newPotentials);
            emotionModifiers = ApplyAttitudeToEmotionalModifiers(source, _relationships, emotionModifiers);

            List<EmotionModifier> accumulatedModifiers = ApplyAccumulatorsToEmotionModifiers(emotionModifiers);

            List<EmotionModifier> finalModifiers = CombineLikeEmotionModifiers(accumulatedModifiers);

            Memory newMemory = new(_ownerID, intensity, source, finalModifiers, def.Tags, source.CurrentTotalHours, source.CurrentTotalHours);

            AddMemory(newMemory);
            ApplyMemoryInfluenceToRelationships(newMemory);

            return newMemory;
        }

        private static Dictionary<EmotionModifier, EmotionAccumulator> ExpandEmotionPotentialsIntoEmotionModifiers(List<MemoryParticipant> participants, Dictionary<EmotionPotential, EmotionAccumulator> newPotentials)
        {
            Dictionary<EmotionModifier, EmotionAccumulator> modifiers = new();

            foreach (var (potential, accumulator) in newPotentials)
            {
                List<MemoryParticipant> targetParticipants = participants.Where(p => p.Roles.Contains(potential.Role)).ToList();

                foreach (var p in targetParticipants)
                {
                    EmotionModifier mod = new(null, p, potential.Emotion, potential.Value);

                    modifiers.Add(mod, accumulator);
                }
            }

            return modifiers;
        }

        public void StrengthenMemories(MemoryQuery query, double amount)
        {
            List<Memory> targetMemories = _memories.Where(m => m.Matches(query)).ToList();

            foreach (var memory in targetMemories)
            {
                memory.Strength += amount;
            }
        }

        /// <summary>
        /// Takes a list of EmotionModifiers and combines ones with the same emotion type and target.
        /// </summary>
        /// <returns>A list of EmotionModifiers where each one has a unique emotion type and target.</returns>
        private static List<EmotionModifier> CombineLikeEmotionModifiers(List<EmotionModifier> modifiers)
        {
            if (modifiers == null || modifiers.Count == 0)
            {
                return new List<EmotionModifier>();
            }

            List<EmotionModifier> combinedModifiers = modifiers
                .GroupBy(m => (m.Type, m.Target))
                .Select(group =>
                {
                    double totalAmount = group.Sum(m => m.InitialAmount);

                    EmotionModifier first = group.First();

                    return new EmotionModifier(
                        parentMemory: null,
                        target: first.Target,
                        type: first.Type,
                        initialAmount: totalAmount
                    );
                })
                .ToList();

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
            double newInitialAmount = mod.InitialAmount + accumulator.TotalAdd;

            if (accumulator.TotalMult >= 0)
            {
                newInitialAmount *= accumulator.TotalMult;
            }
            else
            {
                newInitialAmount /= accumulator.TotalMult;
            }

            return mod with { InitialAmount = newInitialAmount };
        }
        
        private Dictionary<EmotionPotential, EmotionAccumulator> ApplyPersonalityReactionsToEmotionModifiers(MemoryDefinition def, Dictionary<EmotionPotential, EmotionAccumulator> initialPotentials)
        {
            // Loop over each MemoryTag in the given MemoryDefinition
            foreach (MemoryTag tag in def.Tags)
            {
                // Get the reactions from each personality trait that this MemoryTag defines
                Dictionary<PersonalityTrait, List<EmotionReaction>> PersonalityReactions = MemoryTagDefinitions.Definitions[tag].Definition.PersonalityEmotionModifiers;

                // Loop over each personality trait in PersonalityReactions
                // personalityTrait is an enumeration (either Curiosity, Compassion or Aggression)
                foreach (var (personalityTrait, personalityReactions) in PersonalityReactions)
                {
                    // Loop over every reaction in this MemoryTag's personalityTrait's personalityReactions
                    // Each 'reaction' contains the emotion that is affected and a mult and add value
                    foreach (EmotionReaction reaction in personalityReactions)
                    {
                        // Track whether this reaction affects an emotion that is already in the given initialModifiers
                        bool seen = false;

                        // For every EmotionModifier and EmotionAccumulator in the given initialModifieres, 
                        // check if the modified emotion is the same as this reaction's emotion.
                        foreach (var (potential, accumulator) in initialPotentials)
                        {
                            if (potential.Emotion == reaction.Emotion)
                            {
                                // If both this reaction and the current emotion potential being checked modify the same emotion,
                                // then add to the associated EmotionAccumulator's TotalMult and mark seen as true.
                                accumulator.TotalMult = CalculateEmotionAccumulatorTotalMult(accumulator.TotalMult, _personality.PersonalityTraits[personalityTrait], reaction.Mult);
                                seen = true;
                            }
                        }

                        // If there were no initial emotion modifiers that modify reaction.Emotion then add a new EmotionPotential
                        if (!seen)
                        {
                            EmotionPotential potential = new(reaction.Emotion, reaction.Role, reaction.Add * _personality.PersonalityTraits[personalityTrait]);
                            initialPotentials.Add(potential, new EmotionAccumulator());
                        }
                    }
                }
            }

            return initialPotentials;
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

        public List<Memory> GetMemoriesByCause(IMemorySource cause) => _memories.Where(m => m.Cause == cause).ToList();

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

        public bool RemembersEvent(IMemorySource cause) => _memories.Any(m => m.Cause == cause);

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
        /// Takes a Hashset of MemoryTags and returns the base EmotionPotentials that the given MemoryTags define.
        /// </summary>
        /// <param name="tags">The MemoryTags to parse and interpret.</param>
        /// <param name="currentTotalHours">The current total hours of the game.</param>
        /// <returns></returns>
        private static Dictionary<EmotionPotential, EmotionAccumulator> GetInitialEmotionPotentials(HashSet<MemoryTag> tags)
        {
            Dictionary<EmotionPotential, EmotionAccumulator> mods = new();

            foreach (MemoryTag tag in tags)
            {
                MemoryTagDefinition tagDefinition = MemoryTagDefinitions.Definitions[tag].Definition;

                foreach (var (emotion, (role, value)) in tagDefinition.BaseEmotionalModifiers)
                {
                    mods[new EmotionPotential(emotion, role, value)] = new EmotionAccumulator();
                }
            }

            return mods;
        }
        
        private bool ShouldReceiveMemorySource(IMemorySource source)
        {
            return source.Participants.Any(participant => participant.EntityId == _ownerID);
        }

        private double CalculateActualIntensity(double baseIntensity, IMemorySource source)
        {
            double intensity = baseIntensity;

            intensity += GetPersonalityIntensityImpact(_personality, source.MemoryDefinition.Tags);

            intensity += GetAttitudeIntensityImpact(_relationships, source.MemoryDefinition.Tags, source.Participants);

            return Math.Clamp(intensity, 0.0, 1.0);
        }

        /// <summary>
        /// Takes the Relationships an NPC has, the Tags a Memory has, and the Participants in that Memory, 
        /// and returns how much the Memory's intensity should change based on those inputs.
        /// </summary>
        /// <remarks>
        /// For example, if the NPC has a hostile relationship with someone else, and that person is a Victim of 
        /// their house burning down, then the intensity of the Memory (how significant that is to the NPC) would 
        /// decrease as they don't care.
        /// </remarks>
        private static double GetAttitudeIntensityImpact(Dictionary<DefinitionID, Attitude> relationships, HashSet<MemoryTag> tags, List<MemoryParticipant> participants)
        {
            double intensityImpact = 0.0;

            foreach (MemoryTag tag in tags)
            {
                var attitudeIntensityModifiers = MemoryTagDefinitions.Definitions[tag].Definition.AttitudeIntensityModifiers;

                foreach ((RelationshipType attitudeType, List<AttitudeRoleIntensityRule> intensityRules) in attitudeIntensityModifiers)
                {
                    foreach (AttitudeRoleIntensityRule rule in intensityRules)
                    {
                        // for each rule, find out who in the participants list is affected by that rule
                        List<MemoryParticipant> targetParticipants = participants.Where(p => p.Roles.Contains(rule.Role)).ToList();

                        // loop over each participant who is affected by that rule
                        // (e.g, loop over every participant who was a Target)
                        // figure out, for each participant, if we have a relationship with them
                        // if so, then figure out how our intensity should be impacted based on that relationship and what the rule says
                        foreach (MemoryParticipant participant in targetParticipants)
                        {
                            if (relationships.TryGetValue(participant.EntityId, out var attitude))
                            {
                                // for each attitude type, check if the given attitude actually aligns with that attitude type
                                double alignment = Math.Abs(GetAttitudeAlignmentWithAttitudeType(attitude, attitudeType));
                                intensityImpact += rule.Weight * alignment;
                            }
                        }
                    }
                }
            }

            return intensityImpact;
        }

        private static double GetAttitudeAlignmentWithAttitudeType(Attitude attitude, RelationshipType attitudeType)
        {
            return attitudeType switch
            {
                RelationshipType.Loves => Math.Max(0, attitude.Affection),
                RelationshipType.Hates => Math.Min(0, attitude.Affection),

                RelationshipType.Trusts => Math.Max(0, attitude.Trust),
                RelationshipType.Distrusts => Math.Min(0, attitude.Trust),

                RelationshipType.Respects => Math.Max(0, attitude.Respect),
                RelationshipType.Disrespects => Math.Min(0, attitude.Respect),

                RelationshipType.Fears => Math.Max(0, attitude.Fear),
                RelationshipType.DoesNotFear => Math.Min(0, attitude.Fear),

                RelationshipType.Dominates => Math.Max(0, attitude.Dominance),
                RelationshipType.Submits => Math.Min(0, attitude.Dominance),

                _ => throw new ArgumentOutOfRangeException(nameof(attitudeType), $"Unhandled attitude: {attitudeType}")
            };
        }

        /// <summary>
        /// Takes the personality an NPC has and the Memory tags of a memory and returns
        /// how much the Memory's intensity for this NPC should change because of the NPC's personality.
        /// </summary>
        private double GetPersonalityIntensityImpact(PersonalityProfile personality, HashSet<MemoryTag> tags)
        {
            double impact = 0;

            foreach (MemoryTag tag in tags)
            {
                MemoryTagDefinition tagDef = MemoryTagDefinitions.Definitions[tag].Definition;

                foreach (var (personalityTrait, intensityMod) in tagDef.PersonalityIntensityModifiers)
                {
                    impact += intensityMod * personality.PersonalityTraits[personalityTrait];
                }
            }

            return impact;
        }

        // TODO: review this function; can we make it any more efficient?
        /// <summary>
        /// Takes a game event, the NPC's relationships and the current emotional modifiers of a memory and returns a Dictionary of the previous emotion modifiers and the updated emotion accumulators.
        /// </summary>
        private static Dictionary<EmotionModifier, EmotionAccumulator> ApplyAttitudeToEmotionalModifiers(IMemorySource source, Dictionary<DefinitionID, Attitude> relationships, Dictionary<EmotionModifier, EmotionAccumulator> mods)
        {
            // Loops over each memory tag in the memory definition
            // Gets the memory tag's AttitudeEmotionModifiers
            // Get all MemoryParticipants in the event where this NPC's attitude towards them
            // is a key in the memory tag's AttitudeEmotionModifiers
            // Get the alignment with the attitude type

            foreach (MemoryTag tag in source.MemoryDefinition.Tags)
            {
                var attitudeEmotionModifiers = MemoryTagDefinitions.Definitions[tag].Definition.AttitudeEmotionModifiers;

                foreach (var (attitudeType, attitudeRoleEmotionRules) in attitudeEmotionModifiers)
                {
                    foreach (AttitudeRoleEmotionRule rule in attitudeRoleEmotionRules)
                    {
                        // figure out who in the list of participants satisfies the rule's memory role
                        List<MemoryParticipant> targetParticipants = source.Participants.Where(p => p.Roles.Contains(rule.Role)).ToList();
                        
                        // loop over each participant who is affected by that rule
                        // (e.g, loop over every participant who was a Target)
                        // figure out, for each participant, if we have a relationship with them
                        // if so, then figure out how the emotion accumulators should be impacted based on that relationship and what the rule says
                        foreach (MemoryParticipant participant in targetParticipants)
                        {
                            if (relationships.TryGetValue(participant.EntityId, out var attitude))
                            {
                                // now we've figured out that this participant, we have a relationship with them
                                // figure out if the rule says anything about our relationship type
                                double alignment = Math.Abs(GetAttitudeAlignmentWithAttitudeType(attitude, attitudeType));

                                // if the rule modifies emotion modifiers with this relationship type
                                if (alignment > 0)
                                {
                                    EmotionReaction reaction = rule.Reaction;

                                    var targetMods = mods.Where(m => m.Key.Type == reaction.Emotion);

                                    foreach (var (emotionMod, emotionAccumulator) in targetMods)
                                    {
                                        emotionAccumulator.TotalMult = CalculateEmotionAccumulatorTotalMult(emotionAccumulator.TotalMult, alignment, reaction.Mult);
                                    }

                                    if (targetMods.Count() == 0)
                                    {
                                        mods.Add(new(null, participant, reaction.Emotion, reaction.Add * alignment), new());
                                    }
                                }
                            }
                        }
                    }
                }
            }

            return mods;
        }

        private void ApplyMemoryInfluenceToRelationships(Memory memory)
        {
            List<EmotionModifier> emotionModifiers = memory.EmotionModifiers;

            foreach (var modifier in emotionModifiers)
            {
                MemoryParticipant target = modifier.Target;
                EmotionType emotion = modifier.Type;

                if (!_relationships.Keys.ToList().Contains(target.EntityId))
                {
                    // if we don't have a relationship with this entity yet
                    List<AttitudeReaction> attitudeReactions = EmotionToAttitudeMap.Reactions[emotion];

                    _relationships.Add(target.EntityId, AttitudeFactory.CreateAttitude(attitudeReactions));
                }
                else
                {
                    List<AttitudeReaction> attitudeReactions = EmotionToAttitudeMap.Reactions[emotion];
                    _relationships[target.EntityId] = AttitudeFactory.ModifyAttitude(_relationships[target.EntityId], attitudeReactions);
                }
            }
        }

        #endregion Initial Memory Calculations
    }
}
