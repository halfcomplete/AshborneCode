using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours
{
    public class EquippableBehaviour : Behaviour, IEquippable
    {
        public List<string> EquippableSlots { get; set; }
        public int TimesEquipped { get; set; }
        public bool IsEquipped { get; set; }

        public EquippableBehaviour(List<string> equippableSlots, int timesEquipped = 0)
        {
            ArgumentNullException.ThrowIfNull(equippableSlots);

            if (equippableSlots.Count == 0)
            {
                throw new ArgumentException("Body parts cannot be empty.", nameof(equippableSlots));
            }

            EquippableSlots = equippableSlots;
            TimesEquipped = timesEquipped;
        }

        // TODO: remove async
        public async void Equip(Player player, string bodyPart)
        {
            if (string.IsNullOrWhiteSpace(bodyPart) || !player.EquippedItems.ContainsKey(bodyPart.ToLower()) || !EquippableSlots.Contains(bodyPart))
            {
                throw new ArgumentException($"Invalid equipment slot: {bodyPart}", nameof(bodyPart));
            }

            player.EquipItem(Owner, bodyPart);
            await IOService.Output.DisplayDebugMessage($"Equipped {Owner.Name} in the {bodyPart} slot.", ConsoleMessageTypes.INFO);
            await IOService.Output.WriteNonDialogueLine($"You equip {Owner.Name} on your {bodyPart}.");
            await IOService.Output.DisplayDebugMessage($"Item Behaviour Values: {Owner.ByModule.Values.SelectMany(x => x).OfType<IActOnEquip>().Count()}", ConsoleMessageTypes.INFO);
            
            foreach (var behaviour in Owner.ByModule)
            {
                await IOService.Output.DisplayDebugMessage($"Behaviour Type: {behaviour.Key.Name}, Count: {behaviour.Value.Count}", ConsoleMessageTypes.INFO);
            }

            foreach (var behaviour in Owner.ByModule.Values.SelectMany(x => x).OfType<IActOnEquip>())
            {
                behaviour.OnEquip(player);
            }

            IsEquipped = true;
        }

        // TODO: do we really need to pass in the entire Player object every time?
        public void Unequip(Player player, string bodyPart)
        {
            if (string.IsNullOrWhiteSpace(bodyPart) || !player.EquippedItems.ContainsKey(bodyPart.ToLower()))
            {
                throw new ArgumentException($"Invalid equipment slot: {bodyPart}", nameof(bodyPart));
            }

            if (player.EquippedItems[bodyPart] == null)
            {
                throw new InvalidOperationException($"No item is currently equipped in the {bodyPart} slot.");
            }
            player.UnequipItem(bodyPart);

            foreach (var behaviour in Owner.ByModule.Values.SelectMany(x => x).OfType<IActOnEquip>())
            {
                behaviour.OnUnequip(player);
            }

            IsEquipped = false;
        }

        public override Behaviour DeepClone()
        {
            return new EquippableBehaviour(new(EquippableSlots), TimesEquipped);
        }
    }
}
