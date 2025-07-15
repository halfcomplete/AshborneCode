using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.MaskBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.BOCS.ItemSystem
{
    internal static class MaskInitialiser
    {
        public static Dictionary<string, Item> InitialiseMasks()
        {
            Dictionary<string, Item> masks = new Dictionary<string, Item>();

            var (name, ossaneth) = CreateOssaneth();
            masks.Add(name, ossaneth);

            return masks;
        }

        private static Item CreateMask(string name, string description, out MaskInterjectionBehaviour interjectionBehaviour)
        {
            var mask = new Item(name, description, "", 1, ItemTypes.Equipment, ItemQualities.Legendary);
            mask.AddBehaviour(typeof(IEquippable), new EquippableBehaviour(new List<string>() { "face" }));
            mask.AddBehaviour(typeof(IInspectable), new InspectableBehaviour(mask, mask.Description, mask.Quality, "This mask is heavy with history, power, and owners."));

            interjectionBehaviour = new MaskInterjectionBehaviour(mask, GameContext.GameState);
            mask.AddBehaviour(typeof(MaskInterjectionBehaviour), interjectionBehaviour);

            return mask;
        }

        private static (string, Item) CreateOssaneth()
        {
            string name = "Ossaneth";
            var ossaneth = CreateMask(name, "The Unblinking Eye", out var interjectionBehaviour);

            // Add various triggers and messages for Ossaneth
            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                EventName: "player.actions.prayed",
                EventCondition: evt => evt.Get<string>("location_group") == "Ossaneth's Domain",
                StateCondition: state => state.TryGetCounter("player.prayers", out var times) && times == 1,
                Message: "Pray without fear, child, for the Eye sees, and hears, all."
            ));

            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                EventName: "player.actions.prayed",
                EventCondition: evt => evt.Get<string>("location_group") == "Ossaneth's Domain",
                StateCondition: state => state.TryGetCounter("player.prayers", out var times) && times == 2,
                Message: "As I said, do not fear."
            ));

            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                EventName: "player.actions.prayed",
                EventCondition: evt => evt.Get<string>("location_group") == "Ossaneth's Domain",
                StateCondition: state => state.TryGetCounter("player.prayers", out var times) && times == 3,
                Message: "Again? How many times must you pray..."
            ));

            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                EventName: "player.actions.prayed",
                EventCondition: evt => evt.Get<string>("location_group") == "Ossaneth's Domain",
                StateCondition: state => state.TryGetCounter("player.prayers", out var times) && times == 4,
                Message: "Neither God nor I will answer to your prayers anymore.",
                OneTime: true
            ));

            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                  EventName: "player.actions.sat_on_throne",
                  EventCondition: evt => evt.Get<string>("location") == "the throne",
                  StateCondition: state => state.TryGetFlag("player.actions.sat_on_throne", out var value) && value,
                  Message: null,
                  Effect: async () => await GameContext.DialogueService.StartDialogue("Act1_Scene1_Throne_Dialogue"),
                  OneTime: true
            ));

            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                  EventName: "player.actions.touched_knife",
                  EventCondition: evt => evt.Get<string>("location") == "the pedestal",
                  StateCondition: state => state.TryGetFlag("player.actions.touched_knife", out var value) && value,
                  Message: null,
                  Effect: async () => await GameContext.DialogueService.StartDialogue("Act1_Scene1_Knife_Dialogue"),
                  OneTime: true
            ));

            interjectionBehaviour.AddTrigger(new MaskInterjectionBehaviour.MaskInterjectionTrigger
            (
                EventName: "player.actions.waited_once_at_mirror",
                EventCondition: evt => evt.Get<string>("location") == "the mirror",
                StateCondition: null,
                Message: "You wait too long... are you hoping something will change?",
                Effect: null,
                OneTime: true
            ));

            GameContext.GameState.AddLocationTimeTrigger(new LocationTimeTrigger
            (
                locationName: "the mirror",
                duration: TimeSpan.FromSeconds(20),
                eventToRaise: new GameEvent("player.actions.waited_once_at_mirror", new() {
                    { "location", "the mirror" }
                }),
                effect: null,
                oneTime: true
            ));

            interjectionBehaviour.Register();

            return (name, ossaneth);
        }
    }
}
