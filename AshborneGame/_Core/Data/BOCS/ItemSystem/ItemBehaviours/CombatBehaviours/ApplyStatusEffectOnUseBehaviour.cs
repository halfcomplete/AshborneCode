using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.CombatBehaviours
{
    public class ApplyStatusEffectOnUseBehaviour : ItemBehaviourBase<ApplyStatusEffectOnUseBehaviour>, IUsable
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
    }
}
