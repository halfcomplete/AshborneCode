using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class CognitiveBehaviour : IAwareOfParentObject
    {
        public BOCSObject ParentObject { get; set; }

        public PsychologicalState PsychologicalState { get; init; }

        public CognitiveBehaviour(BOCSObject parent, PsychologicalState psychologicalState)
        {
            ParentObject = parent;
            PsychologicalState = psychologicalState;
        }
    }
}