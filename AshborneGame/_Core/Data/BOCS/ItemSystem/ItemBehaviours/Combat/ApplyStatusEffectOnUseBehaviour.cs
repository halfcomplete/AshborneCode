using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Combat
{
    public class ApplyStatusEffectOnUseBehaviour : Behaviour, IUsable
    {
        public StatusEffectTypes StatusEffectType { get; private set; }
        public bool ConsumeOnUse { get; private set; }

        public ApplyStatusEffectOnUseBehaviour(StatusEffectTypes statusEffectType, bool consumeOnUse = true)
        {
            StatusEffectType = statusEffectType;
            ConsumeOnUse = consumeOnUse;
        }

        public async void Use(Player player, string? target = null)
        {
            // Implementation for applying the status effect to the target
            // For now just print a message
            await IOService.Output.WriteNonDialogueLine($"Applying '{StatusEffectType}' to {target ?? "the target"}.");

            // Apply status effect logic here
        }

        public override ApplyStatusEffectOnUseBehaviour DeepClone()
        {
            return new ApplyStatusEffectOnUseBehaviour(StatusEffectType, ConsumeOnUse);
        }


        private record SaveData(StatusEffectTypes StatusEffectType, bool ConsumeOnUse);

        public override BehaviourSaveData? GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement((new SaveData(StatusEffectType, ConsumeOnUse))));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State) ?? throw new InvalidDataException("Failed to deserialise ApplyStatusEffectOnUseBehaviour save data.");

            StatusEffectType = save.StatusEffectType;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
