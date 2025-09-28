using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours
{
    internal class OnEquipChangeEnvironmentStatBehaviour : ItemBehaviourBase<OnEquipChangeEnvironmentStatBehaviour>, IActOnEquip
    {
        string Message;
        public OnEquipChangeEnvironmentStatBehaviour(string message)
        {
            Message = message;
        }
        public async void OnEquip(Player player)
        {
            await IOService.Output.WriteNonDialogueLine(Message);
        }

        public async void OnUnequip(Player player)
        {
            await IOService.Output.WriteNonDialogueLine(Message);
        }

        public override OnEquipChangeEnvironmentStatBehaviour DeepClone()
        {
            return new OnEquipChangeEnvironmentStatBehaviour(Message);
        }
    }
}
