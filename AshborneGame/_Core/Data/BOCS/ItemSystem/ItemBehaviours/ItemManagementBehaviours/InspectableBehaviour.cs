using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours
{
    public class InspectableBehaviour : Behaviour, IInspectable
    {
        private ItemQualities _rarity;
        private string? _inspectDesc;
        public bool IsInspected { get; private set; } = false;

        public InspectableBehaviour(string? inspectDesc, ItemQualities rarity)
        {
            _rarity = rarity;
            _inspectDesc = inspectDesc;
        }

        public async Task Inspect()
        {
            // TODO: Implement logic to reveal hidden lore based on player actions or conditions, such as having a specific skill, item or quest completion.

            if (_inspectDesc == null)
            {
                await IOService.Output.WriteNonDialogueLine($"You inspect the {Owner.Name}; there is nothing interesting about it.");
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
            return new InspectableBehaviour(_inspectDesc, _rarity)
            {
                IsInspected = IsInspected,
            };
        }
    }
}
