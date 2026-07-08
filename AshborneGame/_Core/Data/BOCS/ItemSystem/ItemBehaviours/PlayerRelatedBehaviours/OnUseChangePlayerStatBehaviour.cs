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
    /// <summary>
    /// Changes a specific player stat instantly when used.
    /// </summary>
    internal class OnUseChangePlayerStatBehaviour : Behaviour, IActOnUse
    {
        public PlayerStatType StatType;
        public bool ConsumeOnUse { get; set; }

        /// <summary>
        /// How much the player's health will change when this component is used.
        /// </summary>
        public double ChangeAmount { get; private set; }

        public OnUseChangePlayerStatBehaviour(double changeAmount, PlayerStatType statType, bool consumeOnUse = true)
        {
            ChangeAmount = changeAmount;
            ConsumeOnUse = consumeOnUse;
            StatType = statType;
        }

        public async void OnUse(Player player)
        {
            await IOService.Output.WriteNonDialogueLine($"You use the item to change your {StatType} by {ChangeAmount}.");
            // TODO: add support for other stat types in the future
            switch (StatType)
            {
                case PlayerStatType.Health:
                    player.ChangeHealth(ChangeAmount);
                    break;
                default:
                    //throw new ArgumentOutOfRangeException(nameof(StatType), $"Unsupported stat type: {StatType}");
                    break;
            }

            await IOService.Output.WriteNonDialogueLine($"You now have {player.Stats.GetStat(PlayerStatType.Health)} HP.");
        }

        public override OnUseChangePlayerStatBehaviour DeepClone()
        {
            return new OnUseChangePlayerStatBehaviour(ChangeAmount, StatType, ConsumeOnUse);
        }


        private record SaveData(double ChangeAmount, PlayerStatType StatType, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(ChangeAmount, StatType, ConsumeOnUse)));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnUseChangePlayerStatBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnUseChangePlayerStatBehaviour save data.");
            ChangeAmount = save.ChangeAmount;
            StatType = save.StatType;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
