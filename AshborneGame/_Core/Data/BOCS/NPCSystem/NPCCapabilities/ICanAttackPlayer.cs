using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
public interface ICanAttackPlayer
{
    BOCSObject Weapon { get; set; }
    int BaseAttackPower { get; set; }
    int AttackPower { get; }
    int AttackDamage { get; }
}
