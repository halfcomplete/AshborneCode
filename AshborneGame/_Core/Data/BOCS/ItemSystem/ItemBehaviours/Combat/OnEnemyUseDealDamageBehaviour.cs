using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Combat
{
    public class OnEnemyUseDealDamageBehaviour : Behaviour, IUsable, ICanDamage
    {
        public double BaseDamage { get; set; }
        public bool ConsumeOnUse { get; private set; }

        public OnEnemyUseDealDamageBehaviour(double baseDamage, bool consumeOnUse = false)
        {
            BaseDamage = baseDamage;
            ConsumeOnUse = consumeOnUse;
        }

        public async void Use(Player player, string? target = null)
        {
            // Attack player
            player.ChangeHealth(-BaseDamage);
            await IOService.Output.WriteNonDialogueLine($"You take {BaseDamage} damage from the enemy's attack. You now have {player.Stats.GetStat(Globals.Enums.PlayerStatType.Health)} HP left.");
        }

        public override OnEnemyUseDealDamageBehaviour DeepClone()
        {
            return new OnEnemyUseDealDamageBehaviour(BaseDamage, ConsumeOnUse);
        }


        private record SaveData(double BaseDamage, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement((new SaveData(BaseDamage, ConsumeOnUse))));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnEnemyUseDealDamageBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnEnemyUseDealDamageBehaviour save data.");

            BaseDamage = save.BaseDamage;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
