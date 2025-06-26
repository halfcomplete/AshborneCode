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
    public class MaskInterjectionBehaviour : IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }

        public record MaskInterjectionTrigger
        (
            string EventType,
            Func<GameEvent, bool>? EventCondition,
            Func<GameStateManager, bool>? StateCondition,
            string Message,
            bool OneTime = false
        );

        private readonly List<MaskInterjectionTrigger> _triggers = new();
        private readonly HashSet<string> _firedOnce = new();
        private EventBus? _eventBus;
        private GameStateManager? _stateManager;

        public MaskInterjectionBehaviour(BOCSGameObject parentObject)
        {
            ParentObject = parentObject;
        }

        public void AddTrigger(MaskInterjectionTrigger trigger) => _triggers.Add(trigger);
        public void Register(EventBus eventBus, GameStateManager stateManager)
        {
            _eventBus = eventBus;
            _stateManager = stateManager;

            foreach (var trigger in _triggers)
            {
                eventBus.Subscribe(trigger.EventType, (e) =>
                {
                    if (ShouldTrigger(trigger, e))
                    {
                        IOService.Output.WriteLine($"{ParentObject.Name}: {trigger.Message}");
                    }
                });
            }
        }

        private bool ShouldTrigger(MaskInterjectionTrigger trigger, GameEvent evt)
        {
            if (trigger.OneTime && _firedOnce.Contains(trigger.EventType)) return false;

            bool passesEvent = trigger.EventCondition?.Invoke(evt) ?? true;
            bool passesState = trigger.StateCondition?.Invoke(_stateManager!) ?? true;

            return passesEvent && passesState;
        }
    }
}
