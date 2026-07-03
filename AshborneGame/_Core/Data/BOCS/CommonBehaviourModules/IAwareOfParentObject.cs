using AshborneGame._Core.Data.BOCS.ItemSystem;

namespace AshborneGame._Core.Data.BOCS.CommonBehaviourModules
{
    /// <summary>
    /// A Behaviour module that is implemented by Behaviours which need a reference to their parent object to function.
    /// </summary>
    public interface IAwareOfParentObject
    {
        BOCSObject ParentObject { get; set; }
        
        public void SetParentObject(BOCSObject obj)
        {
            ParentObject = obj ?? throw new ArgumentNullException(nameof(obj), "Parent object cannot be null.");
        }
    }
}
