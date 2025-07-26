using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Game.DescriptionHandling
{
    public class LookDescription
    {
        public string FirstLook { get; set; }
        public string SecondLook { get; set; }
        public string RepeatLook { get; set; }

        public int LookCount { get; set; } = 0;

        public LookDescription(string firstLook, string? secondLook = null, string? repeatLook = null)
        {
            FirstLook = firstLook;
            SecondLook = secondLook ?? "You look around once more. Nothing has changed.";
            RepeatLook = repeatLook ?? "You look around again, but everything remains the same.";
        }

        public LookDescription()
        {
            FirstLook = "You look around, taking in your surroundings.";
            SecondLook = "You look around again, but nothing has changed.";
            RepeatLook = "You glance around once more, but everything remains the same.";
        }
    }
}
