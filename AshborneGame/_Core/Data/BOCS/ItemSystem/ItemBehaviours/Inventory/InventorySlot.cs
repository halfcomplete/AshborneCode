using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory
{
    public class InventorySlot
    {
        public BOCSObject Item { get; }
        public int Quantity { get; private set; }

        public bool IsFull
        {
            get
            {
                var (res, b) = Item.TryGetBehaviour<IStorable>().Result;
                if (!res || b == null)
                {
                    throw new Exception($"Item {Item.Name} doesn't have IStorable attached.");
                }
                return Quantity >= b.StackLimit;
            }
        }

        public InventorySlot(BOCSObject item, int quantity = 1)
        {
            var (res, b) = item.TryGetBehaviour<IStorable>().Result;
            if (!res || b == null)
            {
                throw new Exception($"Item {item.Name} doesn't have IStorable attached.");
            }

            Item = item ?? throw new ArgumentNullException(nameof(item));
            Quantity = Math.Clamp(quantity, 0, b.StackLimit);
        }

        /// <summary>
        /// Returns the number of items that could not be added due to stack limit but also adds as many as possible.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Add(int amount)
        {
            var (res, b) = Item.TryGetBehaviour<IStorable>().Result;
            if (!res || b == null)
            {
                throw new Exception($"Item {Item.Name} doesn't have IStorable attached.");
            }
            int space = b.StackLimit - Quantity;
            int toAdd = Math.Min(space, amount);
            Quantity += toAdd;
            return amount - toAdd; // leftover
        }

        /// <summary>
        /// Returns the number of items that are left in the inventory slot but also removes as many as possible.
        /// </summary>
        /// <param name="amount"></param>
        /// <returns></returns>
        public int Remove(int amount)
        {
            int toRemove = Math.Min(amount, Quantity);
            Quantity -= toRemove;
            return amount - toRemove;
        }

        public bool IsEmpty => Quantity <= 0;
    }

}
