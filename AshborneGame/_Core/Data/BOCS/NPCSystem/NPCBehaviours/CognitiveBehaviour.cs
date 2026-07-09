using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Data.CognitionDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class CognitiveBehaviour : Behaviour
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
            return new CognitiveBehaviour(new PsychologicalState(Owner.DefinitionID));
        }


        private record SaveData(PsychologicalStateSaveData PsychologicalStateSaveData);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(PsychologicalState.GetSaveData())));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("CognitiveBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise CognitiveBehaviour save data.");
            PsychologicalState.LoadSaveData(save.PsychologicalStateSaveData);
        }
    }
}