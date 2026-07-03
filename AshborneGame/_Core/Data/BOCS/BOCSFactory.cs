using AshborneGame._Core.Data.Definitions;
using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

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

        public BOCSObject Create(DefinitionID definitionId)
        {
            var definition = _definitionRegistry.Get<GameObjectDefinition>(definitionId);

            var gameObject = new BOCSObject(definitionId);

            foreach (var behaviourDefinition in definition.Behaviours)
            {
                var behaviour = _behaviourFactory.Create(
                    behaviourDefinition,
                    gameObject);

                gameObject.AddBehaviour(behaviour);
            }

            _instanceRegistry.Register(gameObject);

            return gameObject;
        }

        public BOCSObject Clone(BOCSObject source)
        {
            var definition = _definitionRegistry.Get<GameObjectDefinition>(source.DefinitionID);

            var clone = new BOCSObject(source.DefinitionID);

            foreach (var (behaviourType, behaviours) in source.Behaviours)
            {
                foreach (var behaviour in behaviours)
                {
                    // TODO: Add deep clone functionality to every behaviour
                    clone.AddBehaviour(behaviourType, behaviour);
                }
            }

            _instanceRegistry.Register(clone);

            return clone;
        }
    }
}