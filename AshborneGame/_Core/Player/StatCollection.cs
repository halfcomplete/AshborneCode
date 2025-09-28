using AshborneGame._Core.Globals.Enums;
using System.Text;
using System.Xml.Linq;

namespace AshborneGame._Core._Player
{
    public class StatCollection
    {
        private readonly Dictionary<PlayerStatType, StatHolder> _stats = new();

        public StatCollection()
        {
            // Initialise all stats with default values
            foreach (PlayerStatType statType in Enum.GetValues(typeof(PlayerStatType)))
            {
                if (statType == PlayerStatType.NA) continue; // Skip NA type
                int initialValue;

                switch (statType)
                {
                    case PlayerStatType.Health:
                        initialValue = 100;
                        break;
                    case PlayerStatType.MaxHealth:
                        initialValue = 100;
                        break;
                    case PlayerStatType.Strength:
                        initialValue = 10;
                        break;
                    case PlayerStatType.Defense:
                        initialValue = 10;
                        break;
                    default:
                        initialValue = 3;
                        break;
                }

                _stats[statType] = new StatHolder(statType, initialValue);
            }
        }

        public StatHolder this[PlayerStatType type] => _stats[type];

        /// <summary>
        /// Gets the base, bonus, and total value of a specific stat.
        /// </summary>
        /// <exception cref="ArgumentException"></exception>
        public (int, int, int) GetStat(PlayerStatType type)
        {
            if (_stats.TryGetValue(type, out var statHolder))
            {
                int baseValue = statHolder.BaseValue;
                int bonusValue = statHolder.BonusValue;
                int totalValue = statHolder.Total;

                return (baseValue, bonusValue, totalValue);
            }
            else
            {
                throw new ArgumentException($"Stat {type} does not exist.");
            }
        }

        /// <summary>
        /// Gets the base, bonus and total value of a player stat by name.
        /// </summary>
        /// <param name="statName"></param>
        /// <returns>(int baseValue, int bonusValue, int totalValue)</returns>
        public bool GetStat(string statName, out (int, int, int) stat)
        {
            stat = (0, 0, 0);
            if (TryGetStatTypeByName(statName, out var statType)) stat = GetStat(statType);
            else return false;
            return true;
        }

        public bool TryGetStatTypeByName(string statName, out PlayerStatType statType)
        {
            statType = PlayerStatType.NA;
            switch (statName.ToLowerInvariant())
            {
                case "health":
                    statType = PlayerStatType.Health;
                    break;
                case "maxhealth":
                    statType = PlayerStatType.MaxHealth;
                    break;
                case "strength":
                    statType = PlayerStatType.Strength;
                    break;
                case "defense":
                    statType = PlayerStatType.Defense;
                    break;
                case "guilt":
                    statType = PlayerStatType.Guilt;
                    break;
                case "fear":
                    statType = PlayerStatType.Fear;
                    break;
                case "violence":
                    statType = PlayerStatType.Violence;
                    break;
                case "hope":
                    statType = PlayerStatType.Hope;
                    break;
                case "powerhunger":
                    statType = PlayerStatType.PowerHunger;
                    break;
                case "resolve":
                    statType = PlayerStatType.Resolve;
                    break;
                default:
                    break;
            }
            if (statType == PlayerStatType.NA)
                return false;
            return true;
        }

        public void SetBase(string statName, int value)
        {
            if (TryGetStatTypeByName(statName, out var statType)) _stats[statType].SetBase(value);
        }
        public void ChangeBase(string statName, int amount)
        {
            if (TryGetStatTypeByName(statName, out var statType)) _stats[statType].SetBase(_stats[statType].BaseValue + amount);
        }

        public void AddBonus(string statName, int bonus)
        {
            if (TryGetStatTypeByName(statName, out var statType)) _stats[statType].AddBonus(bonus);
        }

        public void RemoveBonus(string statName, int bonus)
        {
            if (TryGetStatTypeByName(statName, out var statType)) _stats[statType].RemoveBonus(bonus);
        }

        public void SetBase(PlayerStatType type, int value)
        {
            if (type == PlayerStatType.NA) return;
            _stats[type].SetBase(value);
        }

        public void ChangeBase(PlayerStatType type, int amount)
        {
            if (type == PlayerStatType.NA) return;
            _stats[type].SetBase(_stats[type].BaseValue + amount);
        }

        public void AddBonus(PlayerStatType type, int bonus)
        {
            if (type == PlayerStatType.NA) return;
            _stats[type].AddBonus(bonus);
        }

        public void RemoveBonus(PlayerStatType type, int bonus)
        {
            if (type == PlayerStatType.NA) return;
            _stats[type].RemoveBonus(bonus);
        }

        public string GetFormattedStats()
        {
            var sb = new StringBuilder();
            foreach (var stat in _stats.Values)
            {
                sb.AppendLine(string.Format("{0, -10} {1, -3} ({2} + {3})", stat.Type + ":", stat.Total, stat.BaseValue, stat.BonusValue));
            }
            return sb.ToString();
        }
    }
}
