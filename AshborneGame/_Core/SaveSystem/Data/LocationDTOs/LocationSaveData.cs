using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data.LocationDTO
{
    public sealed class LocationSaveData
    {
        public DefinitionID DefinitionId { get; set; }

        public DefinitionID? ParentDefinitionId { get; set; }
        public List<DefinitionID> ChildDefinitionIds { get; set; } = new();

        public int VisitCount { get; set; }

        public List<InstanceID> ContainedObjectInstanceIds { get; set; } = new();
    }
}
