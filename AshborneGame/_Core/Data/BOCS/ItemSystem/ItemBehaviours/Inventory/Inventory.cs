using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using System.Text;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory
{
    /// <summary>
    /// Represents a collection of items that can be carried by a player or stored in a container.
    /// Uses InventorySlot to manage item stacking and quantities.
    /// </summary>
    public class Inventory
    {
        /// <summary>
        /// Internal list of inventory slots.
        /// </summary>
        private readonly List<InventorySlot> _slots = new();

        /// <summary>
        /// Public read-only view of the inventory slots.
        /// </summary>
        public IReadOnlyList<InventorySlot> Slots => _slots.AsReadOnly();

        /// <summary>
        /// Initializes a new empty inventory.
        /// </summary>
        public Inventory() { }

        /// <summary>
        /// Adds an item to the inventory, stacking where possible.
        /// </summary>
        public bool TryAddItem(BOCSObject item, int count = 1)
        {
            var res = item.TryGetBehaviour<IStorable>().GetAwaiter().GetResult();
            IStorable b;

            if (res.Item1 && res.Item2 != null)
            {
                b = res.Item2;
            }
            else
            {
                return false;
            }

            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0.", nameof(count));

            int remaining = count;

            // Fill existing stacks
            foreach (var slot in _slots)
            {
                if (slot.Item.Name.Matches(item.Name.ReferenceName) && !slot.IsFull)
                {
                    remaining = slot.Add(remaining);
                    if (remaining <= 0) return true;
                }
            }

            // Create new stacks
            while (remaining > 0)
            {
                int toAdd = Math.Min(b.StackLimit, remaining);
                var newItem = GameContext.BOCSFactory.Clone(item);
                
                _slots.Add(new InventorySlot(newItem, toAdd));
                remaining -= toAdd;
            }

            return true;
        }

        public bool TryAddItem(DefinitionID definitionID, int count = 1)
        {
            var item = GameContext.BOCSFactory.Create(definitionID);
            if (item == null)
            {
                throw new ArgumentException($"No item found for DefinitionID: {definitionID.Value}");
            }
            return TryAddItem(item, count);
        }

        /// <summary>
        /// Removes a quantity of an item from the inventory.
        /// </summary>
        /// <returns>True if the given obj is an item, false otherwise.</returns>
        public async Task<bool> TryRemoveItem(BOCSObject obj, int count = 1)
        {
            if (!obj.IsItem()) return false;

            if (obj == null)
                throw new ArgumentNullException(nameof(obj));
            if (count <= 0)
                throw new ArgumentException("Count must be greater than 0.", nameof(count));

            var relevantSlots = _slots
                .Where(s => s.Item.Name.Matches(obj.Name.ReferenceName))
                .OrderByDescending(s => s.Quantity)
                .ToList();

            int removed = 0;

            foreach (var slot in relevantSlots)
            {
                if (removed >= count) break;

                int needed = count - removed; // Tracks how many items left need to be removed
                int prevQuantity = slot.Quantity; // Tracks how many items were originally in this current inventory slot
                int left = slot.Remove(needed); // The number of items left
                removed += prevQuantity - left; // Tracks how many items have been removed

                if (slot.IsEmpty)
                    _slots.Remove(slot); // Delete the inventory slot if it is empty
            }

            if (removed < count)
            {
                // If we didn't find enough items to remove but still removed as many as possible
                await IOService.Output.DisplayDebugMessage($"Not enough items to remove {count} of them!");
            }

            return true;
        }

        /// <summary>
        /// Finds the first item by name.
        /// </summary>
        public BOCSObject? GetItem(string itemName)
        {
            return _slots
                .Select(slot => slot.Item)
                .FirstOrDefault(item => item.Name.Matches(itemName));
        }

        /// <summary>
        /// Returns a textual summary of the inventory.
        /// </summary>
        public async Task<(bool, string)> GetInventoryContents(Player player)
        {
            if (_slots.Count == 0)
                return (true, "");

            var sb = new StringBuilder();

            ArgumentNullException.ThrowIfNull(player);

            foreach (var slot in _slots)
            {
                var equipped = string.Empty;
                var (hasBehaviour, equippableBehaviour) = await slot.Item.TryGetBehaviour<IEquippable>();
                if (hasBehaviour && equippableBehaviour != null)
                {
                    // If the item is equippable, check if it's equipped
                    equipped = equippableBehaviour.IsEquipped ? "(Equipped)" : "";
                }
                
                sb.AppendLine($"{slot.Quantity} x {slot.Item.Name} - {slot.Item.Description} {equipped}");
            }

            return (false, sb.ToString());
        }

        public async void PrintInventoryContents(Player player)
        {
            var (isEmpty, contents) = await GetInventoryContents(player);
            if (isEmpty)
            {
                await IOService.Output.WriteNonDialogueLine("Your inventory is empty.");
            }
            else
            {
                await IOService.Output.WriteNonDialogueLine("Your inventory contains:");
                await IOService.Output.WriteNonDialogueLine(contents);
            }
        }

        /// <summary>
        /// Transfers items between two inventories.
        /// </summary>
        public async void TransferItem(Inventory originInventory, Inventory destinationInventory, BOCSObject item, int count)
        {
            if (!item.IsItem())
            {
                throw new ArgumentException($"Can't transfer {item.Name} between inventories as it's not an item!");
            }
            await IOService.Output.DisplayDebugMessage($"Transferring {count} x {item.Name} from {originInventory.GetType().Name} to {destinationInventory.GetType().Name}.");
            await originInventory.TryRemoveItem(item, count);
            destinationInventory.TryAddItem(item, count);
        }

        public async void TransferAllItems(Inventory originInventory, Inventory destinationInventory)
        {
            await IOService.Output.DisplayDebugMessage($"Transferring all items from {originInventory.GetType().Name} to {destinationInventory.GetType().Name}.");
            
            var uneditedInventory = new Inventory();
            uneditedInventory._slots.AddRange(originInventory.Slots.Select(slot => new InventorySlot(slot.Item, slot.Quantity)));
            foreach (var slot in uneditedInventory.Slots)
            {
                if (!slot.IsEmpty)
                {
                    destinationInventory.TryAddItem(slot.Item, slot.Quantity);
                    originInventory.TryRemoveItem(slot.Item, slot.Quantity);
                }
            }
        }
    }
}
