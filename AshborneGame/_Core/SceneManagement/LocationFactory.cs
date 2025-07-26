using AshborneGame._Core.Game.DescriptionHandling;
using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.SceneManagement
{
    public static class LocationFactory
    {
        public static Location CreateLocation(Location location, LookDescription lookDescription, FadingDescription fading, SensoryDescription sensory, AmbientDescription? ambient = null, ConditionalDescription? conditional = null)
        {
            var descriptionComposer = new DescriptionComposer(
                lookDescription,
                fading,
                sensory,
                ambient,
                conditional
            );

            location.SetDescriptionComposer(descriptionComposer);

            return location;
        }

        /// <summary>
        /// Adds mutual exits between two locations in both directions.
        /// </summary>
        /// <param name="location1">The first location.</param>
        /// <param name="location2">The second location.</param>
        /// <param name="direction">The direction from the first location to the second location.</param>
        /// <exception cref="ArgumentNullException">Thrown when any of the locations provided are null.</exception>
        public static void AddMutualExits(Location location1, Location location2, string direction)
        {
            if (location1 == null || location2 == null)
                throw new ArgumentNullException("LocationFactory.cs: Locations provided for 'AddMutualExits.cs' cannot be null.");

            if (!location1.Exits.ContainsKey(direction) && !location2.Exits.ContainsKey(direction))
            {
                location1.Exits.Add(direction, location2);
                location2.Exits.Add(DirectionConstants.CardinalDirectionOppositesMap[direction], location1);
            }
            else
            {
                throw new InvalidOperationException($"LocationFactory.cs: Cannot add mutual exits to locations '{location1.Name.ReferenceName}' and '{location2.Name.ReferenceName}'. One or both locations already have an exit in that direction.");
            }
        }

        public static Scene CreateScene(string sceneName, string sceneID, List<Location>? locations = null)
        {
            return new Scene(sceneID, sceneName, locations);
        }
    }
}
