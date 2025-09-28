using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.OtherBehaviours
{
    internal class OnUseLogMessage : ItemBehaviourBase<OnUseLogMessage>, IActOnUse
    {
        public bool ConsumeOnUse { get; set; } = false;
        private readonly string message;

        public OnUseLogMessage(string message)
        {
            this.message = message;
        }

        public async void OnUse(Player player)
        {
            await IOService.Output.DisplayDebugMessage($"On Use Trigger successfully called, message: {message}", ConsoleMessageTypes.INFO);
        }

        public override OnUseLogMessage DeepClone()
        {
            return new OnUseLogMessage(message);
        }
    }
}
