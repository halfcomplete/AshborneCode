using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    public class ContainerBehaviour : Behaviour, IHasInventory
    {
        public Inventory Inventory { get; private set; } = new Inventory();

        // TODO: deep clone inventory? and constructor?
        public override ContainerBehaviour DeepClone()
        {
            return new ContainerBehaviour();
        }
    }
}
