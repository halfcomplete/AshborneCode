using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.SceneManagement;
using System.Linq;
using System.Text;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class DescriptionComposer
    {
        public LookDescription Look { get; private set; }
        public FadingDescription Fading { get; private set; }
        public SensoryDescription Sensory { get; private set; }
        public AmbientDescription? Ambient { get; private set; }
        public ConditionalDescription? Conditional { get; private set; }

        private readonly Dictionary<string, string> positionalPhrases = new()
        {
            { OutputConstants.ShortenedCentre, "In the centre" },
            { OutputConstants.ShortenedRight, "On the right" },
            { OutputConstants.ShortenedLeft, "On the left" },
            { OutputConstants.ShortenedAtTop, "At the top" },
            { OutputConstants.ShortenedAtBottom, "At the bottom" },
            { OutputConstants.ShortenedAtFront, "At the front" },
            { OutputConstants.ShortenedAtBack, "At the back" },
            { OutputConstants.ShortenedAtMiddle, "At the middle" },
            { OutputConstants.ShortenedOnTop, "On the top" },
            { OutputConstants.ShortenedOnBottom, "On the bottom" },
            { OutputConstants.ShortenedInFront, "In the front" },
            { OutputConstants.ShortenedInBack, "In the back" },
            { OutputConstants.ShortenedInMiddle, "In the middle" },
            { OutputConstants.ShortenedBehind, "Behind" },
            { OutputConstants.ShortenedAbove, "Above" },
            { OutputConstants.ShortenedBelow, "Below" }
        };

        public DescriptionComposer(
            LookDescription lookDescription,
            FadingDescription fading,
            SensoryDescription sensory,
            AmbientDescription? ambient = null,
            ConditionalDescription? conditional = null)
        {
            Look = lookDescription;
            Fading = fading ?? throw new ArgumentNullException(nameof(fading), "Fading description cannot be null.");
            Sensory = sensory ?? throw new ArgumentNullException(nameof(sensory), "Sensory description cannot be null.");
            Ambient = ambient;
            Conditional = conditional;
        }

        public DescriptionComposer()
        {
            Look = new LookDescription();
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
            else if (player.CurrentLocation.VisitCount == 4)
            {
                description.Append(Fading.FourthTime);
            }
            else if (Fading.UnchangedTime != null)
            {
                description.Append(Fading.UnchangedTime);
            }
            else
            {
                description.Append($"You go back to {player.CurrentLocation.Name.DisplayName}. It hasn't changed since you last came.");
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

            // Add sublocation snippets if available and not currently in a sublocation
            if (player.CurrentSublocation == null)
            {
                foreach (var sublocation in player.CurrentLocation.Sublocations)
                {
                    description.Append(" " + positionalPhrases[sublocation.ShortenedPositionalPhrase] + " is " + sublocation.ShortRefDesc + ".");
                }
            }

            return description.ToString();
        }

        public string GetLookDescription(Player player, GameStateManager gameState)
        {
            StringBuilder description = new StringBuilder();
            if (Look.LookCount == 0)
            {
                description.Append(Look.FirstLook);
            }
            else if (Look.LookCount == 1)
            {
                description.Append(Look.SecondLook);
            }
            else if (Look.LookCount >= 2)
            {
                description.Append(Look.RepeatLook);
            }

            // Add conditional snippets if available
            if (Conditional != null)
            {
                description.Append(" " + Conditional.GetDescription());
            }

            // Increment the look count
            Look.LookCount++;

            return description.ToString();
        }
    }
}
