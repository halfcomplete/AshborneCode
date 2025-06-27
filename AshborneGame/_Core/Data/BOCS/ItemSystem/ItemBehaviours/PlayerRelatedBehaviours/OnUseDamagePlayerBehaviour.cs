using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours
{
    internal class OnUseDamagePlayerBehaviour : ItemBehaviourBase<OnUseDamagePlayerBehaviour>, IActOnUse
    {
        public bool ConsumeOnUse { get; set; }
        public int DamageAmount { get; private set; }
        public OnUseDamagePlayerBehaviour(int damageAmount, bool consumeOnUse = true)
        {
            DamageAmount = damageAmount;
            ConsumeOnUse = consumeOnUse;
        }
        public void OnUse(Player player)
        {
            // Logic to apply damage to the player
            player.ChangeHealth(-DamageAmount);
            IOService.Output.WriteLine($"You take {DamageAmount} damage. You now have {player.Stats.GetStat(PlayerStatType.Health)} HP.");
        }

        public override OnUseDamagePlayerBehaviour DeepClone()
        {
            return new OnUseDamagePlayerBehaviour(DamageAmount, ConsumeOnUse);
        }
    }
}
