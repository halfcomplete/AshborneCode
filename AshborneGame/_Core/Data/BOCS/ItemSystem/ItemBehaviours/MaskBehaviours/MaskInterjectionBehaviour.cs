using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules.MaskBehaviourModules;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.MaskBehaviours
{
    public class MaskInterjectionBehaviour : ItemBehaviourBase<MaskInterjectionBehaviour>, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }

        /// <summary>
        /// A trigger that can respond to strongly-typed game events.
        /// </summary>
        /// <typeparam name="TEvent">The event type to listen for.</typeparam>
        public record MaskInterjectionTrigger<TEvent>(
            Func<TEvent, bool>? EventCondition,
            Func<GameStateManager, bool>? StateCondition,
            string? Message,
            Func<Task>? Effect = null,
            bool OneTime = false
        ) where TEvent : IGameEvent;

        private interface ITriggerRegistration
        {
            EventToken Register(MaskInterjectionBehaviour behaviour);
        }

        private class TriggerRegistration<TEvent> : ITriggerRegistration where TEvent : IGameEvent
        {
            private readonly MaskInterjectionTrigger<TEvent> _trigger;
            
            public TriggerRegistration(MaskInterjectionTrigger<TEvent> trigger)
            {
                _trigger = trigger;
            }

            public EventToken Register(MaskInterjectionBehaviour behaviour)
            {
                return EventBus.SubscribeAsync<TEvent>(async (e) =>
                {
                    if (behaviour.ShouldTrigger(_trigger, e))
                    {
                        if (_trigger.Message != null)
                            await IOService.Output.WriteNonDialogueLine($"{behaviour.ParentObject.Name}: {_trigger.Message}");
                        if (_trigger.Effect != null)
                            await _trigger.Effect();
                    }
                });
            }
        }

        private readonly List<ITriggerRegistration> _triggerRegistrations = new();
        private readonly CompositeEventToken _subscriptionTokens = new();
        private GameStateManager _stateManager;

        public MaskInterjectionBehaviour(BOCSGameObject parentObject, GameStateManager stateManager)
        {
            ParentObject = parentObject;
            _stateManager = stateManager;
        }

        /// <summary>
        /// Adds a strongly-typed trigger for a specific event type.
        /// </summary>
        public void AddTrigger<TEvent>(MaskInterjectionTrigger<TEvent> trigger) where TEvent : IGameEvent
        {
            _triggerRegistrations.Add(new TriggerRegistration<TEvent>(trigger));
        }

        /// <summary>
        /// Registers all triggers with the EventBus.
        /// </summary>
        public Task Register()
        {
            foreach (var registration in _triggerRegistrations)
            {
                var token = registration.Register(this);
                _subscriptionTokens.Add(token);
            }
            return Task.CompletedTask;
        }

        /// <summary>
        /// Unregisters all triggers from the EventBus.
        /// </summary>
        public void Unregister()
        {
            _subscriptionTokens.Dispose();
        }

        private bool ShouldTrigger<TEvent>(MaskInterjectionTrigger<TEvent> trigger, TEvent evt) where TEvent : IGameEvent
        {
            bool passesEvent = trigger.EventCondition?.Invoke(evt) ?? true;
            bool passesState = trigger.StateCondition?.Invoke(_stateManager) ?? true;

            return passesEvent && passesState;
        }

        public override MaskInterjectionBehaviour DeepClone()
        {
            var clone = new MaskInterjectionBehaviour(ParentObject, _stateManager);
            foreach (var registration in _triggerRegistrations)
            {
                clone._triggerRegistrations.Add(registration);
            }
            return clone;
        }
    }
}
