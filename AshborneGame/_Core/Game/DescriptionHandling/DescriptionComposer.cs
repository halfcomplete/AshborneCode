using AshborneGame._Core._Player;
using AshborneGame._Core.Globals.Constants;
using AshborneGame._Core.SceneManagement;
using System.ComponentModel.Design;
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
            
            // Determine which visit count to use based on whether player is in a sublocation or location
            int visitCount;
            if (player.CurrentSublocation != null)
            {
                // Player is in a sublocation, use sublocation visit count
                visitCount = player.CurrentSublocation.VisitCount;
            }
            else
            {
                // Player is in a location, use location visit count
                visitCount = player.CurrentLocation.VisitCount;
            }
            
            // Add fading descriptions based on visit count
            if (visitCount == 0)
            {
                description.Append(Fading.FirstTime);
            }
            else if (visitCount == 1)
            {
                description.Append(Fading.SecondTime);
            }
            else if (visitCount == 4)
            {
                description.Append(Fading.FourthTime);
            }
            else if (Fading.UnchangedTime != null)
            {
                description.Append(Fading.UnchangedTime);
            }
            else
            {
                // Use appropriate name based on whether player is in sublocation or location
                string displayName = player.CurrentSublocation?.Name.DisplayName ?? player.CurrentLocation.Name.DisplayName;
                description.Append($"You go back to {displayName}. It hasn't changed since you last came.");
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
            if (player.CurrentSublocation == null && player.CurrentLocation.Sublocations.Count > 0)
            {
                var names = player.CurrentLocation.Sublocations
                    .Select(s => s.ShortRefDesc);

                var joined = NaturalJoin(names);

                var random = new Random();
                int chance = random.Next(0, 5);
                if (chance == 0)
                {
                    description.Append(" You can also see ");
                }
                else if (chance == 1)
                {
                    description.Append(" You also notice ");
                }
                else if (chance == 2)
                {
                    description.Append(" Near you, you also see ");
                }
                else if (chance == 3)
                {
                    description.Append(" There is also ");
                }
                else
                {
                    description.Append(" Here there is also ");
                }
                    
                description.Append(joined);
                description.Append(".");
            }

            description.Append("\n");

            return description.ToString();
        }

        private string NaturalJoin(IEnumerable<string> items)
        {
            var list = items.ToList();
            if (list.Count == 0)
                return string.Empty;
            if (list.Count == 1)
                return list[0];
            if (list.Count == 2)
                return $"{list[0]} and {list[1]}";

            // 3+ items: "A, B, C, and D"
            var allButLast = string.Join(", ", list.Take(list.Count - 1));
            return $"{allButLast}, and {list.Last()}";
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

            description.Append("\n");

            return description.ToString();
        }
    }
}
