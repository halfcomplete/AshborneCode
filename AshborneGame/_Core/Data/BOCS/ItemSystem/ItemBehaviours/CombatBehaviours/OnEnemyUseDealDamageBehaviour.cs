using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.CombatBehaviours
{
    public class OnEnemyUseDealDamageBehaviour : ItemBehaviourBase<OnEnemyUseDealDamageBehaviour>, IUsable, ICanDamage
    {
        public int BaseDamage { get; set; }
        public bool ConsumeOnUse { get; private set; }

        public OnEnemyUseDealDamageBehaviour(int baseDamage, bool consumeOnUse = false)
        {
            BaseDamage = baseDamage;
            ConsumeOnUse = consumeOnUse;
        }

        public void Use(Player player, string? target = null)
        {
            // Attack player
            player.ChangeHealth(-BaseDamage);
            IOService.Output.WriteNonDialogueLine($"You take {BaseDamage} damage from the enemy's attack. You now have {player.Stats.GetStat(Globals.Enums.PlayerStatType.Health)} HP left.");
        }

        public override OnEnemyUseDealDamageBehaviour DeepClone()
        {
            return new OnEnemyUseDealDamageBehaviour(BaseDamage, ConsumeOnUse);
        }
    }
}
