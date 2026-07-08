using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Combat
{
    /// <summary>
    /// Deals a specific amount of damage to a target when used.
    /// </summary>
    public class OnPlayerUseDealDamageBehaviour : Behaviour, IActOnUse, ICanDamage
    {
        public double BaseDamage { get; set; }
        public bool ConsumeOnUse { get; set; }
        public OnPlayerUseDealDamageBehaviour(double damageAmount, bool consumeOnUse = true)
        {
            BaseDamage = damageAmount;
            ConsumeOnUse = consumeOnUse;
        }
        public async void OnUse(Player player)
        {
            // Implementation for dealing damage to the target
            // For now just print a message
            await IOService.Output.WriteNonDialogueLine($"You deal {BaseDamage} damage to the target!");
        }

        public override OnPlayerUseDealDamageBehaviour DeepClone()
        {
            return new OnPlayerUseDealDamageBehaviour(BaseDamage, ConsumeOnUse);
        }


        private record SaveData(double BaseDamage, bool ConsumeOnUse);

        public override BehaviourSaveData? GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement((new SaveData(BaseDamage, ConsumeOnUse))));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State) ?? throw new InvalidDataException("Failed to deserialise OnPlayerUseDealDamageBehaviour save data.");

            BaseDamage = save.BaseDamage;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
