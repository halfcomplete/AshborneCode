
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules
{
    public interface IEquippable
    {
        public List<string> EquippableSlots { get; set; }
        public int TimesEquipped { get; set; }
        bool IsEquipped { get; set; }

        void Equip(Player player, string bodyPart);

        void Unequip(Player player, string bodyPart);
    }
}
