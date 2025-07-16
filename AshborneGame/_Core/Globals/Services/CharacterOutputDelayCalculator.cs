using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.Globals.Services
{
    public static class CharacterOutputDelayCalculator
    {
        private static Random random = new Random();
        public static int CalculateDelay(char letter, int ms, bool isEnd)
        {
            int randomVariation = random.Next(OutputConstants.RandomPauseMin, OutputConstants.RandomPauseMax + 1);
            int delay = 0;

            if (letter.Equals('.') && isEnd) // If we reach a full stop and it's the last in the message / sentence.
            {
                delay = (ms + randomVariation) * OutputConstants.FullStopPauseMultiplier;
            }
            else if (letter.Equals('-') && isEnd) // If we reach a hyphen and it's the end of the message / sentence
            {
                delay = (ms + randomVariation) * OutputConstants.HyphenPauseMultiplier;
            }
            else if (letter.Equals(',') && isEnd) // If we reach a comma and it's the end of the message / sentence
            {
                delay = (ms + randomVariation) * OutputConstants.CommaPauseMultiplier;
            }
            else if (letter.Equals('"') && isEnd) // If we reach a quotation mark or colon and it's the end of the message / sentence
            {
                delay = (ms + randomVariation) * OutputConstants.QuotationPauseMultiplier;
            }
            else if (letter.Equals(':') && isEnd) // If we reach a colon and it's the end of the clause / sentence / line
            {
                delay = (ms + randomVariation) * OutputConstants.QuotationPauseMultiplier;
            }
            else if (letter.Equals(']') && isEnd) // If we reach a closing square bracket and it's the end of the message / sentence
            {
                delay = (ms + randomVariation) * OutputConstants.ClosingSquareBracketPauseMultiplier;
            }
            else
            {
                delay = (ms + randomVariation); // Default type speed
            }

            return Math.Max(delay, 0); // Ensure non-negative delay
        }

        public static int CalculateDebugDelay(char letter, int ms, bool isEnd)
        {
            int delay = CalculateDelay(letter, ms, isEnd);
            return Math.Max(Convert.ToInt32(Math.Round(delay * OutputConstants.DefaultDebugTypeSpeedModifier)), 0); // Ensure non-negative delay
        }
    }
}
