using AshborneGame._Core.Data.BOCS.ItemSystem;

namespace AshborneGame._Core.Data.BOCS.CommonBehaviourModules
{
    /// <summary>
    /// A Behaviour module that is implemented by Behaviours which need a reference to their parent object to function.
    /// </summary>
    public interface IAwareOfParentObject
    {
        BOCSGameObject ParentObject { get; set; }
        
        public void SetParentItem(Item parentItem)
        {
            ParentObject = parentItem ?? throw new ArgumentNullException(nameof(parentItem), "Parent item cannot be null.");
        }
    }
}
