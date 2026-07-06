using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;

namespace AshborneGame._Core.Data.Definitions
{
    public static class SceneDefinitions
    {
        public static IReadOnlyDictionary<DefinitionID, SceneDefinition> Definitions = new Dictionary<DefinitionID, SceneDefinition>
        {
            { 
                DefinitionIDs.Scenes.OssanethsDomain, 
                new SceneDefinition(DefinitionIDs.Scenes.OssanethsDomain, "Ossaneth's Domain")
            },
            {
                DefinitionIDs.Scenes.Prologue,
                new SceneDefinition(DefinitionIDs.Scenes.Prologue, "Prologue")
            },
        };
    }
}