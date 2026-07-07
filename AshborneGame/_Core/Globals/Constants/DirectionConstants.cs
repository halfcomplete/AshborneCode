using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AshborneGame._Core.Globals.Constants
{
    public static class DirectionConstants
    {
        public const string North = "north";
        public const string South = "south";
        public const string East = "east";
        public const string West = "west";

        public static IReadOnlyCollection<string> CardinalDirections =
        [
            North, South, East, West
        ];

        public static IReadOnlyDictionary<string, string> DirectionOppositesMap =
        new Dictionary<string, string>
        {
            { North, South },
            { South, North },
            { East, West },
            { West, East },
            { Forward, Back  },
            { Back, Forward },
            { Up, Down },
            { Down, Up },
            { Left, Right },
            { Right, Left },
            { In, Out },
            { Out, In }
        };

        public const string Back = "back";
        public const string Up = "up";
        public const string Down = "down";
        public const string Forward = "forward";
        public const string Left = "left";
        public const string Right = "right";

        public const string In = "in";
        public const string Out = "out";
        public const string Through = "through";

        public static IReadOnlyCollection<string> AllDirections =
        [
            North, South, East, West,
            Back, Up, Down, Forward, Left, Right,
            In, Out, Through
        ];
    }
}
