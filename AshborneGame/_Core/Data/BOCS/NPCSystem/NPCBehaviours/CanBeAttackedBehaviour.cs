using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    /// <summary>
    /// An NPC-specific Behaviour that allows the NPC to be attacked/damaged by the player or another hostile entity.
    /// <para>This Behaviour notifies other Behaviours that respond to attacks through its public <c>Attack(int)</c> method.</para>
    /// </summary>
    public class CanBeAttackedBehaviour : Behaviour, ICanBeAttacked
    {
        public double Health { get; set; }
        public double MaxHealth { get; set; }

        public CanBeAttackedBehaviour(double maxHealth)
        {
            Health = MaxHealth = maxHealth;
        }

        // TODO: remove async void
        /// <summary>
        /// Reduces the health of the NPC by <c>damage</c> points. If <c>Health</c> is below 1, the NPC dies.
        /// </summary>
        /// <remarks>
        /// Additionally notifies all of the parent objects' Behaviours that implement the <c>IActOnAttacked</c> module.
        /// </remarks>
        public async void Attacked(double damage)
        {
            Health -= damage;

            if (Owner.GetAllBehavioursOfType<IActOnAttacked>().Any())
            {
                await IOService.Output.DisplayDebugMessage($"The enemy {Owner.Name} has been attacked and took {damage} damage.", ConsoleMessageTypes.INFO);
                foreach (var behaviour in Owner.GetAllBehavioursOfType<IActOnAttacked>())
                {
                    behaviour.OnAttacked();
                }
            }
            else
            {
                await IOService.Output.DisplayDebugMessage($"The enemy {Owner.Name} has been attacked and took {damage} damage, but has no behaviours to act on this event.", ConsoleMessageTypes.WARNING);
            }

            if (Health < 1)
            {
                Die();
            }
        }

        private async void Die()
        {
            await IOService.Output.WriteNonDialogueLine("The enemy has been defeated!");
        }

        public override CanBeAttackedBehaviour DeepClone()
        {
            return new CanBeAttackedBehaviour(MaxHealth) { Health = Health };
        }


        private record SaveData(double Health, double MaxHealth);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement((new SaveData(Health, MaxHealth))));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("CanBeAttackedBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise CanBeAttackedBehaviour save data.");
            Health = save.Health;
            MaxHealth = save.MaxHealth;
        }
    }
}
