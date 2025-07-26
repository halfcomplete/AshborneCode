using AshborneGame._Core._Player;
using AshborneGame._Core.SceneManagement;
using System.Text;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class DescriptionComposer
    {
        public string LookDescription { get; init; }
        public FadingDescription Fading { get; private set; }
        public SensoryDescription Sensory { get; private set; }
        public AmbientDescription? Ambient { get; private set; }
        public ConditionalDescription? Conditional { get; private set; }

        public DescriptionComposer(
            string lookDescription,
            FadingDescription fading,
            SensoryDescription sensory,
            AmbientDescription? ambient = null,
            ConditionalDescription? conditional = null)
        {
            LookDescription = lookDescription;
            Fading = fading ?? throw new ArgumentNullException(nameof(fading), "Fading description cannot be null.");
            Sensory = sensory ?? throw new ArgumentNullException(nameof(sensory), "Sensory description cannot be null.");
            Ambient = ambient;
            Conditional = conditional;
        }

        public DescriptionComposer()
        {
            Fading = new FadingDescription();
            Sensory = new SensoryDescription();
        }

        public string GetDescription(Player player, GameStateManager gameState)
        {
            StringBuilder description = new StringBuilder();
            // Add fading descriptions based on visit count
            if (player.CurrentLocation.VisitCount == 0)
            {
                description.Append(Fading.FirstTime);
            }
            else if (player.CurrentLocation.VisitCount == 1)
            {
                description.Append(Fading.SecondTime);
            }
            else if (player.CurrentLocation.VisitCount < 5)
            {
                description.Append(Fading.FourthTime);
            }
            else if (Fading.UnchangedTime != null)
            {
                description.Append(Fading.UnchangedTime);
            }

            // Add ambient snippets if available
            if (Ambient != null)
            {
                description.Append(" " + Ambient.GetSnippetFromRandom());
            }

            // Add conditional snippets if available
            if (Conditional != null)
            {
                description.Append(" " + Conditional.GetDescription());
            }
            return description.ToString();
        }

        public string GetLookDescription()
        {
            if (string.IsNullOrEmpty(LookDescription))
            {
                return "There's nothing much to see here.";
            }

            return LookDescription;
        }
    }
}
