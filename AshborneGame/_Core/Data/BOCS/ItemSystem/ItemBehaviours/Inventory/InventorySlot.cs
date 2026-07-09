using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

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


        public InventorySlotSaveData GetSaveData()
        {
            return new InventorySlotSaveData{ ItemInstanceId = Item.InstanceID, Quantity = Quantity };
        }

        public static InventorySlot LoadFromSaveData(InventorySlotSaveData data, SaveLoadContext context)
        {
            var item = context.ResolveObject(data.ItemInstanceId);
            if (item == null)
            {
                throw new InvalidDataException($"No item found for InstanceID: {data.ItemInstanceId}");
            }
            return new InventorySlot(item, data.Quantity);
        }
    }
}
