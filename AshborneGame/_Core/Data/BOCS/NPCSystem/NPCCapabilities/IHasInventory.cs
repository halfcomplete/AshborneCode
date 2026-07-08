using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities
{
    public interface IHasInventory
    {
        Inventory Inventory { get; }
    }
}
