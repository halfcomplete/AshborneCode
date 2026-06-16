
using AshborneGame._Core._Player;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules
{
    /// <summary>
    /// A subscriber-type module that is implemented by Behaviours which need to know when the parent object is equipped/unequipped to function.
    /// </summary>
    public interface IActOnEquip
    {
        void OnEquip(Player player);
        void OnUnequip(Player player);
    }
}
