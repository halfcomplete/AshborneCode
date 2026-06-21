
namespace AshborneGame._Core.Data.BOCS.ObjectSystem
{
    public class GameObject : BOCSGameObject
    {
        public override string Name { get; }
        public string Description { get; }

        public GameObject(string name, string description)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name), "Object name cannot be null.");
            Description = description;
        }
    }
}
