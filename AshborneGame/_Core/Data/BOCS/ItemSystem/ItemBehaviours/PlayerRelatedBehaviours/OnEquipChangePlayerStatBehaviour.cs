using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours
{
    internal class OnEquipChangePlayerStatBehaviour : Behaviour, IActOnEquip
    {
        public override string SaveId => "onEquipChangePlayerStat";

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


        private record SaveData(PlayerStatType StatType, double ChangeAmount);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(StatType, ChangeAmount)));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnEquipChangePlayerStatBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnEquipChangePlayerStatBehaviour save data.");

            StatType = save.StatType;
            ChangeAmount = save.ChangeAmount;
        }
    }
}
