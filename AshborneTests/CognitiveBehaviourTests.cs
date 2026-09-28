using AshborneGame._Core.Data.Definitions.LocationSpecific;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;

namespace AshborneTests;

[Collection("AshborneTests")]
public class CognitiveBehaviourTests
{
    [Fact]
    public void DeepClone_DoesNotRequireAnAttachedOwner()
    {
        var behaviour = Assert.IsType<CognitiveBehaviour>(
            Gardener.BehaviourPrototypes.Single());

        var clone = behaviour.DeepClone();

        Assert.IsType<CognitiveBehaviour>(clone);
        Assert.Null(behaviour.Owner);
    }
}
