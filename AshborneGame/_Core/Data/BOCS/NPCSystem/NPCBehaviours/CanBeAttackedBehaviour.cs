﻿using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class CanBeAttackedBehaviour : ICanBeAttacked, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }
        public float Health { get; set; }
        public float MaxHealth { get; set; }

        public CanBeAttackedBehaviour(int maxHealth, BOCSGameObject parentObject)
        {
            Health = MaxHealth = maxHealth;
            ParentObject = parentObject;
        }

        public void Attacked(float damage)
        {
            Health -= damage;

            if (ParentObject.GetAllBehaviours<IActOnAttacked>().Any())
            {
                IOService.Output.DisplayDebugMessage($"The enemy {ParentObject.Name} has been attacked and took {damage} damage.", ConsoleMessageTypes.INFO);
                foreach (var behaviour in ParentObject.GetAllBehaviours<IActOnAttacked>())
                {
                    behaviour.OnAttacked();
                }
            }
            else
            {
                IOService.Output.DisplayDebugMessage($"The enemy {ParentObject.Name} has been attacked and took {damage} damage, but has no behaviours to act on this event.", ConsoleMessageTypes.WARNING);
            }

            if (Health < 1)
            {
                Die();
            }
        }

        private void Die()
        {
            IOService.Output.WriteLine("The enemy has been defeated!");
        }
    }
}
