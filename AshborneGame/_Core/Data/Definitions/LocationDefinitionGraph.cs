using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.Definitions
{
    public static class LocationDefinitionGraph
    {
        public static IReadOnlyList<ExitDefinition> Exits =
        [
            new(DefinitionIDs.Locations.Dreamspace.EyePlatform, DefinitionIDs.Locations.Dreamspace.HallOfMirrors, "east"),
        ];

        public static readonly IReadOnlyList<ParentChildDefinition> Hierarchy =
        [
            new(DefinitionIDs.Locations.Dreamspace.EyePlatform, DefinitionIDs.Locations.Dreamspace.PlatformEdge),
        ];
    }
}