using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.Combat;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.ItemManagementBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.NotifierBehaviours;
using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviours.OtherBehaviours;
using AshborneGame._Core.Data.BOCS.NPCSystem.NPCBehaviours;
using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours;
using AshborneGame._Core.SaveSystem.Data.BOCSDTOs;
using AshborneGame._Core.SaveSystem.Serialisation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SaveSystem.Loading
{
    public static class BehaviourLoadingService
    {
        public static IReadOnlyDictionary<string, Type> BehaviourTypeMap = new Dictionary<string, Type>()
        {
            ["cognitive"] = typeof(CognitiveBehaviour),
            ["talkable"] = typeof(TalkableBehaviour),
            ["breakable"] = typeof(BreakableBehaviour),
            ["inspectable"] = typeof(InspectableBehaviour),
            ["equippable"] = typeof(EquippableBehaviour),
            ["usable"] = typeof(UsableBehaviour),
            ["maskInterjection"] = typeof(MaskInterjectionBehaviour),
            ["tradeableNPC"] = typeof(TradeableNPCBehaviour),
            ["container"] = typeof(ContainerBehaviour),
            ["describable"] = typeof(DescribableBehaviour),
            ["lockUnlock"] = typeof(LockUnlockBehaviour),
            ["openClose"] = typeof(OpenCloseBehaviour),
            ["applyStatusEffectOnUse"] = typeof(ApplyStatusEffectOnUseBehaviour),
        };

        public static Behaviour LoadFromSaveData(BehaviourSaveData saveData, SaveLoadContext context)
        {
            Type behaviourType = BehaviourTypeMap.GetValueOrDefault(saveData.BehaviourId) ?? throw new InvalidDataException($"Failed to find type {saveData.BehaviourId} for behaviour loading.");
            if (!typeof(Behaviour).IsAssignableFrom(behaviourType))
            {
                throw new InvalidDataException($"Type {saveData.BehaviourId} is not a subclass of Behaviour.");
            }
            Behaviour behaviour = (Behaviour)Activator.CreateInstance(behaviourType) ?? throw new InvalidDataException($"Failed to create instance of type {saveData.BehaviourId} for behaviour loading.");
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
