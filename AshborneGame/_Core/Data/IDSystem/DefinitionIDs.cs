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
            public static readonly DefinitionID BoundOne = new DefinitionID("NPCs.BoundOne");
            public static readonly DefinitionID Dummy = new DefinitionID("NPCs.Dummy");
        }

        public static class Locations
        {
            public static class Dreamspace
            {
                public static readonly DefinitionID EyePlatform = new("Locations.Dreamspace.EyePlatform");
                public static readonly DefinitionID HallOfMirrors = new("Locations.Dreamspace.HallOfMirrors");
                public static readonly DefinitionID MirrorShardSublocation = new("Locations.Dreamspace.MirrorShardSublocation");
                public static readonly DefinitionID ChamberOfCycles = new("Locations.Dreamspace.ChamberOfCycles");
                public static readonly DefinitionID TempleOfTheBoundOne = new("Locations.Dreamspace.TempleOfTheBoundOne");
                public static readonly DefinitionID PlatformEdge = new("Locations.Dreamspace.PlatformEdge");
            }

            public static class Prologue
            {
                public static readonly DefinitionID PrologueStart = new("Locations.Prologue.PrologueStart");
            }
        }

        public static class Scenes
        {
            public static readonly DefinitionID OssanethsDomain = new("Locations.Scenes.OssanethsDomain");
            public static readonly DefinitionID Prologue = new("Locations.Scenes.Prologue");
        }

        public static class Items
        {
            public static class Masks
            {
                public static readonly DefinitionID Ossaneth = new("Items.Masks.Ossaneth");
            }

            public static class Magic
            {
                public static readonly DefinitionID MirrorShard = new("Items.Magic.MirrorShard");
            }
        }

        public static class Objects
        {
            public static class Dreamspace
            {
                public static readonly DefinitionID MirrorShard = new("Objects.Dreamspace.MirrorShard");
            }
        }
    }
}