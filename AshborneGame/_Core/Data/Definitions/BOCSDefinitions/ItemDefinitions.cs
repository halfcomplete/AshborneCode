using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.OtherBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.Events;
using AshborneGame._Core.Globals.Enums;

namespace AshborneGame._Core.Data.Definitions.BOCSDefinitions
{
    public static class ItemDefinitions
    {
        // TODO: clarify what each 'description' format is meant to be
        // TODO: add various interjection triggers for Ossaneth
        public static BOCSObjectDefinition Ossaneth = 
            new(
                DefinitionIDs.Items.Masks.Ossaneth,
                new("Ossaneth", "a void-black mask", [], null),
                "A strange, void-black object of unknown origins",
                [
                    new StorableBehaviour(1, ItemTypes.Equipment, ItemQualities.Legendary),
                    new EquippableBehaviour(["face"]),
                    new InspectableBehaviour("The mask is heavy with history, power, and owners.", ItemQualities.Legendary),
                    new MaskInterjectionBehaviour(GameContext.GameState),
                ]
            );

        public static BOCSObjectDefinition MirrorShard =
            new(
                DefinitionIDs.Items.Magic.MirrorShard,
                new("Mirror Shard", "a shard from a mirror", ["shard", "mirror", "shard of mirror", "piece of mirror", "mirror piece"]),
                "A small, sharp mirror shard from Ossaneth's Domain.",
                [
                    new StorableBehaviour(1, ItemTypes.Consumable, ItemQualities.Rare),
                    new InspectableBehaviour("You hold the shard up to your face and feel a strange emptiness inside you. Light faintly glints off it as you turn it around in your hand.", ItemQualities.Rare),
                    new UsableBehaviour(),
                    new OnUseChangePlayerStatBehaviour(20, PlayerStatType.MaxHealth, true),
                ]
            );
    }
}