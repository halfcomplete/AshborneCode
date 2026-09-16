using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;

namespace AshborneGame._Core.Data.Definitions.LocationSpecific
{
    public static class SceneDefinitions
    {
        public static IReadOnlyDictionary<DefinitionID, SceneDefinition> Definitions = new Dictionary<DefinitionID, SceneDefinition>
        {
            { 
                DefinitionIDs.Scenes.OssuaryOfEyes, 
                new SceneDefinition(DefinitionIDs.Scenes.OssuaryOfEyes, "Ossuary of Eyes")
            },
            {
                DefinitionIDs.Scenes.Prologue,
                new SceneDefinition(DefinitionIDs.Scenes.Prologue, "Prologue")
            },
        };
    }
}