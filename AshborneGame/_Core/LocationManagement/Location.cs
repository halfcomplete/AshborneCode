using System.Collections.Generic;
using System.Text;
using AshborneGame._Core._Player;
using AshborneGame._Core.Data.BOCS;
using AshborneGame._Core.Data.BOCS.ObjectSystem;
using AshborneGame._Core.Data.IDSystem;
using AshborneGame._Core.Game;
using AshborneGame._Core.Game.CommandHandling;
using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.Globals.Interfaces;
using AshborneGame._Core.Globals.Services;
using AshborneGame._Core.SaveSystem.Data.LocationDTO;

namespace AshborneGame._Core.LocationManagement
{
    /// <summary>
    /// Represents a navigable region of the game world.
    ///
    /// Locations form a recursive hierarchy describing the world's spatial
    /// structure. Every location has a unique compiletime identity
    /// (<see cref="DefinitionID"/>).
    ///
    /// Unlike <see cref="BOCSObject"/>, Locations are not composition-based.
    /// They are a specialised spatial structure responsible for:
    /// <list type="bullet">
    /// <item><description>World topology (parent/child relationships)</description></item>
    /// <item><description>Navigation (via <see cref="Exit"/> objects)</description></item>
    /// <item><description>Environmental descriptions</description></item>
    /// <item><description>Containing runtime <see cref="BOCSObject"/> instances</description></item>
    /// </list>
    /// </summary>
    public class Location
    {
        /// <summary>
        /// Immutable identifier of the definition this location was created from.
        /// </summary>
        public DefinitionID DefinitionID { get; }

        /// <summary>
        /// The scene this location belongs to.
        /// </summary>
        public Scene Scene { get; internal set; }

        /// <summary>
        /// Flexible naming used for display and parser matching.
        /// </summary>
        public LocationNameAdapter Name { get; }

        /// <summary>
        /// Generates the dynamic environmental description for this location.
        /// </summary>
        public DescriptionComposer DescriptionComposer { get; private set; }

        /// <summary>
        /// The parent location in the world hierarchy, or null if this is a root location.
        /// Parent/child relationships describe world organisation only and do not imply
        /// traversability.
        /// </summary>
        public Location? Parent { get; private set; }

        /// <summary>
        /// Child locations contained within this location.
        /// </summary>
        public IReadOnlyList<Location> Children => _children;
        private readonly List<Location> _children = new();

        /// <summary>
        /// Runtime objects currently contained within this location.
        /// </summary>
        public IReadOnlyList<BOCSObject> ContainedObjects => _containedObjects;
        private readonly List<BOCSObject> _containedObjects = new();

        /// <summary>
        /// Traversable connections originating from this location.
        /// Every form of movement, including movement to child locations, is represented through an Exit.
        /// </summary>
        public IReadOnlyList<Exit> Exits => _exits;
        private readonly List<Exit> _exits = new();

        /// <summary>
        /// Custom commands available only while the player is in this location.
        /// </summary>
        public CustomCommandHandler CustomCommands { get; } = new();

        /// <summary>
        /// Number of times the player has entered this location.
        /// </summary>
        public int VisitCount { get; internal set; }

        public Location(DefinitionID definitionID, LocationNameAdapter name, DescriptionComposer composer, List<Location> children, List<BOCSObject> objects, List<Exit> exits, Dictionary<string, (Func<string> Message, Action Effect)> customCommands)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            DescriptionComposer = composer ?? throw new ArgumentNullException(nameof(composer));
            _children = children;
            _containedObjects = objects;
            _exits = exits;
            DefinitionID = definitionID;
        }

        public Location(LocationNameAdapter name, DefinitionID definitionID)
            : this(definitionID, name, new DescriptionComposer(), new(), new(), new(), new())
        {
        }

        /// <summary>
        /// Adds a child location to this location.
        /// This establishes the spatial hierarchy only.
        /// Traversal should be added separately via an Exit.
        /// </summary>
        public void AddChild(Location child)
        {
            ArgumentNullException.ThrowIfNull(child);

            if (child.Parent != null)
                throw new InvalidOperationException(
                    $"Location '{child.Name.ReferenceName}' already has a parent.");

            child.Parent = this;
            _children.Add(child);
        }

