using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.SceneManagement
{
    public class LocationGroup
    {
        public string Name { get; init; }
        public List<Location> Locations { get; init; }

        public LocationGroup(string name, List<Location> locations)
        {
            Name = name;
            Locations = locations;
        }
    }
}
