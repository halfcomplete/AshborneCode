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
            string EventName,
            Func<GameEvent, bool>? EventCondition,
            Func<GameStateManager, bool>? StateCondition,
            string? Message,
            Action? Effect = null,
            bool OneTime = false
        );

        private List<MaskInterjectionTrigger> _triggers = new();
        private GameStateManager _stateManager;

        public MaskInterjectionBehaviour(BOCSGameObject parentObject, GameStateManager stateManager)
        {
            ParentObject = parentObject;
            _stateManager = stateManager;
        }

        public void AddTrigger(MaskInterjectionTrigger trigger) => _triggers.Add(trigger);
        public void Register()
        {
            foreach (var trigger in _triggers)
            {
                EventBus.Subscribe(trigger.EventName, (e) =>
                {
                    var _triggers2 = new List<MaskInterjectionTrigger>(_triggers);
                    if (ShouldTrigger(trigger, e, out bool shouldDelete))
                    {
                        IOService.Output.WriteLine($"{ParentObject.Name}: {trigger.Message}");
                        if (shouldDelete) _triggers2.Remove(trigger);
                    }
                    _triggers = new List<MaskInterjectionTrigger>(_triggers2);
                });
            }
        }

        private bool ShouldTrigger(MaskInterjectionTrigger trigger, GameEvent evt, out bool shouldDelete)
        {
            shouldDelete = false;
            if (trigger.OneTime)
            {
                shouldDelete = true;
            }

            bool passesEvent = trigger.EventCondition?.Invoke(evt) ?? true;
            bool passesState = trigger.StateCondition?.Invoke(_stateManager) ?? true;

            return passesEvent && passesState;
        }
    }
}
