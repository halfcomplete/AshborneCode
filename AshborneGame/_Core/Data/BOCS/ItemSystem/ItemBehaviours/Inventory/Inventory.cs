using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using System.Text;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

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
        private List<BOCSObject> _items = new();

        private Dictionary<string, List<BOCSObject>> _byReferenceName = new();

        /// <summary>
        /// Initializes a new empty inventory.
        /// </summary>
        public Inventory() { }

        /// <summary>
        /// Adds an item to the inventory, stacking where possible.
        /// </summary>
        public bool TryAddItem(BOCSObject item)
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

            ArgumentNullException.ThrowIfNull(nameof(item));

            // Check if the item is already in the inventory
            var existing = _items.FirstOrDefault(s => s.InstanceID == item.InstanceID);

            if (existing != null)
            {
                // If the item is already in the inventory, we can't add it again
                return false;
            }

            _items.Add(item);
            if (!_byReferenceName.ContainsKey(item.Name))
            {
                _byReferenceName[item.Name] = new List<BOCSObject>();
            }
            _byReferenceName[item.Name].Add(item);
            return true;
        }

        public bool TryAddItem(DefinitionID definitionID, int count = 1)
        {
            for (int i = 0; i < count; i++)
            {
                var item = GameContext.BOCSFactory.Create(definitionID);
                if (item == null)
                {
                    throw new ArgumentException($"No item found for DefinitionID: {definitionID.Value}");
                }
                if (!TryAddItem(item))
                {
                    return false;
                }
            }
            
            return true;
        }

        /// <summary>
        /// Removes a quantity of an item from the inventory.
        /// </summary>
        /// <returns>True if the given obj is an item, false otherwise.</returns>
        public async Task<bool> TryRemoveItem(BOCSObject item)
        {
            if (!item.IsItem()) return false;

            if (item == null)
            {
                throw new ArgumentNullException(nameof(item));
            }

            if (_items.Remove(item))
            {
                _byReferenceName[item.Name].Remove(item);
                if (_byReferenceName[item.Name].Count == 0)
                {
                    _byReferenceName.Remove(item.Name);
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Finds the first item by name.
        /// </summary>
        public BOCSObject? GetItem(string itemName)
        {
            return _items.FirstOrDefault(s => s.Name.Matches(itemName));
        }

        public List<BOCSObject> GetItems(string itemName)
        {
            return _byReferenceName.GetValueOrDefault(itemName, new List<BOCSObject>());
        }

        public int GetItemCount(string itemName)
        {
            return _byReferenceName.TryGetValue(itemName, out var items) ? items.Count : 0;
        }

        /// <summary>
        /// Returns a textual summary of the inventory.
        /// </summary>
        public async Task<(bool, string)> GetInventoryContents(Player player)
        {
            if (_items.Count == 0)
                return (true, "");

            var sb = new StringBuilder();

            ArgumentNullException.ThrowIfNull(player);

            foreach (var kvp in _byReferenceName)
            {
                var definitionID = kvp.Key;
                var items = kvp.Value;
                if (items.Count > 0)
                {
                    var itemName = items[0].Name;
                    var count = items.Count;
                    sb.AppendLine($"{count} x {itemName}");
                }
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
        public async void TransferItem(Inventory originInventory, Inventory destinationInventory, BOCSObject item)
        {
            if (!item.IsItem())
            {
                throw new ArgumentException($"Can't transfer {item.Name} between inventories as it's not an item!");
            }
            await IOService.Output.DisplayDebugMessage($"Transferring {item.Name} from {originInventory.GetType().Name} to {destinationInventory.GetType().Name}.");
            await originInventory.TryRemoveItem(item);
            destinationInventory.TryAddItem(item);
        }

        public async void TransferItemsByName(Inventory originInventory, Inventory destinationInventory, string itemName, int quantity)
        {
            var item = originInventory.GetItem(itemName);
            if (item == null)
            {
                throw new ArgumentException($"Item '{itemName}' not found in inventory.");
            }
            if (!item.IsItem())
            {
                throw new ArgumentException($"Can't transfer {item.Name} between inventories as it's not an item!");
            }
            await IOService.Output.DisplayDebugMessage($"Transferring {item.Name} from {originInventory.GetType().Name} to {destinationInventory.GetType().Name}.");
            await originInventory.TryRemoveItem(item);
            destinationInventory.TryAddItem(item);
        }

        public async void TransferAllItems(Inventory originInventory, Inventory destinationInventory)
        {
            await IOService.Output.DisplayDebugMessage($"Transferring all items from {originInventory.GetType().Name} to {destinationInventory.GetType().Name}.");
            
            var uneditedInventory = new Inventory();
            uneditedInventory._items.AddRange(originInventory._items);
            foreach (var item in uneditedInventory._items)
            {
                destinationInventory.TryAddItem(item);
                await originInventory.TryRemoveItem(item);
            }
        }


        public InventorySaveData GetSaveData()
        {
            return new InventorySaveData { Items = _items.Select(item => item.InstanceID).ToList() };
        }

        public void LoadSaveData(InventorySaveData saveData, SaveLoadContext context)
        {
            _items.Clear();
            _byReferenceName.Clear();
            foreach (var instanceID in saveData.Items)
            {
                var item = context.InstanceRegistry.Get(instanceID);
                _items.Add(item);
                if (_byReferenceName.ContainsKey(item.Name))
                {
                    _byReferenceName[item.Name].Add(item);
                }
                else
                {
                    _byReferenceName[item.Name] = new List<BOCSObject> { item };
                }
            }
        }
    }
}
