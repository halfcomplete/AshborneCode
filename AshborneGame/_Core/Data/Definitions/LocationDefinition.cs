using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
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
        public DefinitionID Scene { get; init; }

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
        public DefinitionID? Parent { get; private set; }

        /// <summary>
        /// Runtime objects currently contained within this location.
        /// </summary>
        public IReadOnlyList<DefinitionID> ContainedObjects => _containedObjects;
        private readonly List<DefinitionID> _containedObjects = new();

        /// <summary>
        /// Custom commands available only while the player is in this location.
        /// </summary>
        public Dictionary<string, (Func<string> Message, Action Effect)> CustomCommands { get; init; } = new();

        public LocationDefinition(
            DefinitionID definitionID,
            DefinitionID scene,
            LocationNameAdapter name,
            DescriptionComposer composer,
            List<DefinitionID>? objects,
            Dictionary<string, (Func<string> Message, Action Effect)>? customCommands)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Scene = scene;
            DescriptionComposer = composer ?? throw new ArgumentNullException(nameof(composer));

            _containedObjects = objects ?? new();
            DefinitionID = definitionID;
            ID = definitionID;
            CustomCommands = customCommands ?? new();
        }
    }
}
