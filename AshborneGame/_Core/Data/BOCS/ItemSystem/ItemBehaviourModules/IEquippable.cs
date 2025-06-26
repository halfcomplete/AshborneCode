
using AshborneGame._Core._Player;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules
{
    public interface IEquippable
    {
        (bool IsEquippable, List<string> BodyParts) EquipInfo { get; set; }
        int TimesEquipped { get; set; }

        void Equip(Player player, Item item, string bodyPart);

        void Unequip(Player player, Item item, string bodyPart);
    }
}
