using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.CombatBehaviours
{
    /// <summary>
    /// Deals a specific amount of damage to a target when used.
    /// </summary>
    public class OnPlayerUseDealDamageBehaviour : ItemBehaviourBase<OnPlayerUseDealDamageBehaviour>, IActOnUse, ICanDamage
    {
        public int BaseDamage { get; set; }
        public bool ConsumeOnUse { get; set; }
        public OnPlayerUseDealDamageBehaviour(int damageAmount, bool consumeOnUse = true)
        {
            BaseDamage = damageAmount;
            ConsumeOnUse = consumeOnUse;
        }
        public void OnUse(Player player)
        {
            // Implementation for dealing damage to the target
            // For now just print a message
            IOService.Output.WriteNonDialogueLine($"You deal {BaseDamage} damage to the target!");
        }

        public override OnPlayerUseDealDamageBehaviour DeepClone()
        {
            return new OnPlayerUseDealDamageBehaviour(BaseDamage, ConsumeOnUse);
        }
    }
}
