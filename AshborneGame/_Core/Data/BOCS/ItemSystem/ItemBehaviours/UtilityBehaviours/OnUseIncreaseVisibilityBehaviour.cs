using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Game;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours
{
    internal class OnUseIncreaseVisibilityBehaviour : Behaviour, IActOnUse
    {
        private int _visibilityIncreaseAmount = 3;
        public bool ConsumeOnUse { get; set; }
        public OnUseIncreaseVisibilityBehaviour(int visibilityIncrease, bool consumeOnUse = true)
        {
            _visibilityIncreaseAmount = visibilityIncrease;
            ConsumeOnUse = consumeOnUse;
        }
        public void OnUse(Player player)
        {
            player.SetVariable("visibility", player.Visibility + _visibilityIncreaseAmount);
        }

        public override OnUseIncreaseVisibilityBehaviour DeepClone()
        {
            return new OnUseIncreaseVisibilityBehaviour(_visibilityIncreaseAmount, ConsumeOnUse);
        }
    }
}
