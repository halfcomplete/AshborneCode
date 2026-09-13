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
            new(DefinitionIDs.Locations.OssuaryOfEyes.WakingChamber, DefinitionIDs.Locations.OssuaryOfEyes.HallOfLostThoughts, DirectionConstants.East),
            new(DefinitionIDs.Locations.OssuaryOfEyes.WakingChamber, DefinitionIDs.Locations.OssuaryOfEyes.KeepersQuarters, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyes.KeepersQuarters, DefinitionIDs.Locations.OssuaryOfEyes.CloisterGardens, DirectionConstants.East),
            new(DefinitionIDs.Locations.OssuaryOfEyes.HallOfLostThoughts, DefinitionIDs.Locations.OssuaryOfEyes.CloisterGardens, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyes.CloisterGardens, DefinitionIDs.Locations.OssuaryOfEyes.CentralOssuary, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyes.CentralOssuary, DefinitionIDs.Locations.OssuaryOfEyes.Observatory, DirectionConstants.North),
            new(DefinitionIDs.Locations.OssuaryOfEyes.CentralOssuary, DefinitionIDs.Locations.OssuaryOfEyes.Scriptorium, DirectionConstants.East),
        ];

        public static readonly IReadOnlyList<ParentChildDefinition> Hierarchy =
        [
        ];
    }
}