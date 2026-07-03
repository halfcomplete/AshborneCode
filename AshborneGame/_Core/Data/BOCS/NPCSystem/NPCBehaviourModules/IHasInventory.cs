using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules
{
    public interface IHasInventory
    {
        Inventory Inventory { get; }
    }
}
