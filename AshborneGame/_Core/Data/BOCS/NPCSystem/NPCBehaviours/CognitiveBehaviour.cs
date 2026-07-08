using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class CognitiveBehaviour : Behaviour
    {
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
    }
}