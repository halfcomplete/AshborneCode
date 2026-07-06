using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    public class DefinitionRegistrationService
    {
        public void RegisterAllDefinitions(DefinitionRegistry registry)
        {
            RegisterNPCDefinitions(registry);
            RegisterObjectDefinitions(registry);
            RegisterItemDefinitions(registry);
            RegisterLocationDefinitions(registry);
        }

        public void RegisterNPCDefinitions(DefinitionRegistry registry)
        {
            registry.Register(NPCDefinitions.BoundOne);
            registry.Register(NPCDefinitions.Dummy);
        }

        public void RegisterObjectDefinitions(DefinitionRegistry registry)
        {

        }

        public void RegisterItemDefinitions(DefinitionRegistry registry)
        {
            registry.Register(ItemDefinitions.MirrorShard);
        }

        public void RegisterLocationDefinitions(DefinitionRegistry registry)
        {
            foreach (var locationDefinition in LocationDefinitions.All)
            {
                registry.Register(locationDefinition);
            }
        }
    }
}
