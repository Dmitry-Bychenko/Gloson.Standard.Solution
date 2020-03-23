using System.Globalization;
using System.Text;

namespace Gloson.Text.NaturalLanguages.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Number To Words (Russian)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RuNumberToWords : INumberToWords {
    #region Private Data

    private static readonly (long value, string[] name)[] s_Powers = new (long value, string[] name)[] {
        ( 1_000_000_000_000_000_000, new [] { "квинтиллион", "квинтиллиона", "квинтиллионов" } ),
        (     1_000_000_000_000_000, new [] { "квадриллион", "квадриллиона", "квадриллионов" } ),
        (         1_000_000_000_000, new [] { "триллион",    "триллиона",    "триллионов"    } ),
        (             1_000_000_000, new [] { "миллиард",    "миллиарда",    "миллиардов"    } ),
        (                 1_000_000, new [] { "миллион",     "миллиона",     "миллионов"     } ),
        (                     1_000, new [] { "тысяча",      "тысячи",       "тысяч"         } ),
        (                         1, new [] { "",            "",             ""              } ),
      };

    private static readonly string[][] s_Digits = new string[][] {
      new string[] { "нуль" },
      new string[] { "один", "одна", "одно" },
      new string[] { "два", "две", "два" },
      new string[] { "три" },
      new string[] { "четыре" },
      new string[] { "пять" },
      new string[] { "шесть" },
      new string[] { "семь" },
      new string[] { "восемь" },
      new string[] { "девять" },

      new string[] { "десять" },
      new string[] { "одиннадцать" },
      new string[] { "двенадцать" },
      new string[] { "тринадцать" },
      new string[] { "четырнадцать" },
      new string[] { "пятнадцать" },
      new string[] { "шестнадцать" },
      new string[] { "семнадцать" },
      new string[] { "восемнадцать" },
      new string[] { "девятнадцать" },
    };

    private static readonly string[] s_Decimals = new string[] {
      "десять",
      "двадцать",
      "тридцать",
      "сорок",
      "пятьдесят",
      "шестьдесят",
      "семьдесят",
      "восемьдесят",
      "девяносто",
    };

    private static readonly string[] s_Hundreds = new string[] {
      "сто",
      "двести",
      "тристо",
      "четыресто",
      "пятьсот",
      "шестьсот",
      "семьсот",
      "восемьсот",
      "девятьсот",
    };

    private static readonly CultureInfo s_Culture = CultureInfo.GetCultureInfo("Ru");

    #endregion Private Data

    #region Algorithm

    private static string Short(int value, GrammaticalGender gender) {
      StringBuilder sb = new StringBuilder();

      if (value < 0)
        value = -value;

      if (value >= 100)
        sb.Append(s_Hundreds[value / 100 - 1]);

      value %= 100;

      if (value >= 20) {
        if (sb.Length > 0)
          sb.Append(' ');

        sb.Append(s_Decimals[value / 10 - 1]);

        value %= 10;
      }

      if (value > 0) {
        if (sb.Length > 0)
          sb.Append(' ');

        string[] nums = s_Digits[value];

        if (nums.Length <= 1)
          sb.Append(nums[0]);
        else {
          if (gender.HasFlag(GrammaticalGender.Feminine))
            sb.Append(nums[1]);
          else if (gender.HasFlag(GrammaticalGender.Nueter))
            sb.Append(nums[2]);
          else
            sb.Append(nums[0]);
        }
      }

      return sb.ToString();
    }

    #endregion Algorithm

    #region INumberToWords

    /// <summary>
    /// Number To Words
    /// </summary>
    public string NumberToNominative(long value, GrammaticalGender gender) {
      if (0 == value)
        return "нуль";

      StringBuilder sb = new StringBuilder();

      if (value < 0)
        sb.Append("минус");

      foreach (var tuple in s_Powers) {
        int v = (int)(value / tuple.value);
        value %= tuple.value;

        if (v != 0) {
          if (sb.Length > 0)
            sb.Append(' ');

          sb.Append(Short(v,
               tuple.value == 1000 ? GrammaticalGender.Feminine
             : tuple.value == 1 ? gender
             : GrammaticalGender.Masculine));

          if (tuple.value > 1) {
            sb.Append(' ');

            var number = GrammaticalNumberDetector.DetectNumber(v, s_Culture);

            if (number == GrammaticalNumber.Singular)
              sb.Append(tuple.name[0]);
            else if (number == GrammaticalNumber.Dual || number == GrammaticalNumber.Trial || number == GrammaticalNumber.Quadral)
              sb.Append(tuple.name[1]);
            else
              sb.Append(tuple.name[2]);
          }
        }
      }

      return sb.ToString();
    }

    #endregion INumberToWords
  }
}
