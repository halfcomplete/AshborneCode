using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.CommonBehaviourModules;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours
{
    /// <summary>
    /// To intialise:
    /// 1. Create a new instance of this class with the default constructor: `new CompositeUseBehaviour()`.
    /// 2. Use the `Add` method to add behaviours that implement `IUsable` to this composite behaviour.
    /// </summary>
    public class UsableBehaviour : ItemBehaviourBase<UsableBehaviour>, IUsable, IAwareOfParentObject
    {
        public BOCSGameObject ParentObject { get; set; }

        public UsableBehaviour(BOCSGameObject parentObject)
        {
            ParentObject = parentObject ?? throw new ArgumentNullException(nameof(parentObject), "Parent object cannot be null.");
        }

        public async void Use(Player player, string? target = null)
        {
            // Iterate through all behaviours that implement IActOnUse and call their Use method
            foreach (var behaviour in ParentObject.GetAllBehaviours<IActOnUse>())
            {
                await IOService.Output.DisplayDebugMessage($"Using behaviour {behaviour.GetType().Name} on item {ParentObject.Name}.", ConsoleMessageTypes.INFO);
                behaviour.OnUse(player);
            }
        }

        public override UsableBehaviour DeepClone()
        {
            return new UsableBehaviour(ParentObject);
        }
    }
}
