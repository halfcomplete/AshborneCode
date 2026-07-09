using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours
{
    public class StorableBehaviour : Behaviour, IStorable
    {
        public override string SaveId => "storable";
        /// <summary>
        /// Gets the maximum number of this item that can be stacked in a single inventory slot.
        /// Default is 1 for most items, but can be higher for consumables like potions or materials.
        /// </summary>
        /// <remarks>
        /// For example:
        /// <br></br>
        /// <example>
        /// 1 for weapons and equipment
        /// <br></br>
        /// 32 for gold coins
        /// <br></br>
        /// 10 for potions
        /// </example>
        /// </remarks>
        public int StackLimit { get; private set; } = 1;

        /// <summary>
        /// Gets the type of the item, which determines its behavior and restrictions.
        /// Each type has specific rules about what properties it can have.
        /// </summary>
        /// <remarks>
        /// Type-specific rules:
        /// - Weapons: Can have quality and durability
        /// - Tools: Can have durability but not quality
        /// - Equipment: Can be equipped to specific body parts
        /// - Consumables: Can be used and stacked
        /// - Keys: Special items for unlocking
        /// </remarks>
        public ItemTypes ItemType { get; private set; } = ItemTypes.Consumable;

        /// <summary>
        /// Gets the quality level of the item. Only applies to weapons.
        /// Higher quality weapons may have better stats or effects.
        /// </summary>
        /// <remarks>
        /// Quality levels from lowest to highest:
        /// - None: Not a weapon
        /// - Common: Basic weapon
        /// - Uncommon: Slightly better
        /// - Rare: Good weapon
        /// - Epic: Very powerful
        /// - Mythic: Extremely powerful
        /// - Legendary: Best possible quality
        /// </remarks>
        public ItemQualities Quality { get; private set; }

        public StorableBehaviour(int stackLimit, ItemTypes itemType, ItemQualities itemQuality)
        {
            StackLimit = stackLimit;
            ItemType = itemType;
            Quality = itemQuality;
        }

        public override StorableBehaviour DeepClone() => new(StackLimit, ItemType, Quality);


        private record SaveData(int StackLimit, ItemTypes ItemType, ItemQualities Quality);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(StackLimit, ItemType, Quality)));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("StorableBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise StorableBehaviour save data.");

            StackLimit = save.StackLimit;
            ItemType = save.ItemType;
            Quality = save.Quality;
        }
    }
}