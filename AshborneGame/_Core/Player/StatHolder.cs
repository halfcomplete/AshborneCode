using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core._Player
{
    public class StatHolder
    {
        public PlayerStatType Type { get; private set; }
        public double BaseValue { get; private set; }
        public double BonusValue { get; private set; } = 0;
        public double Total => BaseValue + BonusValue;

        public StatHolder(PlayerStatType statType, double initialValue = 0)
        {
            Type = statType;
            BaseValue = initialValue;
        }

        public void SetBase(double value)
        {
            BaseValue = value;
        }

        public void IncreaseBase(double value)
        {
            BaseValue += value;
        }

        public void LowerBase(double value)
        {
            BaseValue -= value;
            if (BaseValue < 0) BaseValue = 0; // Ensure base value doesn't go negative
        }

        public void AddBonus(double bonus)
        {
            BonusValue += bonus;
        }

        public void RemoveBonus(double bonus)
        {
            BonusValue -= bonus;
            if (BonusValue < 0) BonusValue = 0; // Ensure bonus value doesn't go negative
        }
    }
}
