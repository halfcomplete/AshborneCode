using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions.BOCSDefinitions
{
    public sealed class BOCSObjectDefinition : Definition
    {
        public ObjectNameAdapter Name { get; init; }

        public string Description { get; init; }

        public IReadOnlyList<Behaviour> BehaviourPrototypes { get; init; }

        public BOCSObjectDefinition(DefinitionID id, ObjectNameAdapter name, string description, IReadOnlyList<Behaviour> behaviourPrototypes)
        {
            ID = id;
            Name = name;
            Description = description;
            BehaviourPrototypes = behaviourPrototypes;
        }
    }
}