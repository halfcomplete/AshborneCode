using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours
{
    public class EquippableBehaviour : ItemBehaviourBase<EquippableBehaviour>, IEquippable
    {
        public (bool IsEquippable, List<string> BodyParts) EquipInfo { get; set; }
        public int TimesEquipped { get; set; }

        public EquippableBehaviour(List<string> bodyParts, bool isEquippable = true)
        {
            if (bodyParts == null || bodyParts.Count == 0)
            {
                throw new ArgumentException("Body parts cannot be null or empty.", nameof(bodyParts));
            }
            EquipInfo = (isEquippable, bodyParts);
        }

        public void Equip(Player player, Item item, string bodyPart)
        {
            if (string.IsNullOrWhiteSpace(bodyPart) || !player.EquippedItems.ContainsKey(bodyPart.ToLower()) || !EquipInfo.BodyParts.Contains(bodyPart))
            {
                throw new ArgumentException($"Invalid equipment slot: {bodyPart}", nameof(bodyPart));
            }

            player.EquipItem(item, bodyPart);
            IOService.Output.DisplayDebugMessage($"Equipped {item.Name} in the {bodyPart} slot.", ConsoleMessageTypes.INFO);
            IOService.Output.WriteLine($"You equip {item.Name} on your {bodyPart}.");
            IOService.Output.DisplayDebugMessage($"Item Behaviour Values: {item.Behaviours.Values.SelectMany(x => x).OfType<IActOnEquip>().Count()}", ConsoleMessageTypes.INFO);
            
            foreach (var behaviour in item.Behaviours)
            {
                IOService.Output.DisplayDebugMessage($"Behaviour Type: {behaviour.Key.Name}, Count: {behaviour.Value.Count}", ConsoleMessageTypes.INFO);
            }

            foreach (var behaviour in item.Behaviours.Values.SelectMany(x => x).OfType<IActOnEquip>())
            {
                behaviour.OnEquip(player);
            }


        }

        public void Unequip(Player player, Item item, string bodyPart)
        {
            if (string.IsNullOrWhiteSpace(bodyPart) || !player.EquippedItems.ContainsKey(bodyPart.ToLower()))
            {
                throw new ArgumentException($"Invalid equipment slot: {bodyPart}", nameof(bodyPart));
            }

            if (player.EquippedItems[bodyPart] == null)
            {
                throw new InvalidOperationException($"No item is currently equipped in the {bodyPart} slot.");
            }
            player.UnequipItem(item, bodyPart);

            foreach (var behaviour in item.Behaviours.Values.SelectMany(x => x).OfType<IActOnEquip>())
            {
                behaviour.OnUnequip(player);
            }
        }

        public override EquippableBehaviour DeepClone()
        {
            return new EquippableBehaviour(new List<string>(EquipInfo.BodyParts), EquipInfo.IsEquippable);
        }
    }
}
