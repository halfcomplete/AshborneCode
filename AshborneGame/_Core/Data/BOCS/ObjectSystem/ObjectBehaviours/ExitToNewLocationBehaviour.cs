using AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviourModules;
using AshborneGame._Core.LocationManagement;

namespace AshborneGame._Core.Data.BOCS.ObjectSystem.ObjectBehaviours
{
    internal class ExitToNewLocationBehaviour : IExit
    {
        public Location Location { get; set; }

        public ExitToNewLocationBehaviour(Location location)
        {
            Location = location;
        }
    }
}
