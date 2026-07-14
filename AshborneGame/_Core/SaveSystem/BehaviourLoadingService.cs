using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Combat;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.OtherBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.PlayerRelatedBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.UtilityBehaviours;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem
{
    public static class BehaviourLoadingService
    {
        public static IReadOnlyDictionary<string, Func<Behaviour>> BehaviourFactories = new Dictionary<string, Func<Behaviour>>()
        {
            ["cognitive"] = () => new CognitiveBehaviour(new(new())),
            ["talkable"] = () => new TalkableBehaviour(null),
            ["breakable"] = () => new BreakableBehaviour(),
            ["inspectable"] = () => new InspectableBehaviour(null, Globals.Enums.ItemQualities.None),
            ["equippable"] = () => new EquippableBehaviour(["face"]),
            ["usable"] = () => new UsableBehaviour(),
            ["storable"] = () => new StorableBehaviour(0, Globals.Enums.ItemTypes.None, Globals.Enums.ItemQualities.None),
            ["tradeableNPC"] = () => new TradeableNPCBehaviour(),

            ["maskInterjection"] = () => new MaskInterjectionBehaviour(null!),
            
            ["applyStatusEffectOnUse"] = () => new ApplyStatusEffectOnUseBehaviour(Globals.Enums.StatusEffectTypes.None),
            ["onUseChangePlayerStat"] = () => new OnUseChangePlayerStatBehaviour(0, Globals.Enums.PlayerStatType.NA, false),
            ["onEnemyUseDealDamage"] = () => new OnEnemyUseDealDamageBehaviour(0),
            ["onPlayerUseDealDamage"] = () => new OnPlayerUseDealDamageBehaviour(0),
            ["onUseLogMessage"] = () => new OnUseLogMessage(""),
            ["onEquipChangePlayerStat"] = () => new OnEquipChangePlayerStatBehaviour(Globals.Enums.PlayerStatType.NA, 0),
            ["onUseChangePlayerStat"] = () => new OnUseChangePlayerStatBehaviour(0, Globals.Enums.PlayerStatType.NA, false),
            ["onUseIncreaseVisibility"] = () => new OnUseIncreaseVisibilityBehaviour(0),
            ["onUseUnlockObject"] = () => new OnUseUnlockObjectBehaviour(null!),

            ["exitToNewLocation"] = () => new ExitToNewLocationBehaviour(null!),
            ["lockUnlock"] = () => new LockUnlockBehaviour(),
            ["openClose"] = () => new OpenCloseBehaviour(),
            ["container"] = () => new ContainerBehaviour(),
            ["describable"] = () => new DescribableBehaviour(),
        };

        public static Behaviour LoadFromSaveData(BehaviourSaveData saveData, SaveLoadContext context)
        {
            if (!BehaviourFactories.TryGetValue(saveData.BehaviourId, out var factory))
            {
                throw new InvalidDataException($"Behaviour ID '{saveData.BehaviourId}' is not in BehaviourFactories.");
            }

            Behaviour behaviour = factory();

            behaviour.LoadSaveData(saveData, context);

            return behaviour;
        }

        public static T LoadFromSaveData<T>(BehaviourSaveData saveData, SaveLoadContext context) where T : Behaviour, new()
        {
            T behaviour = new T();
            behaviour.LoadSaveData(saveData, context);
            return behaviour;
        }
    }
}
