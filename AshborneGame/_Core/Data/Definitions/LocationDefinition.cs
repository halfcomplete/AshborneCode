using AshborneGame._Core.Data.BOCS;
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
        /// Unique runtime identifier for this specific location instance.
        /// </summary>
        public InstanceID InstanceID { get; init; }

        /// <summary>
        /// Immutable identifier of this definition.
        /// </summary>
        public DefinitionID DefinitionID { get; init; }

        /// <summary>
        /// The scene this location belongs to.
        /// </summary>
        public Scene Scene { get; init; }

        /// <summary>
        /// Flexible naming used for display and parser matching.
        /// </summary>
        public LocationNameAdapter Name { get; init; }

        /// <summary>
        /// Generates the dynamic environmental description for this location.
        /// </summary>
        public DescriptionComposer DescriptionComposer { get; init; }

        /// <summary>
        /// The parent location in the world hierarchy, or null if this is a root location.
        /// Parent/child relationships describe world organisation only and do not imply
        /// traversability.
        /// </summary>
        public DefinitionID? Parent { get; init; }

        /// <summary>
        /// Child locations contained within this location.
        /// </summary>
        public IReadOnlyList<DefinitionID> Children => _children;
        private readonly List<DefinitionID> _children = new();

        /// <summary>
        /// Runtime objects currently contained within this location.
        /// </summary>
        public IReadOnlyList<BOCSObjectDefinition> ContainedObjects => _containedObjects;
        private readonly List<BOCSObjectDefinition> _containedObjects = new();

        /// <summary>
        /// Traversable connections originating from this location.
        /// Every form of movement, including movement to child locations, is represented through an Exit.
        /// </summary>
        public IReadOnlyList<Exit> Exits => _exits;
        private readonly List<Exit> _exits = new();

        /// <summary>
        /// Custom commands available only while the player is in this location.
        /// </summary>
        public Dictionary<string, (Func<string> Message, Action Effect)> CustomCommands { get; init; } = new();

        public LocationDefinition(DefinitionID definitionID, LocationNameAdapter name, DescriptionComposer composer, List<DefinitionID> children, List<BOCSObjectDefinition> objects, List<Exit> exits, Dictionary<string, (Func<string> Message, Action Effect)> customCommands)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DescriptionComposer = composer ?? throw new ArgumentNullException(nameof(composer));

            _children = children;
            _containedObjects = objects;
            _exits = exits;
            DefinitionID = definitionID;
            CustomCommands = customCommands;
        }
    }
}
