using System.Diagnostics;

namespace AshborneGame._Core.Globals.Services
{
    public static class IntToRomanConversionService
    {
        public static string IntToRoman(int number)
        {
            if ((number < 0) || (number > 3999)) throw new ArgumentOutOfRangeException(nameof(number), "insert value between 1 and 3999");
            if (number < 1) return string.Empty;
            if (number >= 1000) return "M" + IntToRoman(number - 1000);
            if (number >= 900) return "CM" + IntToRoman(number - 900);
            if (number >= 500) return "D" + IntToRoman(number - 500);
            if (number >= 400) return "CD" + IntToRoman(number - 400);
            if (number >= 100) return "C" + IntToRoman(number - 100);
            if (number >= 90) return "XC" + IntToRoman(number - 90);
            if (number >= 50) return "L" + IntToRoman(number - 50);
            if (number >= 40) return "XL" + IntToRoman(number - 40);
            if (number >= 10) return "X" + IntToRoman(number - 10);
            if (number >= 9) return "IX" + IntToRoman(number - 9);
            if (number >= 5) return "V" + IntToRoman(number - 5);
            if (number >= 4) return "IV" + IntToRoman(number - 4);
            if (number >= 1) return "I" + IntToRoman(number - 1);
            throw new UnreachableException($"Impossible state reached in ToRoman(int number) at number {number}");
        }
    }
}
