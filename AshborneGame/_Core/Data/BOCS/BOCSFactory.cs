using AshborneGame._Core.Data.BOCS.ItemSystem.ItemBehaviourModules;
using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Globals.Enums;
using AshborneGame._Core.Globals.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AshborneGame._Core.Data.BOCS
{
    public class BOCSFactory
    {
        private IDefinitionRegistry _definitionRegistry;
        private IInstanceRegistry _instanceRegistry;

        public BOCSFactory(IDefinitionRegistry definitionRegistry, IInstanceRegistry instanceRegistry)
        {
            _definitionRegistry = definitionRegistry;
            _instanceRegistry = instanceRegistry;
        }

        /// <summary>
        /// Creates and returns a BOCSObject based on a given DefinitionID.
        /// </summary>
        /// <param name="definitionId"></param>
        /// <returns></returns>
        public BOCSObject Create(DefinitionID definitionId)
        {
            var definition = _definitionRegistry.Get<BOCSObjectDefinition>(definitionId);

            var gameObject = new BOCSObject(definition.Name, definition.Description, definitionId);

            var behavioursD = CreateBehaviours(definition.BehaviourPrototypes);

            foreach (var (btype, behaviours) in behavioursD)
            {
                foreach (var b in behaviours)
                {
                    gameObject.AddBehaviour(btype, b);
                }
            }

            _instanceRegistry.Register(gameObject);

            return gameObject;
        }

        public BOCSObject Clone(BOCSObject source)
        {
            var clone = new BOCSObject(source.Name, source.Description, source.DefinitionID, source.Synonyms);

            foreach (var (behaviourType, behaviours) in source.ByModule)
            {
                foreach (var behaviour in behaviours)
                {
                    // TODO: Add deep clone functionality to every behaviour (not here though!)
                    clone.AddBehaviour(behaviourType, behaviour);
                }
            }

            _instanceRegistry.Register(clone);

            return clone;
        }

        private Dictionary<Type, IReadOnlyList<Behaviour>> CreateBehaviours(IReadOnlyList<Behaviour> behaviours)
        {
            var result = new Dictionary<Type, List<Behaviour>>();

            foreach (var behaviour in behaviours)
            {
                var deepCloned = behaviour.DeepClone();
                foreach (var @interface in deepCloned.GetType().GetInterfaces())
                {
                    if (!result.TryGetValue(@interface, out var list))
                    {
                        list = new List<Behaviour>();
                        result[@interface] = list;
                    }

                    list.Add(deepCloned);
                }
            }

            //return result.ToDictionary(
            //    kvp => kvp.Key,
            //    kvp => (IReadOnlyList<Behaviour>)kvp.Value);
            // unsafe
            // TODO: maybe better way?
            return System.Runtime.CompilerServices.Unsafe.As<Dictionary<Type, IReadOnlyList<Behaviour>>>(result);
        }
    }
}