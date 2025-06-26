using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.CombatBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.OtherBehaviours;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.MaskBehaviours;
using System.Diagnostics;

namespace AshborneGame._Core.Data.BOCS.ItemSystem
{
    public static class ItemFactory
    {
        private static Item AddBaseBehaviours(Item item)
        {
            item.AddBehaviour(typeof(IInspectable), new InspectableBehaviour(item, item.Description, item.Quality, "This item has a hidden lore."));
            
            return item;
        }

        public static Item CreateEnemyWeapon(string name, string description, ItemQualities quality, int maxDurability)
        {
            Item item = new Item(name, description, "", 1, ItemTypes.Weapon, quality);
            item.AddBehaviour(typeof(ICanDamage), new OnEnemyUseDealDamageBehaviour(25));
            return AddBaseBehaviours(item);
        }

        public static Item CreateHealthPotion(string name, int healAmount)
        {
            var item = new Item(
                name,$"Heals {healAmount} HP", 
                "You feel refreshed while drinking the bubbly health potion", 10, ItemTypes.Consumable, ItemQualities.None);
            item.AddBehaviour(typeof(IUsable), new UsableBehaviour(item));
            item.AddBehaviour(typeof(IActOnUse), new OnUseHealPlayerBehaviour(healAmount));
            item.AddBehaviour(typeof(IActOnUse), new OnUseLogMessage("Player drank health potion!"));
            return AddBaseBehaviours(item);
        }

        public static Item CreateKey(string name, string description, string useDescription, List<string> unlockableObjectIDs)
        {
            var item = new Item(name, description, useDescription, 1, ItemTypes.Key, ItemQualities.None);
            item.AddBehaviour(typeof(IUsable), new UsableBehaviour(item));
            item.AddBehaviour(typeof(IActOnUse), new OnUseUnlockObjectBehaviour(unlockableObjectIDs, false));
            item.AddBehaviour(typeof(IActOnUse), new OnUseLogMessage($"Key {name} used to unlock an object."));
            return AddBaseBehaviours(item);
        }

        public static Item CreateArmour(string name, string description, List<string> bodyParts, ItemQualities quality, int maxDurability, Dictionary<PlayerStatTypes, int> statModifiers)
        {
            var item = new Item(name, description, "", 1, ItemTypes.Armour, quality);
            item.AddBehaviour(typeof(IEquippable), new EquippableBehaviour(bodyParts));
            foreach (var modifier in statModifiers)
            {
                item.AddBehaviour(typeof(IActOnEquip), new OnEquipChangePlayerStatBehaviour(modifier.Value, modifier.Key));
            }
            return AddBaseBehaviours(item);
        }

        public static Item CreateEquipment(string name, string description, List<string> bodyParts, int stackLimit = 1)
        {
            var item = new Item(name, description, "", stackLimit, ItemTypes.Equipment, ItemQualities.None);
            item.AddBehaviour(typeof(IEquippable), new EquippableBehaviour(bodyParts));
            item.AddBehaviour(typeof(IUsable), new UsableBehaviour(item));
            item.AddBehaviour(typeof(IActOnUse), new OnUseLogMessage($"Stackable equipment {name} used."));
            return AddBaseBehaviours(item);
        }

        public static Item CreateMagicScroll(string name, string description, string useDescription, int stackLimit = 1)
        {
            var item = new Item(name, description, useDescription, stackLimit, ItemTypes.Consumable, ItemQualities.Mythic);
            item.AddBehaviour(typeof(IUsable), new UsableBehaviour(item));
            item.AddBehaviour(typeof(IActOnUse), new OnUseChangePlayerStatBehaviour(30, PlayerStatTypes.Strength));
            item.AddBehaviour(typeof(IIdentifiable), new IdentifiableBehaviour(item, "This scroll contains powerful magic."));
            item.AddBehaviour(typeof(IInspectable), new InspectableBehaviour(item, item.Description, item.Quality, "This item has a hidden lore.", true));
            return item;
        }

        public static Item CreateLightSourceEquipment(string name, string description, List<string> bodyParts, ItemQualities quality, int maxDurability, int stackLimit = 1)
        {
            var item = new Item(name, description, "", stackLimit, ItemTypes.Equipment, quality);
            item.AddBehaviour(typeof(IUsable), new UsableBehaviour(item));
            item.AddBehaviour(typeof(IEquippable), new EquippableBehaviour(bodyParts));
            item.AddBehaviour(typeof(IActOnEquip), new OnEquipChangeEnvironmentStatBehaviour("Light source effect occurred! Yay."));
            item.AddBehaviour(typeof(IActOnUse), new OnUseLogMessage($"Light source {name} equipped."));
            return AddBaseBehaviours(item);
        }

        public static Item CreateWeapon(string name, string description, ItemQualities quality, int maxDurability, int baseDamage)
        {
            var item = new Item(name, description, "", 1, ItemTypes.Weapon, quality);
            item.AddBehaviour(typeof(IEquippable), new EquippableBehaviour(new List<string> { "hand" }));
            item.AddBehaviour(typeof(ICanDamage), new OnPlayerUseDealDamageBehaviour(baseDamage));
            item.AddBehaviour(typeof(IBreakable), new BreakableBehaviour(item, maxDurability));
            return AddBaseBehaviours(item);
        }

        
    }
}
