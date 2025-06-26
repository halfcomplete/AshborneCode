using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Data.BOCS.ItemSystem
{
    /// <summary>
    /// Represents an item that can be collected, used, and managed in the game.
    /// Items can have various properties such as usability, equippability, durability, and quality.
    /// Each item type (weapon, tool, equipment, consumable, key) has specific behaviors and restrictions.
    /// </summary>
    /// <remarks>
    /// Item Types and Their Properties:
    /// - Weapons: Can have quality levels and durability. Examples: swords, wands, bows
    /// - Tools: Have durability but no quality. Examples: pickaxes, repair kits, lockpicks
    /// - Equipment: Can be equipped to specific body parts. Examples: armor, rings, amulets
    /// - Consumables: Can be used and stacked. Examples: potions, scrolls, materials
    /// - Keys: Special items used for unlocking. Examples: rusty keys, skeleton keys
    /// </remarks>
    public class Item : BOCSGameObject
    {
        /// <summary>
        /// Gets the name of the object. This is used for identification and display.
        /// </summary>
        /// <example>"Iron Sword", "Health Potion", "Rusty Key"</example>
        public override string Name { get; }

        /// <summary>
        /// Gets the description of the item. This is shown when the item is inspected.
        /// </summary>
        /// <example>"A sharp blade made of iron", "A red liquid that smells sweet"</example>
        public string Description { get; }

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

        #region Constructor
        /// <summary>
        /// Base constructor that takes all parameters. Used internally by other constructors.
        /// </summary>
        public Item(string name, string description, string useDescription, int stackLimit, ItemTypes itemType, ItemQualities quality)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            UseDescription = useDescription ?? throw new ArgumentNullException(nameof(useDescription));
            StackLimit = stackLimit;
            ItemType = itemType;
            Quality = quality;
        }
        #endregion Constructor
    }
}