        public void RemoveChild(Location child)
        {
            ArgumentNullException.ThrowIfNull(child);
            if (child.Parent != this)
                throw new InvalidOperationException(
                    $"Location '{child.Name.ReferenceName}' is not a child of '{Name.ReferenceName}'.");
            child.Parent = null;
            _children.Remove(child);
        }

        /// <summary>
        /// Adds a runtime object to this location.
        /// </summary>
        public void AddObject(BOCSObject obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            _containedObjects.Add(obj);
        }

        /// <summary>
        /// Removes a runtime object from this location.
        /// </summary>
        public bool RemoveObject(BOCSObject obj)
        {
            ArgumentNullException.ThrowIfNull(obj);

            return _containedObjects.Remove(obj);
        }

        /// <summary>
        /// Adds a traversable exit from this location.
        /// </summary>
        public bool AddExit(Exit exit)
        {
            ArgumentNullException.ThrowIfNull(exit);

            if (_exits.Any(e => e.Direction.Equals(exit.Direction, StringComparison.OrdinalIgnoreCase) && e.TargetLocation.Equals(exit.TargetLocation)))
            {
                return false;
            }

            _exits.Add(exit);

            return true;
        }

        /// <summary>
        /// Removes a custom command.
        /// </summary>
        public void RemoveCustomCommand(string command)
        {
            CustomCommands.RemoveCommand(command);
        }

        /// <summary>
        /// Replaces the DescriptionComposer used by this location.
        /// </summary>
        public void SetDescriptionComposer(DescriptionComposer composer)
        {
            DescriptionComposer = composer
                ?? throw new ArgumentNullException(nameof(composer));
        }

        /// <summary>
        /// Gets the full description, including the DescriptionComposer and exits, of this Location.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GetDescription(Player player, GameStateManager state)
        {
            var sb = new StringBuilder();

            sb.AppendLine(DescriptionComposer.GetDescription(player, state));
            sb.AppendLine(GetExits());

            return sb.ToString();
        }

        /// <summary>
        /// Gets only the DescriptionComposer's LookDescription.
        /// </summary>
        /// <param name="player"></param>
        /// <param name="state"></param>
        /// <returns></returns>
        public string GetLookDescription(Player player, GameStateManager state)
            => DescriptionComposer.GetLookDescription(player, state);


        public string GetExits()
        {
            var sb = new StringBuilder();
            List<DefinitionID> listedLocationIDs = new();
            if (Exits.Count == 0)
            {
                sb.Append("There are no exits from here.");
                if (Children.Count == 0) 
                { 
                    sb.Append(" There is also nothing of note here."); 
                } 
                else 
                { 
                    sb.Append(" However, you can go to:"); 
                }
            }
            else 
            { 
                sb.Append("From here you can go:\n"); 
                foreach (var exit in Exits) 
                { 
                    // TODO: fix?
                    if (!GameContext.LocationRegistry.TryGetLocationByDefinitionID(exit.TargetLocation, out var loc))
                    {
                        throw new KeyNotFoundException($"Definition ID {exit.TargetLocation} not found.");
                    }
                    sb.AppendLine($"- {exit.Direction} to {loc?.Name.DisplayName}"); 
                    listedLocationIDs.Add(exit.TargetLocation);
                }
            }

            if (Children.Count > 0 && !Children.All(child => listedLocationIDs.Contains(child.DefinitionID)))
            {
                sb.AppendLine("\nYou can also go to:");
            }

            foreach (var child in Children) 
            {
                if (listedLocationIDs.Contains(child.DefinitionID))
                {
                    continue;
                }
                sb.AppendLine($"- {child.Name.DisplayName}");
            }

            sb.Length -= Environment.NewLine.Length;

            return sb.ToString();
        }


        public LocationSaveData GetSaveData()
        {
            return new LocationSaveData
            {
                DefinitionId = DefinitionID,
                VisitCount = VisitCount,
                ChildDefinitionIds = Children.Select(c => c.DefinitionID).ToList(),
                ContainedObjectInstanceIds = ContainedObjects.Select(o => o.InstanceID).ToList(),
            };
        }
    }
}
