using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;

namespace AshborneGame._Core.Data.Definitions.LocationDefinitions
{
    public record ParentChildDefinition(DefinitionID parent, DefinitionID child);
}