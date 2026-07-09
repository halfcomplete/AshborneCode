using AshborneGame._Core.Data.BOCS;
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
        public InstanceID InstanceID { get; set; }                // required, stable
        public DefinitionID DefinitionID { get; set; }           
        public ObjectNameSaveData Name { get; set; } = null!;        // serializable name adapter state
        public string Description { get; set; } = null!;
        public List<BehaviourSaveData> Behaviours { get; set; } = new();

        public BOCSObjectSaveData(InstanceID instanceID, DefinitionID definitionID, ObjectNameSaveData name, string description, List<BehaviourSaveData> behaviours)
        {
            InstanceID = instanceID;
            DefinitionID = definitionID;
            Name = name;
            Description = description;
            Behaviours = behaviours;
        }
    }
}
