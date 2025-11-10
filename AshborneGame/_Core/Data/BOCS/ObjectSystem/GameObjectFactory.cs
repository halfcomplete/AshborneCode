using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.SceneManagement;

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

        public static async Task<GameObject> CreateDoor(string name, string description, Location location, bool isOpen = false)
        {
            var gameObject = new GameObject(name, description);
            gameObject.AddBehaviour(typeof(IInteractable), new OpenCloseBehaviour(gameObject, isOpen));
            gameObject.AddBehaviour(typeof(IExit), new ExitToNewLocationBehaviour(location));
            (gameObject, var describableBehaviour) = AddDescribableBehaviour(gameObject);
            var (hasBehaviour, behaviour) = await gameObject.TryGetBehaviour<OpenCloseBehaviour>();
            describableBehaviour.AddCondition(_ => (
                hasBehaviour && behaviour is OpenCloseBehaviour openCloseBehaviour && openCloseBehaviour.IsOpen
                ),
            "It's slightly ajar.");

            return gameObject;
        }

        public static GameObject CreateLockableDoor(string name, string description, Location location, bool isLocked = false, bool isOpen = false)
        {
            var gameObject = new GameObject(name, description);
            gameObject.AddBehaviour(typeof(IHasInventory), new ContainerBehaviour());
            gameObject.AddBehaviour(typeof(IInteractable), new OpenCloseBehaviour(gameObject, isOpen));
            gameObject.AddBehaviour(typeof(IInteractable), new LockUnlockBehaviour(gameObject, isLocked));
            gameObject.AddBehaviour(typeof(IExit), new ExitToNewLocationBehaviour(location));
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

        public static GameObject CreateDecoration(string name, string description)
        {
            var gameObject = new GameObject(name, description);
            return gameObject;
        }
    }
}
