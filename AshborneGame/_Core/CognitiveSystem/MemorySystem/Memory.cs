using AshborneGame._Core.Data.BOCS.NPCSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.CognitiveSystem.MemorySystem
{
    public class Memory
    {
        /// <summary>
        /// A unique ID for the Memory.
        /// </summary>
        public Guid ID { get; init; }

        /// <summary>
        /// The NPC that owns this Memory.
        /// </summary>
        public Guid Owner { get; init; }

        private double _intensity;

        /// <summary>
        /// How significant this Memory is to the NPC. Rarely changes.
        /// </summary>
        /// <remarks>
        /// Determines how strongly the Memory affects the NPC's emotions, whether it affect the NPC's attitudes, and how long the Memory persists.
        /// Note that this only answers "How much does this event matter?", not "How clearly do I remember it?" (strength) nor "How do I feel about it?" (emotion modifiers)
        /// <para>
        /// Intensity may change through:
        /// <list type="bullet">
        /// <item>Repetition: The player lies repeatedly (3+ times) and the NPC realises it is a pattern rather than an isolated incident; the memory of the lies increases in significance.</item>
        /// <item>Escalation: The player steals from the NPC and later burns down the NPC's home. The earlier theft increases in significance as it would seem like a warning sign.</item>
        /// <item>Major events: A seemingly insignificant conversation becomes the NPC's last interaction with a deceased friend. The memory of the conversation gains significance because of later events.</item>
        /// </list>
        /// </para>
        /// <para>
        /// Intensity is subjective and depends on the NPC's personality.
        /// Intensity is different from Strength and Influence as it is how <i>important</i> the Memory is to the NPC.
        /// </para>
        /// Constrained from 0 (the NPC regards the memory as completely unimportant) to 1 (the Memory is very, very important/significant to the NPC).
        /// </remarks>
        public double Intensity
        {
            get => _intensity;
            set
            {
                _intensity = Math.Min(1, Math.Max(0, value));
            }
        }

        private double _strength;

        /// <summary>
        /// How strongly the NPC remembers this Memory. Decays over time depending on the Intensity but may be reinforced.
        /// </summary>
        /// <remarks>
        /// Differs from Intensity as it essentially simulates the NPC's tendency to forget memories over time.
        /// Constrained from 0 (the NPC has forgotten the Memory) to 1 (the Memory is fresh in the NPC's mind).
        /// </remarks>
        public double Strength 
        { 
            get => _strength; 
            set
            {
                _strength = Math.Min(1, Math.Max(GetStrengthFloorBasedOnIntensity(Intensity), value));
            }
        }

        public bool IsActive { get => Strength > 0.15f; }

        /// <summary>
        /// How much this Memory actually influences the NPC's decision making. Higher values means this Memory has greater influence.
        /// </summary>
        /// <remarks>
        /// Influence is higher the more recently this Memory was created or reinforced.
        /// Influence is also higher the more Intense this Memory is (i.e., the more important this Memory is to the NPC).
        /// </remarks>
        public double Influence { get => Intensity * Strength; }

        /// <summary>
        /// The cause of this Memory/what created the Memory.
        /// </summary>
        public IMemorableGameEvent Cause { get; init; }

        /// <summary>
        /// The EmotionModifiers associated with this Memory; how this Memory affects the NPC's emotions.
        /// </summary>
        public List<EmotionModifier> EmotionModifiers { get; private set; }

        public HashSet<MemoryTag> Tags { get; init; }

        /// <summary>
        /// The total in-game hours when this Memory was created.
        /// </summary>
        public int HourCreatedAt { get; init; }

        /// <summary>
        /// The total in-game hours when this Memory was last reinforced.
        /// </summary>
        public int HourLastReinforcedAt { get; private set; }

        public Memory(Guid owner, double intensity, IMemorableGameEvent cause, List<EmotionModifier> emotionModifiers, HashSet<MemoryTag> tags, int hourCreatedAt, int hourLastReinforcedAt)
        {
            ID = Guid.NewGuid();
            Owner = owner;
            Intensity = intensity;
            Strength = 1.0f;
            Cause = cause;
            EmotionModifiers = emotionModifiers;
            Tags = tags;
            HourCreatedAt = hourCreatedAt;
            HourLastReinforcedAt = hourLastReinforcedAt;
        }

        public static double GetStrengthFloorBasedOnIntensity(double intensity)
        {
            return (double)Math.Min(1.0, Math.Max(0.0, Math.Pow(intensity, 3.0)/2.0));
        }

        public void Reinforce(int currentTotalInGameHours)
        {
            HourLastReinforcedAt = currentTotalInGameHours;
        }

        public int GetAgeInHours(int currentTotalInGameHours) => currentTotalInGameHours - HourCreatedAt;

        public int GetHoursSinceLastReinforce(int currentTotalInGameHours) => currentTotalInGameHours - HourLastReinforcedAt;
    }
}
