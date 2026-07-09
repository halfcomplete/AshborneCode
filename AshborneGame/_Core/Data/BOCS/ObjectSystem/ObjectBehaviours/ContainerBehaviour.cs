using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Game;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    public class ContainerBehaviour : Behaviour, IHasInventory
    {
        public override string SaveId => "container";

        public Inventory Inventory { get; private set; } = new Inventory();

        // TODO: deep clone inventory? and constructor?
        public override ContainerBehaviour DeepClone()
        {
            return new ContainerBehaviour();
        }


        private record SaveData(InventorySaveData InventorySaveData);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, System.Text.Json.JsonSerializer.SerializeToElement(new SaveData(Inventory.GetSaveData())));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("TradeableNPCBehaviour save data is missing state.");
            }
            SaveData save = System.Text.Json.JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise TradeableNPCBehaviour save data.");
            Inventory.LoadSaveData(save.InventorySaveData, context);
        }
    }
}
