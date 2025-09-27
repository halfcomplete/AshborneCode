using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours
{
    internal class OnEquipChangePlayerStatBehaviour : ItemBehaviourBase<OnEquipChangePlayerStatBehaviour>, IActOnEquip
    {
        private PlayerStatType StatType { get; set; }

        private int ChangeAmount { get; set; }

        public OnEquipChangePlayerStatBehaviour(int changeAmount, PlayerStatType statType)
        {
            StatType = statType;
            ChangeAmount = changeAmount;
        }

        public void OnEquip(Player player)
        {
            player.Stats.AddBonus(StatType, ChangeAmount);
            IOService.Output.WriteNonDialogueLine($"Your {StatType} has been increased by {ChangeAmount} while this item is equipped.");
        }

        public void OnUnequip(Player player)
        {
            player.Stats.RemoveBonus(StatType, ChangeAmount);
            IOService.Output.WriteNonDialogueLine($"Your {StatType} has been decreased by {ChangeAmount} after unequipping this item.");
        }

        public override OnEquipChangePlayerStatBehaviour DeepClone()
        {
            return new OnEquipChangePlayerStatBehaviour(ChangeAmount, StatType);
        }
    }
}
