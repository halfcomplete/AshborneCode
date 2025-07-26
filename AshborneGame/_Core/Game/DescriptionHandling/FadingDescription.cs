using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class FadingDescription
    {
        /// <summary>
        /// Description for the first time this is described.
        /// </summary>
        public string FirstTime { get; private set; }

        /// <summary>
        /// Description for the second time this is described.
        /// </summary>
        public string SecondTime { get; private set; }

        /// <summary>
        /// Description for the third to fourth times this is described.
        /// </summary>
        public string FourthTime { get; private set; }

        /// <summary>
        /// Description for the third, fifth and beyond times this is described. Optional.
        /// </summary>
        public string? UnchangedTime { get; private set; }

        public FadingDescription(
            string firstTime,
            string secondTime,
            string fourthTime,
            string? unchangedTime = null)
        {
            FirstTime = firstTime;
            SecondTime = secondTime;
            FourthTime = fourthTime;
            UnchangedTime = unchangedTime;
        }

        public FadingDescription()
        {
            FirstTime = "You enter a new place.";
            SecondTime = "You are here again.";
            FourthTime = "You have been here many times.";
            UnchangedTime = null; // Optional, can be set later
        }
    }
}
