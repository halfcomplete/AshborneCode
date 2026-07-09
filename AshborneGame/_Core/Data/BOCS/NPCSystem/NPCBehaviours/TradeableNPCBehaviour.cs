using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Inventory;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCCapabilities;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours
{
    public class TradeableNPCBehaviour : Behaviour, IHasInventory
    {
        public override string SaveId => "tradeable";

        public Inventory Inventory { get; private set; } = new Inventory();

        public bool IsTradeable { get; set; } = true;
        // TODO: implement varying inventory sizes

        // TODO: implement inventory deep clone (i think this is needed bcuz what if we need to save/load an npc with an inventory)
        public override TradeableNPCBehaviour DeepClone() => new();


        private record SaveData(InventorySaveData InventorySaveData, bool IsTradeable);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, System.Text.Json.JsonSerializer.SerializeToElement(new SaveData(Inventory.GetSaveData(), IsTradeable)));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("TradeableNPCBehaviour save data is missing state.");
            }
            SaveData save = System.Text.Json.JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise TradeableNPCBehaviour save data.");
            Inventory.LoadSaveData(save.InventorySaveData, context);
            IsTradeable = save.IsTradeable;
        }
    }
}
