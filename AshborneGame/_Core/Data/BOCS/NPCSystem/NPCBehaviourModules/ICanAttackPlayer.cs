using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
public interface ICanAttackPlayer
{
    Item Weapon { get; set; }
    int BaseAttackPower { get; set; }
    int AttackPower { get; }
    int AttackDamage { get; }
}
