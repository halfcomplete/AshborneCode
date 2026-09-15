using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.Definitions.LocationSpecific
{
    public static class LocationDefinitionGraph
    {
        public static IReadOnlyList<ExitDefinition> Exits =
        [
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.WakingChamber, DefinitionIDs.Locations.OssuaryOfEyesLocs.HallOfLostThoughts, DirectionConstants.East),
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.WakingChamber, DefinitionIDs.Locations.OssuaryOfEyesLocs.KeepersQuarters, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.KeepersQuarters, DefinitionIDs.Locations.OssuaryOfEyesLocs.CloisterGardens, DirectionConstants.East),
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.HallOfLostThoughts, DefinitionIDs.Locations.OssuaryOfEyesLocs.CloisterGardens, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.CloisterGardens, DefinitionIDs.Locations.OssuaryOfEyesLocs.CentralOssuary, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.CentralOssuary, DefinitionIDs.Locations.OssuaryOfEyesLocs.Observatory, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyesLocs.CentralOssuary, DefinitionIDs.Locations.OssuaryOfEyesLocs.Scriptorium, DirectionConstants.East),
        ];

        public static readonly IReadOnlyList<ParentChildDefinition> Hierarchy =
        [
        ];
    }
}