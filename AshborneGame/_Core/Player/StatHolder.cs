using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core._Player
{
    public class StatHolder
    {
        public PlayerStatType Type { get; private set; }
        public int BaseValue { get; private set; }
        public int BonusValue { get; private set; } = 0;
        public int Total => BaseValue + BonusValue;

        public StatHolder(PlayerStatType statType, int initialValue = 0)
        {
            Type = statType;
            BaseValue = initialValue;
        }

        public void SetBase(int value)
        {
            BaseValue = value;
        }

        public void IncreaseBase(int value)
        {
            BaseValue += value;
        }

        public void LowerBase(int value)
        {
            BaseValue -= value;
            if (BaseValue < 0) BaseValue = 0; // Ensure base value doesn't go negative
        }

        public void AddBonus(int bonus)
        {
            BonusValue += bonus;
        }

        public void RemoveBonus(int bonus)
        {
            BonusValue -= bonus;
            if (BonusValue < 0) BonusValue = 0; // Ensure bonus value doesn't go negative
        }
    }
}
