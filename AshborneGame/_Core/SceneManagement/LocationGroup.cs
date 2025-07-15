using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AshborneGame._Core.Globals.Interfaces;

namespace AshborneGame._Core.SceneManagement
{
    public class LocationGroup
    {
        public string Name { get; init; }
        public List<ILocation> Locations { get; init; }

        public LocationGroup(string name, List<ILocation> locations)
        {
            Name = name;
            Locations = locations;
        }
    }
}
