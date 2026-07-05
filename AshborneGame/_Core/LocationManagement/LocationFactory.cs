using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.LocationManagement
{
    public class LocationFactory
    {
        private ILocationRegistry _registry;

        public LocationFactory(ILocationRegistry registry)
        {
            _registry = registry;
        }

        public Location CreateLocation(Location location, LookDescription lookDescription, VisitDescription fading, SensoryDescription sensory, AmbientDescription? ambient = null, params ConditionalDescription[] conditionals)
        {
            var descriptionComposer = new DescriptionComposer(
                lookDescription,
                fading,
                sensory,
                ambient,
                conditionals .Where(c => c != null).ToArray()
            );
            location.SetDescriptionComposer(descriptionComposer);
            location.Name.SetParentLocation(location);

            _registry.RegisterLocation(location);
            
            return location;
        }

        /// <summary>
        /// Adds mutual exits between two locations in both directions.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <param name="direction">The direction from the first location to the second location.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the locations provided are null.</exception>
        public void AddMutualExits(Location location1, Location location2, string direction)
        {
            if (location1 == null || location2 == null)
                throw new ArgumentNullException("LocationFactory.cs: Locations provided for 'AddMutualExits.cs' cannot be null.");

            if (!location1.Exits.Any(e => e.Direction == direction) && !location2.Exits.Any(e => e.Direction == direction))
            {
                // TODO: support custom exit checking n stuff
                location1.AddExit(new Exit(location2, direction, () => true));
                location2.AddExit(new Exit(location1, DirectionConstants.CardinalDirectionOppositesMap[direction], () => true));
            }
            else
            {
                throw new InvalidOperationException($"LocationFactory.cs: Cannot add mutual exits to locations '{location1.Name.ReferenceName}' and '{location2.Name.ReferenceName}'. One or both locations already have an exit in that direction.");
            }
        }

        public Scene CreateScene(string sceneName, string sceneID, List<Location>? locations = null)
        {
            var scene = new Scene(sceneID, sceneName);
            if (locations != null)
            {
                foreach (var loc in locations)
                {
                    if (loc != null)
                    {
                        scene.AddLocation(loc); // sets loc.Scene correctly
                    }
                }
            }
            return scene;
        }
    }
}
