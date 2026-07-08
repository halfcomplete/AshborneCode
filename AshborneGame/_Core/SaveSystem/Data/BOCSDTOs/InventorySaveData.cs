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
        public List<InventorySlotSaveData> Slots { get; set; } = new();
    }
    public sealed class InventorySlotSaveData
    {
        public InstanceID ItemInstanceId { get; set; }
        public int Quantity { get; set; }
    }
}
