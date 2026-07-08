using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours
{
    internal class OnEquipChangePlayerStatBehaviour : Behaviour, IActOnEquip
    {
        public PlayerStatType StatType { get; set; }
        public double ChangeAmount { get; set; }

        public OnEquipChangePlayerStatBehaviour(PlayerStatType statType, double changeAmount)
        {
            StatType = statType;
            ChangeAmount = changeAmount;
        }

        public async void OnEquip(Player player)
        {
            player.Stats.AddBonus(StatType, ChangeAmount);
            await IOService.Output.WriteNonDialogueLine($"Your {StatType} has been increased by {ChangeAmount} while this item is equipped.");
        }

        public async void OnUnequip(Player player)
        {
            player.Stats.RemoveBonus(StatType, ChangeAmount);
            await IOService.Output.WriteNonDialogueLine($"Your {StatType} has been decreased by {ChangeAmount} after unequipping this item.");
        }

        public override Behaviour DeepClone()
        {
            return new OnEquipChangePlayerStatBehaviour(StatType, ChangeAmount);
        }
    }
}
