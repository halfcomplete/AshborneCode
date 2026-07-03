using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory
{
    /// <summary>
    /// Represents an item that can be collected, used, and managed in the game.
    /// Items can have various properties such as usability, equippability, durability, and quality.
    /// Each item type (weapon, tool, equipment, consumable, key) has specific behaviors and restrictions.
    /// </summary>
    /// <remarks>
    /// Item Types and Their Properties:
    /// - Weapons: Can have quality levels and durability
    /// Examples: swords, wands, bows
    /// - Tools: Have durability but no quality. Examples: pickaxes, repair kits, lockpicks
    /// - Equipment: Can be equipped to specific body parts. Examples: armor, rings, amulets
    /// - Consumables: Can be used and stacked. Examples: potions, scrolls, materials
    /// - Keys: Special items used for unlocking. Examples: rusty keys, skeleton keys
    /// </remarks>
    public sealed class Item : BOCSObject
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
        public override string Description { get; }

        #region Constructor
        /// <summary>
        /// Base constructor that takes all parameters. Used internally by other constructors.
        /// </summary>
        public Item(string name, string description, string useDescription, int stackLimit, ItemTypes itemType, ItemQualities quality)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Description = description ?? throw new ArgumentNullException(nameof(description));
            AddBehaviour(typeof(ItemBehaviour), new ItemBehaviour(useDescription, stackLimit, itemType, quality));
        }
        #endregion Constructor
    }
}
