﻿using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours
{
    /// <summary>
    /// Changes a specific player stat instantly when used.
    /// </summary>
    internal class OnUseChangePlayerStatBehaviour : ItemBehaviourBase<OnUseChangePlayerStatBehaviour>, IActOnUse
    {
        public PlayerStatTypes StatType;
        public bool ConsumeOnUse { get; set; }

        /// <summary>
        /// How much the player's health will change when this component is used.
        /// </summary>
        public int ChangeAmount { get; private set; }

        public OnUseChangePlayerStatBehaviour(int changeAmount, PlayerStatTypes statType, bool consumeOnUse = true)
        {
            ChangeAmount = changeAmount;
            ConsumeOnUse = consumeOnUse;
            StatType = statType;
        }

        public void OnUse(Player player)
        {
            IOService.Output.WriteLine($"You use the item to change your {StatType} by {ChangeAmount}.");

            switch (StatType)
            {
                case PlayerStatTypes.Health:
                    player.ChangeHealth(ChangeAmount);
                    break;
                case PlayerStatTypes.Mana:
                    break;
                default:
                    throw new ArgumentOutOfRangeException(nameof(StatType), $"Unsupported stat type: {StatType}");
            }

            IOService.Output.WriteLine($"You now have {player.Stats.GetStat(PlayerStatTypes.Health)} HP.");
        }

        public override OnUseChangePlayerStatBehaviour DeepClone()
        {
            return new OnUseChangePlayerStatBehaviour(ChangeAmount, StatType, ConsumeOnUse);
        }
    }
}
