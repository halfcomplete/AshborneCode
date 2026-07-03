
using AshborneGame._Core.Data.IDSystem;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules
{
    public interface IUnlocksTarget
    {
        List<InstanceID> UnlockableObjectIDs { get; }
    }
}
