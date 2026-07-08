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
    internal class OnUseHealPlayerBehaviour : Behaviour, IActOnUse
    {
        public bool ConsumeOnUse { get; set; }
        public double HealAmount { get; private set; }
        public OnUseHealPlayerBehaviour(double healAmount, bool consumeOnUse = true)
        {
            HealAmount = healAmount;
            ConsumeOnUse = consumeOnUse;
        }
        public async void OnUse(Player player)
        {
            // Logic to heal the player
            player.ChangeHealth(HealAmount);
            await IOService.Output.WriteNonDialogueLine($"You heal for {HealAmount} HP. You now have {player.Stats.GetStat(PlayerStatType.Health)} HP.");
        }

        public override OnUseHealPlayerBehaviour DeepClone()
        {
            return new OnUseHealPlayerBehaviour(HealAmount, ConsumeOnUse) { ConsumeOnUse = this.ConsumeOnUse };
        }


        private record SaveData(double HealAmount, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(HealAmount, ConsumeOnUse)));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnUseHealPlayerBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnUseHealPlayerBehaviour save data.");
            HealAmount = save.HealAmount;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
