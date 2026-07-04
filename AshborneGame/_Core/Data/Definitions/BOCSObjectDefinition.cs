using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    public sealed class BOCSObjectDefinition : Definition
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public List<string> Synonyms { get; init; }

        public IReadOnlyList<Behaviour> BehaviourPrototypes { get; init; }

        public BOCSObjectDefinition(DefinitionID id, string name, string description, List<string> synonyms, IReadOnlyList<Behaviour> behaviourPrototypes)
        {
            ID = id;
            Name = name;
            Description = description;
            Synonyms = synonyms ?? new();
            BehaviourPrototypes = behaviourPrototypes;
        }
    }
}