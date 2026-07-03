using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.IDSystem
{
    public static class DefinitionIDs
    {
        public static class NPCs
        {
            public static DefinitionID BoundOne = new DefinitionID("NPCs_BoundOne");
        }

        public static class Locations
        {
            public static class Dreamspace
            {
                public static DefinitionID EyePlatform = new("Locations_Dreamspace_EyePlatform");
                public static DefinitionID HallOfMirrors = new("Locations_Dreamspace_HallOfMirrors");
                public static DefinitionID ChamberOfCycles = new("Locations_Dreamspace_ChamberOfCycles");
                public static DefinitionID TempleOfTheBoundOne = new("Locations_Dreamspace_TempleOfTheBoundOne");
            }
        }
    }
}