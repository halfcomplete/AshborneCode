using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Game;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours
{
    internal class OnUseIncreaseVisibilityBehaviour : Behaviour, IActOnUse
    {
        private int _visibilityIncreaseAmount = 3;
        public bool ConsumeOnUse { get; set; }
        public OnUseIncreaseVisibilityBehaviour(int visibilityIncrease, bool consumeOnUse = true)
        {
            _visibilityIncreaseAmount = visibilityIncrease;
            ConsumeOnUse = consumeOnUse;
        }
        public void OnUse(Player player)
        {
            player.SetVariable("visibility", player.Visibility + _visibilityIncreaseAmount);
        }

        public override OnUseIncreaseVisibilityBehaviour DeepClone()
        {
            return new OnUseIncreaseVisibilityBehaviour(_visibilityIncreaseAmount, ConsumeOnUse);
        }


        private record SaveData(int VisibilityIncreaseAmount, bool ConsumeOnUse);

        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, JsonSerializer.SerializeToElement(new SaveData(_visibilityIncreaseAmount, ConsumeOnUse)));
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("OnUseIncreaseVisibilityBehaviour save data is missing state.");
            }
            SaveData save = JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise OnUseIncreaseVisibilityBehaviour save data.");
            _visibilityIncreaseAmount = save.VisibilityIncreaseAmount;
            ConsumeOnUse = save.ConsumeOnUse;
        }
    }
}
