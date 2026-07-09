using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Data.CognitionDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class OnAttackedLogMessage : Behaviour, IActOnAttacked
    {
        public string Message { get; private set; }

        public OnAttackedLogMessage(string message)
        {
            Message = message ?? throw new ArgumentNullException(nameof(message), "Log message cannot be null.");
        }

        public async void OnAttacked()
        {
            await IOService.Output.WriteNonDialogueLine(Message);
        }

        public override OnAttackedLogMessage DeepClone()
        {
            return new(Message);
        }


        private record SaveData(string Message);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(Message)));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnAttackedLogMessage save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnAttackedLogMessage save data.");
            Message = save.Message;
        }
    }
}
