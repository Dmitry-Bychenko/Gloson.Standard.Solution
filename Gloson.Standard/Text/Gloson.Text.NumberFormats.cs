using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Number Formats Converter
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class NumberFormatsConverter {
    #region Private Data

    private static readonly Dictionary<char, int> s_Romans = new() {
      { 'M', 1000 },
      { 'D', 500 },
      { 'C', 100 },
      { 'L', 50 },
      { 'X', 10 },
      { 'V', 5 },
      { 'I', 1 },

      { 'Ⅰ', 1 },
      { 'Ⅱ', 2 },
      { 'Ⅲ', 3 },
      { 'Ⅳ', 4 },
      { 'Ⅴ', 5 },
      { 'Ⅵ', 6 },
      { 'Ⅶ', 7 },
      { 'Ⅷ', 8 },
      { 'Ⅸ', 9 },
      { 'Ⅹ', 10 },
      { 'Ⅺ', 11 },
      { 'Ⅻ', 12 },
      { 'Ⅼ', 50 },
      { 'Ⅽ', 100 },
      { 'Ⅾ', 500 },
      { 'Ⅿ', 1000 },
      { 'ⅰ', 1 },
      { 'ⅱ', 2 },
      { 'ⅲ', 3 },
      { 'ⅳ', 4 },
      { 'ⅴ', 5 },
      { 'ⅵ', 6 },
      { 'ⅶ', 7 },
      { 'ⅷ', 8 },
      { 'ⅸ', 9 },
      { 'ⅹ', 10 },
      { 'ⅺ', 11 },
      { 'ⅻ', 12 },
      { 'ⅼ', 50 },
      { 'ⅽ', 100 },
      { 'ⅾ', 500 },
      { 'ⅿ', 1000 },
      { 'ↀ', 1000 },
      { 'ↁ', 5000 },
      { 'ↂ', 10000 },
      { 'Ↄ', 100 },
      { 'ↄ', 100 },
      { 'ↅ', 6 },
      { 'ↆ', 50 },
      { 'ↇ', 50000 },
      { 'ↈ', 100000 },
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Try to Roman
    /// </summary>
    public static bool TryToRoman(int value, out string result) {
      result = null;

      if (value <= 0 || value >= 5000)
        return false;

      StringBuilder sb = new(32);

      if (value >= 1000)
        sb.Append(new string('M', value / 1000));

      value %= 1000;

      if (value >= 900) {
        value -= 900;
        sb.Append("CM");
      }

      if (value >= 500) {
        value -= 500;
        sb.Append('D');
      }

      if (value >= 400) {
        value -= 400;
        sb.Append("CD");
      }

      //---

      if (value >= 100)
        sb.Append(new string('C', value / 100));

      value %= 100;

      if (value >= 90) {
        value -= 90;
        sb.Append("XC");
      }

      if (value >= 50) {
        value -= 50;
        sb.Append('L');
      }

      if (value >= 40) {
        value -= 40;
        sb.Append("XL");
      }

      //---

      if (value >= 10)
        sb.Append(new string('X', value / 10));

      value %= 10;

      if (value == 9) {
        sb.Append("IX");

        value -= 9;
      }

      if (value >= 5) {
        value -= 5;
        sb.Append('V');
      }

      if (value == 4) {
        value -= 4;
        sb.Append("IV");
      }

      if (value > 0)
        sb.Append(new string('I', value));

      result = sb.ToString();

      return true;
    }

    /// <summary>
    /// To Roman
    /// </summary>
    public static String ToRoman(int value) {
      return TryToRoman(value, out string result)
        ? result
        : throw new ArgumentOutOfRangeException(nameof(value));
    }

    /// <summary>
    /// Try from Roman
    /// </summary>
    public static bool TryFromRoman(string value, out int result) {
      result = 0;

      int sum = 0;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      int count = 0;
      int last = int.MaxValue;

      foreach (char ch in value.Trim()) {
        if (!s_Romans.TryGetValue(char.ToUpperInvariant(ch), out int v))
          return false;

        if (v < last) {
          last = v;
          sum += v;
          count = 1;

          continue;
        }
        else if (v == last) {
          count += 1;

          if (v != 1 && v != 10 && v != 100 && v != 1000)
            return false;
          else if (count > 4)
            return false;
          else if (count == 4 && v != 1000 && v != 4)
            return false;

          sum += v;
        }
        else {
          if (count != 1)
            return false;
          else if ((last == 1 && (v == 5 || v == 10)) ||
                   (last == 10 && (v == 50 || v == 100)) ||
                   (last == 100 && (v == 500 || v == 1000))) {

            sum -= 2 * last;
            sum += v;

            last = v;
            count = 1;
          }
          else
            return false;
        }
      }

      result = sum;

      return true;
    }

    /// <summary>
    /// From Roman
    /// </summary>
    public static int FromRoman(string value) {
      return TryFromRoman(value, out int result)
        ? result
        : throw new FormatException($"{value} is not a valid Roman number");
    }

    #endregion Public
  }
}
