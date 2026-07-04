using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class TradeableNPCBehaviour : Behaviour, IHasInventory
    {
        public Inventory Inventory { get; private set; } = new Inventory();

        public bool IsTradeable { get; set; } = true;
        // TODO: implement inventory sizes

        // TODO: implement inventory deep clone (i think this is needed bcuz what if we need to save/load an npc with an inventory)
        public override TradeableNPCBehaviour DeepClone() => new();
    }
}
