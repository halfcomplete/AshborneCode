using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectCapabilities;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.LocationManagement;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    internal class ExitToNewLocationBehaviour : Behaviour, IExit
    {
        public override string SaveId => "exitToNewLocation";

        public Location Location { get; set; }

        public ExitToNewLocationBehaviour(Location location)
        {
            Location = location;
        }

        public override ExitToNewLocationBehaviour DeepClone()
        {
            return new ExitToNewLocationBehaviour(Location);
        }

        private record SaveData(DefinitionID LocationId);

        public override BehaviourSaveData GetSaveData(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, System.Text.Json.JsonSerializer.SerializeToElement(new SaveData(Location.DefinitionID)));
        }

        public override void LoadSaveData(BehaviourSaveData data, SaveLoadContext context)
        {
            if (data.State.HasValue == false)
            {
                throw new InvalidDataException("ExitToNewLocationBehaviour save data is missing state.");
            }
            SaveData save = System.Text.Json.JsonSerializer.Deserialize<SaveData>(data.State.Value) ?? throw new InvalidDataException("Failed to deserialise ExitToNewLocationBehaviour save data.");
            context.LocationRegistry.TryGetLocationByDefinitionID(save.LocationId, out var location);
            if (location == null)
            {
                throw new InvalidDataException($"Failed to find location with ID {save.LocationId} when loading ExitToNewLocationBehaviour save data.");
            }
            Location = location;
        }
    }
}
