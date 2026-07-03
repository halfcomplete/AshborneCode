using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    public sealed class GameObjectDefinition : Definition
    {
        public string Name { get; init; }

        public string Description { get; init; }

        public Dictionary<Type, List<object>> Behaviours { get; init; }
    }
}