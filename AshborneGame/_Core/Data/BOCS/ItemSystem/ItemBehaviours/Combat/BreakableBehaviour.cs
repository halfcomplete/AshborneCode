using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Combat
{
    public class BreakableBehaviour : Behaviour, IBreakable
    {
        public int Durability { get; set; }
        public int MaxDurability { get; set; }

        public BreakableBehaviour(int maxDurability)
        {
            if (maxDurability <= 0)
            {
                throw new ArgumentException("Max durability must be greater than zero.", nameof(maxDurability));
            }
            MaxDurability = maxDurability;
            Durability = maxDurability;
        }

        // TODO: remove async void
        public async void OnBreak(Player player)
        {
            // Implementation for what happens when the item breaks
            if (Owner.IsItem())
            {
                await IOService.Output.WriteNonDialogueLine($"{Owner.Name} has broken!");
                await player.Inventory.TryRemoveItem(Owner, 1); // Remove the item from the player's inventory
            }
        }

        public override BreakableBehaviour DeepClone()
        {
            return new BreakableBehaviour(MaxDurability) { Durability = Durability };
        }


        private record SaveData(int Durability, int MaxDurability);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement((new SaveData(Durability, MaxDurability))));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("BreakableBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise BreakableBehaviour save data.");

            Durability = save.Durability;
            MaxDurability = save.MaxDurability;
        }
    }
}
