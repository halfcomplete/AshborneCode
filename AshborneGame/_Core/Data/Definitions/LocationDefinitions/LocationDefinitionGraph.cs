using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.Definitions.LocationDefinitions
{
    public static class LocationDefinitionGraph
    {
        public static IReadOnlyList<ExitDefinition> Exits =
        [
            new(DefinitionIDs.Locations.Dreamspace.EyePlatform, DefinitionIDs.Locations.Dreamspace.HallOfMirrors, DirectionConstants.East),
            new(DefinitionIDs.Locations.Dreamspace.EyePlatform, DefinitionIDs.Locations.Dreamspace.PlatformEdge, DirectionConstants.Forward),
        ];

        public static readonly IReadOnlyList<ParentChildDefinition> Hierarchy =
        [
            new(DefinitionIDs.Locations.Dreamspace.EyePlatform, DefinitionIDs.Locations.Dreamspace.PlatformEdge),
            new(DefinitionIDs.Locations.Dreamspace.HallOfMirrors, DefinitionIDs.Locations.Dreamspace.MirrorShardSublocation),
        ];
    }
}