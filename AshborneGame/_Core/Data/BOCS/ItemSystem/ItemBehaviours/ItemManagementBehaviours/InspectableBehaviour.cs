using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours
{
    public class InspectableBehaviour : ItemBehaviourBase<InspectableBehaviour>, IInspectable, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }
        private string _baseDescription;
        private ItemQualities _rarity;
        private string? _inspectDesc;
        public bool IsInspected { get; private set; } = false;

        public InspectableBehaviour(BOCSGameObject parentObject, string baseDesc, ItemQualities rarity, string? inspectDesc)
        {
            ParentObject = parentObject ?? throw new ArgumentNullException(nameof(parentObject));
            _baseDescription = baseDesc;
            _rarity = rarity;
            _inspectDesc = inspectDesc;
        }

        public InspectableBehaviour(BOCSGameObject parentObject, string baseDesc, bool requiresIdentification = false)
        {
            ParentObject = parentObject ?? throw new ArgumentNullException(nameof(parentObject));
            _baseDescription = baseDesc;
            _rarity = ItemQualities.None;
        }

        public async Task Inspect()
        {
            if (IsInspected)
            {
                await IOService.Output.WriteNonDialogueLine("You have already inspected this item.");
                return;
            }

            // TODO: Implement logic to reveal hidden lore based on player actions or conditions, such as having a specific skill, item or quest completion.

            if (_inspectDesc == null)
            {
                await IOService.Output.WriteNonDialogueLine($"You inspect the {ParentObject.Name}; there is nothing interesting about it.");
                if (_rarity >= ItemQualities.Rare)
                {
                    await IOService.Output.WriteNonDialogueLine("However, it seems extremely valuable.");
                }
            }
            else
            {
                await IOService.Output.WriteNonDialogueLine(_inspectDesc);
                if (_rarity >= ItemQualities.Rare)
                {
                    await IOService.Output.WriteNonDialogueLine("It seems extremely valuable.");
                }
            }
            IsInspected = true;
        }

        public override InspectableBehaviour DeepClone()
        {
            return new InspectableBehaviour(ParentObject, _baseDescription, _rarity, _inspectDesc)
            {
                IsInspected = IsInspected,
            };
        }
    }
}
