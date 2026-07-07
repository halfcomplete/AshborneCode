using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours
{
    public class ItemBehaviour : Behaviour, IStorable
    {
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
        public int StackLimit { get; } = 1;

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
        public ItemTypes ItemType { get; } = ItemTypes.Consumable;

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
        public ItemQualities Quality { get; }

        public ItemBehaviour(int stackLimit, ItemTypes itemType, ItemQualities itemQuality)
        {
            StackLimit = stackLimit;
            ItemType = itemType;
            Quality = itemQuality;
        }

        public override ItemBehaviour DeepClone() => new(StackLimit, ItemType, Quality);
    }
}