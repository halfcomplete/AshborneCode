using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.MaskBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Constants;
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

            var ossaneth = CreateOssaneth(out var name);
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

        private static Item CreateOssaneth(out string name)
        {
            name = MaskNameConstants.Ossaneth;
            var ossaneth = CreateMask(name, "The Unblinking Eye", out var interjectionBehaviour);

            // TODO: Add various triggers for Ossaneth

            interjectionBehaviour.Register();

            return ossaneth;
        }
    }
}
