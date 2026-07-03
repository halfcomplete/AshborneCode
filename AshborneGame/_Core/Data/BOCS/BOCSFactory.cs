using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.IDSystem;

namespace AshborneGame._Core.Data.BOCS
{
    public static class BOCSFactory
    {
        public BOCSGameObject Create(DefinitionID definitionId)
        {
            var definition = _definitionRegistry.Get<GameObjectDefinition>(definitionId);

            var gameObject = new BOCSGameObject(
                InstanceId.New(),
                definitionId);

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
    }
}