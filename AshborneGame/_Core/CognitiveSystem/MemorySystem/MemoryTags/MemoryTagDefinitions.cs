using AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags.Tags;


namespace AshborneGame._Core.CognitiveSystem.MemorySystem.MemoryTags
{
    public static class MemoryTagDefinitions
    {
        public static Dictionary<MemoryTagType, IMemoryTag> Definitions = 
            new Dictionary<MemoryTagType, IMemoryTag> 
            {
                { MemoryTagType.Theft, new TheftMemoryTag() },
                { MemoryTagType.Betrayal, new BetrayalMemoryTag() },
                { MemoryTagType.Deception, new DeceptionMemoryTag() },
                { MemoryTagType.Cruelty, new CrueltyMemoryTag() },
                { MemoryTagType.Remorse, new RemorseMemoryTag() },
                { MemoryTagType.Care, new CareMemoryTag() },
                { MemoryTagType.Loss, new LossMemoryTag() },
                { MemoryTagType.Kindness, new KindnessMemoryTag() },
                { MemoryTagType.Gift, new GiftMemoryTag() },
                { MemoryTagType.Help, new HelpMemoryTag() },
            };
    }
}