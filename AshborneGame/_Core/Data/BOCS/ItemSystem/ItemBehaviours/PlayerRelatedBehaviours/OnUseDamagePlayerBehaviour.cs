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
    internal class OnUseDamagePlayerBehaviour : Behaviour, IActOnUse
    {
        public bool ConsumeOnUse { get; set; }
        public double DamageAmount { get; private set; }
        public OnUseDamagePlayerBehaviour(double damageAmount, bool consumeOnUse = true)
        {
            DamageAmount = damageAmount;
            ConsumeOnUse = consumeOnUse;
        }
        public async void OnUse(Player player)
        {
            // Logic to apply damage to the player
            player.ChangeHealth(-DamageAmount);
            await IOService.Output.WriteNonDialogueLine($"You take {DamageAmount} damage. You now have {player.Stats.GetStat(PlayerStatType.Health)} HP.");
        }

        public override OnUseDamagePlayerBehaviour DeepClone()
        {
            return new OnUseDamagePlayerBehaviour(DamageAmount, ConsumeOnUse);
        }


        private record SaveData(double DamageAmount, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(DamageAmount, ConsumeOnUse)));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnUseDamagePlayerBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnUseDamagePlayerBehaviour save data.");
            DamageAmount = save.DamageAmount;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
