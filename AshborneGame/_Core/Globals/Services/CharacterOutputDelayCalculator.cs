using AshborneGame._Core.Globals.Constants;

namespace AshborneGame._Core.Globals.Services
{
    public static class CharacterOutputDelayCalculator
    {
        private static Random random = new Random();
    public static async Task<int> CalculateDelayAsync(char letter, int ms, bool isEnd)
        {
            int randomVariation = random.Next(OutputConstants.RandomPauseMin, OutputConstants.RandomPauseMax + 1);
            int delay = 0;

            if (letter.Equals('.') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.FullStopPauseMultiplier;
            else if (letter.Equals('\u2014') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.EmDashPauseMultiplier;
            else if (letter.Equals(',') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.CommaPauseMultiplier;
            else if (letter.Equals('"') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.QuotationPauseMultiplier;
            else if (letter.Equals(':') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.QuotationPauseMultiplier;
            else if (letter.Equals(']') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.ClosingSquareBracketPauseMultiplier;
            else if (letter.Equals(')') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.ClosingParenthesisPauseMultiplier;
            else if (letter.Equals(':') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.ColonPauseMultiplier;
            else if (letter.Equals(';') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.SemicolonPauseMultiplier;
            else if (letter.Equals('?') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.QuestionMarkPauseMultiplier;
            else if (letter.Equals('!') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.ExclamationMarkPauseMultiplier;
            else if (letter.Equals('\n') && isEnd)
                delay = (ms + randomVariation) * OutputConstants.NewLinePauseMultiplier;
            else
                delay = (ms + randomVariation);

            // Pause typing if requested (only for typewriter output)
            while (OutputConstants.Paused)
            {
                await Task.Delay(50);
            }

            // Apply type speed multiplier
            delay = (int)(delay / OutputConstants.TypeSpeedMultiplier);
            return Math.Max(delay, 0);
        }

        public static async Task<int> CalculateDebugDelayAsync(char letter, int ms, bool isEnd)
        {
            int delay = await CalculateDelayAsync(letter, ms, isEnd);
            return Math.Max(Convert.ToInt32(Math.Round(delay * OutputConstants.DefaultDebugTypeSpeedModifier)), 0); // Ensure non-negative delay
        }
    }
}
