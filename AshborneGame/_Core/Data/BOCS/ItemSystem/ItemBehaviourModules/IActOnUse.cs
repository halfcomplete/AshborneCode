
using AshborneGame._Core._Player;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules
{
    /// <summary>
    /// A subscriber-type module that is implemented by Behaviours which need to know when their parent object is used in order to function.
    /// </summary>
    /// <remarks>Has a boolean property <c>ConsumeOnUse</c> to track whether the parent object should be destroyed when it's used.</remarks>
    // TODO: Generalise the property so that something can be partially damaged on use, or just this specific Behaviour is destroyed on use.
    public interface IActOnUse
    {
        bool ConsumeOnUse { get; set; }
        void OnUse(Player player);
    }
}
