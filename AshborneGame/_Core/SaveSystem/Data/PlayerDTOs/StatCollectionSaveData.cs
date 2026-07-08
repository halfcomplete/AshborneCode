using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.SaveSystem.Data.PlayerDTOs
{
    public class StatCollectionSaveData
    {
        public Dictionary<PlayerStatType, StatHolderSaveData> Stats { get; set; } = new();
    }

    public class StatHolderSaveData
    {
        public PlayerStatType Type { get; set; }
        public int BaseValue { get; set; }
        public int BonusValue { get; set; }
    }
}