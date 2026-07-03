using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.LocationManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Data.Definitions
{
    public class LocationDefinition : Definition
    {
        /// <summary>
        /// The group this location belongs to, if any.
        /// </summary>
        public Scene Scene { get; set; }

        /// <summary>
        /// Flexible naming and parsing for the location.
        /// </summary>
        public LocationNameAdapter Name { get; }

        /// <summary>
        /// Composer class for managing descriptions.
        /// </summary>
        public DescriptionComposer DescriptionComposer { get; private set; }

        /// <summary>
        /// Sublocations within this location.
        /// </summary>
        public List<Sublocation> Sublocations { get; } = new();

        /// <summary>
        /// Dictionary of exits from this location. Keys are directions/keywords, values are Locations.
        /// </summary>
        public Dictionary<string, Location> Exits { get; } = new();

        /// <summary>
        /// Dictionary of custom commands for this location.
        /// </summary>
        public Dictionary<string, (Func<string> message, Action effect)> CustomCommands { get; } = new();

        public int VisitCount { get; set; } = 0;

        public LocationDefinition(LocationNameAdapter name, DescriptionComposer composer)
        {
            Name = name;
            DescriptionComposer = composer;
        }
    }
}
