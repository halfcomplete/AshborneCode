using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Data.CognitionDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class CognitiveBehaviour : Behaviour, ISentientEntity
    {
        public override string SaveId => "cognitive";

        public PsychologicalState PsychologicalState { get; init; }

        public CognitiveBehaviour(PsychologicalState psychologicalState)
        {
            PsychologicalState = psychologicalState;
        }

        public override Behaviour DeepClone()
        {
            // TODO: Figure out deep cloning method of psychological state
            var save = new SaveData(PsychologicalState.GetSaveData());
            var clone = new CognitiveBehaviour(new PsychologicalState(PsychologicalState.OwnerID));
            clone.LoadSaveData(new BehaviourSaveData(
                SaveId,
                JsonSerializer.SerializeToElement(save, CreateSaveDataJsonOptions())));
            clone.PsychologicalState.MemoryEmotion.SubscribeToEvents();
            return clone;
        }


        private record SaveData(PsychologicalStateSaveData PsychologicalStateSaveData);

        private static JsonSerializerOptions CreateSaveDataJsonOptions()
        {
            return new JsonSerializerOptions
            {
                Converters = { new DefinitionIDJsonConverter() }
            };
        }

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(
                SaveId,
                JsonSerializer.SerializeToElement(new SaveData(PsychologicalState.GetSaveData()), CreateSaveDataJsonOptions()));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context = null)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("CognitiveBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value, CreateSaveDataJsonOptions()) ?? throw new InvalidDataException("Failed to deserialise CognitiveBehaviour save data.");
            PsychologicalState.LoadSaveData(save.PsychologicalStateSaveData);
        }
    }
}