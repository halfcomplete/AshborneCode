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
            public static readonly DefinitionID BoundOne = new DefinitionID("NPCs_BoundOne");
            public static readonly DefinitionID Dummy = new DefinitionID("NPCs_Dummy");
        }

        public static class Locations
        {
            public static class Dreamspace
            {
                public static readonly DefinitionID EyePlatform = new("Locations_Dreamspace_EyePlatform");
                public static readonly DefinitionID HallOfMirrors = new("Locations_Dreamspace_HallOfMirrors");
                public static readonly DefinitionID ChamberOfCycles = new("Locations_Dreamspace_ChamberOfCycles");
                public static readonly DefinitionID TempleOfTheBoundOne = new("Locations_Dreamspace_TempleOfTheBoundOne");
                public static readonly DefinitionID PlatformEdge = new("Locations_Dreamspace_PlatformEdge");
            }
        }

        public static class Items
        {
            public static class Masks
            {
                public static readonly DefinitionID Ossaneth = new("Items_Masks_Ossaneth");
            }

            public static class Magic
            {
                public static readonly DefinitionID MirrorShard = new("Items_Magic_MirrorShard");
            }
        }
    }
}