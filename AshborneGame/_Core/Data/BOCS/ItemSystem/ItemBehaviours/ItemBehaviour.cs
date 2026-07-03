using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours
{
    public class ItemBehaviour : ItemBehaviourBase<ItemBehaviour>
    {
        /// <summary>
        /// Gets the description that appears when the item is used.
        /// Only applicable if the item is usable (Usable = true).
        /// </summary>
        /// <example>"The potion heals your wounds", "The wand shoots a bolt of lightning"</example>
        public string UseDescription { get; }

        /// <summary>
        /// Gets the maximum number of this item that can be stacked in a single inventory slot.
        /// Default is 1 for most items, but can be higher for consumables like potions or materials.
        /// </summary>
        /// <example>
        /// 1 for weapons and equipment
        /// 32 for gold coins
        /// 10 for potions
        /// </example>
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

        public ItemBehaviour(string useDescription, int stackLimit, ItemTypes itemType, ItemQualities itemQuality)
        {
            UseDescription = useDescription ?? throw new ArgumentNullException(nameof(useDescription));
            StackLimit = stackLimit;
            ItemType = itemType;
            Quality = itemQuality;
        }

        public override ItemBehaviour DeepClone() => new(UseDescription, StackLimit, ItemType, Quality);
    }
}