using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours
{
    internal class OnUseHealPlayerBehaviour : ItemBehaviourBase<OnUseHealPlayerBehaviour>, IActOnUse
    {
        public bool ConsumeOnUse { get; set; }
        public int HealAmount { get; private set; }
        public OnUseHealPlayerBehaviour(int healAmount, bool consumeOnUse = true)
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
    }
}
