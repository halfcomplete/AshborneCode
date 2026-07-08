using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.OtherBehaviours
{
    internal class OnUseLogMessage : Behaviour, IActOnUse
    {
        public bool ConsumeOnUse { get; set; } = false;
        private string message;

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


        private record SaveData(string Message, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(message, ConsumeOnUse)));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnUseLogMessage save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnUseLogMessage save data.");

            message = save.Message;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
