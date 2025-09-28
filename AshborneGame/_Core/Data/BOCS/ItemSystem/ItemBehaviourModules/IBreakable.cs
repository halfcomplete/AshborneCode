using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules
{
    public interface IBreakable
    {
        int Durability { get; set; }
        int MaxDurability { get; set; }
        bool IsBroken => Durability <= 0;

        async void Damage(Player player, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Damage amount cannot be negative.", nameof(amount));
            }
            Durability -= amount;
            await IOService.Output.WriteNonDialogueLine($"Item damaged by {amount}. Current durability: {Durability}/{MaxDurability}.");
            if (Durability <= 0)
            {
                Durability = 0;
                OnBreak(player);
            }
        }

        void Repair(Player player, int amount)
        {
            if (amount < 0)
            {
                throw new ArgumentException("Repair amount cannot be negative.", nameof(amount));
            }
            Durability += amount;
            if (Durability > MaxDurability)
            {
                Durability = MaxDurability;
            }
        }

        void OnBreak(Player player);
    }
}
