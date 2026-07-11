using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Data.BOCSDTOs
{
    public sealed class InventorySaveData
    {
        public List<InstanceID> Items { get; set; } = new();
    }
}
