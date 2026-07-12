using AshborneGame._Core.CognitiveSystem.EmotionSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.Personality;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.CognitiveSystem.EmotionSystem.AttitudeSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data.CognitionDTOs
{
    public sealed class PsychologicalStateSaveData
    {
        public Dictionary<DefinitionID, Attitude> Relationships { get; set; } = new(); // DefinitionID keys
        public MemoryProfileSaveData Memory { get; set; } = null!;
        public PersonalityProfile Personality { get; set; } = null!;

        public PsychologicalStateSaveData(Dictionary<DefinitionID, Attitude> relationships, MemoryProfileSaveData memory, PersonalityProfile personality)
        {
            Relationships = relationships;
            Memory = memory;
            Personality = personality;
        }
    }
}
