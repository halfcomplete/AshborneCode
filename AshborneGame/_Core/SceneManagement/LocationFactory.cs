using AshborneGame._Core.Game.DescriptionHandling;

namespace AshborneGame._Core.SceneManagement
{
    public static class LocationFactory
    {
        public static Location CreateLocation(Location location, string lookDescription, FadingDescription fading, SensoryDescription sensory, AmbientDescription? ambient = null, ConditionalDescription? conditional = null)
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
    }
}
