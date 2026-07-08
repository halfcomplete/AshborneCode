using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data.BOCSDTOs
{
    public sealed class BOCSObjectSaveData
    {
        public InstanceID InstanceId { get; set; }                // required, stable
        public DefinitionID? DefinitionId { get; set; }           // optional metadata only
        public ObjectNameSaveData Name { get; set; } = null!;        // serializable name adapter state
        public string Description { get; set; } = null!;
        public List<BehaviourSaveData> Behaviours { get; set; } = new();
    }
}
