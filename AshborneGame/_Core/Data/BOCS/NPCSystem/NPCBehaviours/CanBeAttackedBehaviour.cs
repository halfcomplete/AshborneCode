using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    /// <summary>
    /// An NPC-specific Behaviour that allows the NPC to be attacked/damaged by the player or another hostile entity.
    /// <para>This Behaviour notifies other Behaviours that respond to attacks through its public <c>Attack(int)</c> method.</para>
    /// </summary>
    public class CanBeAttackedBehaviour : ICanBeAttacked, IAwareOfParentObject
    {
        public BOCSObject ParentObject { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }

        public CanBeAttackedBehaviour(BOCSObject parentObject, int maxHealth)
        {
            Health = MaxHealth = maxHealth;
            ParentObject = parentObject;
        }

        /// <summary>
        /// Reduces the health of the NPC by <c>damage</c> points. If <c>Health</c> is below 1, the NPC dies.
        /// </summary>
        /// <remarks>
        /// Additionally notifies all of the parent objects' Behaviours that implement the <c>IActOnAttacked</c> module.
        /// </remarks>
        public async void Attacked(float damage)
        {
            Health -= damage;

            if (ParentObject.GetAllBehaviours<IActOnAttacked>().Any())
            {
                await IOService.Output.DisplayDebugMessage($"The enemy {ParentObject.Name} has been attacked and took {damage} damage.", ConsoleMessageTypes.INFO);
                foreach (var behaviour in ParentObject.GetAllBehaviours<IActOnAttacked>())
                {
                    behaviour.OnAttacked();
                }
            }
            else
            {
                await IOService.Output.DisplayDebugMessage($"The enemy {ParentObject.Name} has been attacked and took {damage} damage, but has no behaviours to act on this event.", ConsoleMessageTypes.WARNING);
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
    }
}
