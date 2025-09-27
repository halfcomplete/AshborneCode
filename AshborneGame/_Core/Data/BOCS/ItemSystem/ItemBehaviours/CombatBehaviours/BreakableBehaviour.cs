using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Services;
using System;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.CombatBehaviours
{
    public class BreakableBehaviour : ItemBehaviourBase<BreakableBehaviour>, IBreakable, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }
        public int Durability { get; set; }
        public int MaxDurability { get; set; }

        public BreakableBehaviour(BOCSGameObject parentObject, int maxDurability)
        {
            if (maxDurability <= 0)
            {
                throw new ArgumentException("Max durability must be greater than zero.", nameof(maxDurability));
            }
            ParentObject = parentObject ?? throw new ArgumentNullException(nameof(parentObject), "Parent object cannot be null.");
            MaxDurability = maxDurability;
            Durability = maxDurability;
        }

        public void OnBreak(Player player)
        {
            // Implementation for what happens when the item breaks
            if (ParentObject is Item item)
            {
                IOService.Output.WriteNonDialogueLine($"{item.Name} has broken!");
                player.Inventory.RemoveItem(item, 1); // Remove the item from the player's inventory
            }
        }

        public override BreakableBehaviour DeepClone()
        {
            return new BreakableBehaviour(ParentObject, MaxDurability) { Durability = Durability };
        }
    }
}
