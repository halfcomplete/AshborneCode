using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.Definitions;

namespace AshborneTests
{
    internal static class TestBootstrap
    {
        internal static BOCSFactory CreateFactory()
        {
            var definitionRegistry = new DefinitionRegistry();
            var instanceRegistry = new InstanceRegistry();

            RegisterDefinitions(definitionRegistry);

            return new BOCSFactory(definitionRegistry, instanceRegistry);
        }

        private static void RegisterDefinitions(DefinitionRegistry registry)
        {
            registry.Register(TestDefinitions.TestNPC);
            registry.Register(TestDefinitions.TestTrader);
            registry.Register(TestDefinitions.TestSword);
            registry.Register(TestDefinitions.TestChest);
        }
    }
}