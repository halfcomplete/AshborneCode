using AshborneGame._Core.Data.IDSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.LocationManagement
{
    public interface ILocationRegistry
    {
        void RegisterLocation(Location location);
        void RegisterScene(Scene scene);
        bool TryGetLocationByDefinitionID(DefinitionID definitionID, out Location? location);
        bool TryGetSceneByDefinitionID(DefinitionID definitionID, out Scene? scene);
        IReadOnlyList<DefinitionID> GetLocationIDs();
        IReadOnlyList<Location> GetLocations();
        IReadOnlyList<DefinitionID> GetSceneIDs();
        IReadOnlyList<Scene> GetScenes();
    }
}
