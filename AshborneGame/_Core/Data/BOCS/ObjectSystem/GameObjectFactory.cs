using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Game;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem
{
    public static class GameObjectFactory
    {
        private static (GameObject, DescribableBehaviour) AddDescribableBehaviour(GameObject gameObject)
        {
            var describableBehaviour = new DescribableBehaviour();
            gameObject.AddBehaviour(typeof(IDescribable), describableBehaviour);
            return (gameObject, describableBehaviour);
        }

        public static GameObject CreateChest(string name, string description, bool isLocked = false, bool isOpen = false)
        {
            var gameObject = new GameObject(name, description);
            gameObject.AddBehaviour(typeof(IHasInventory), new ContainerBehaviour());
            gameObject.AddBehaviour(typeof(IInteractable), new OpenCloseBehaviour(gameObject, isOpen));
            gameObject.AddBehaviour(typeof(IInteractable), new LockUnlockBehaviour(gameObject, isLocked));
            (gameObject, var describableBehaviour) = AddDescribableBehaviour(gameObject);
            describableBehaviour.AddCondition(_ =>
                GameContext.Player.Inventory.Slots.Any(slot =>
                    slot.Item.Behaviours.Values
                        .SelectMany(b => b).ToList()
                            .Any(b => b is IUnlocksTarget unlocksTarget && unlocksTarget.UnlockableObjectIDs.Contains(gameObject.ID))
                ),
            "It looks like it can be opened by one of your keys.");

            
            return gameObject;
        }
    }
}
