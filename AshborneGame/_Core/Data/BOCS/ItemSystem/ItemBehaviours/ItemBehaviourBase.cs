namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours
{
    public interface IItemBehaviourBase
    {
        object DeepClone();
    }

    public abstract class ItemBehaviourBase<T> : IItemBehaviourBase
    {
        public abstract T DeepClone();

        object IItemBehaviourBase.DeepClone() => DeepClone();
    }
}
