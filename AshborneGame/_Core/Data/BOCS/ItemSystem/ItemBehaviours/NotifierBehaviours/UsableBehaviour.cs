using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemCapabilities;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System;
using System.Text.Json;

namespace AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours
{
    /// <summary>
    /// To intialise:
    /// 1. Create a new instance of this class with the default constructor: `new CompositeUseBehaviour()`.
    /// 2. Use the `Add` method to add behaviours that implement `IUsable` to this composite behaviour.
    /// </summary>
    public class UsableBehaviour : Behaviour, IUsable
    {
        public async void Use(Player player, string? target = null)
        {
            // Iterate through all behaviours that implement IActOnUse and call their Use method
            foreach (var behaviour in Owner.GetAllBehaviours<IActOnUse>())
            {
                await IOService.Output.DisplayDebugMessage($"Using behaviour {behaviour.GetType().Name} on item {Owner.Name}.", ConsoleMessageTypes.INFO);
                behaviour.OnUse(player);
            }
        }

        public override UsableBehaviour DeepClone()
        {
            return new UsableBehaviour();
        }


        public override BehaviourSaveData GetSaveState(SaveLoadContext context)
        {
            return new BehaviourSaveData(SaveId, null);
        }

        public override void LoadSaveState(BehaviourSaveData data, SaveLoadContext context)
        {
            // No state to load for this behaviour
        }
    }
}
