using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Radix manipulation
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Radix {
    #region Private Data

    private static readonly Dictionary<char, int> s_RadixPrefix = new Dictionary<char, int>() {
      { 'b',  2},
      { 'B',  2},
      { 'o',  8},
      { 'O',  8},
      { 'd', 10},
      { 'D', 10},
      { 'x', 16},
      { 'X', 16},
      { 'z', 36},
      { 'Z', 36},
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Radix From Prefix
    /// </summary>
    public static int RadixFromPrefix(char value) {
      return s_RadixPrefix.TryGetValue(value, out int result)
        ? result
        : -1;
    }

    /// <summary>
    /// Radix From Prefix
    /// </summary>
    public static int RadixFromPrefix(string value) {
      if (null == value)
        return -1;
      else if (value.Length == 1)
        return RadixFromPrefix(value[0]);
      else if (value.StartsWith("0") && value.Length == 2)
        return RadixFromPrefix(value[1]);
      else if (value.StartsWith("-0") && value.Length == 3)
        return RadixFromPrefix(value[2]);
      else if (value.StartsWith("+0") && value.Length == 3)
        return RadixFromPrefix(value[2]);
      else
        return -1;
    }

    /// <summary>
    /// Character to digit
    /// </summary>
    public static int ToDigit(char value) {
      if (value >= '0' && value <= '9')
        return value - '0';
      else if (value >= 'a' && value <= 'z')
        return value - 'a' + 10;
      else if (value >= 'A' && value <= 'Z')
        return value - 'A' + 10;
      else
        return -1;
    }

    /// <summary>
    /// From digit to correponding character
    /// </summary>
    public static char FromDigit(int value) {
      if (value < 0 || value > 36)
        return '?';
      else if (value < 10)
        return (char)('0' + value);
      else
        return (char)('a' + value - 10);
    }

    /// <summary>
    /// From digit to correponding character
    /// </summary>
    public static char FromDigit(int value, bool upperCase) {
      if (value < 0 || value > 36)
        return '?';
      else if (value < 10)
        return (char)('0' + value);
      else
        return (char)((upperCase ? 'A' : 'a') + value - 10);
    }

    /// <summary>
    /// Is value a valid radix number representation
    /// </summary>
    public static bool IsValid(string value, int radix) {
      if (string.IsNullOrEmpty(value))
        return false;
      else if (radix <= 1 || radix > 36)
        return false;

      bool first = true;
      bool hasDigit = false;

      foreach (char c in value) {
        if (first) {
          first = false;

          if (c == '-' || c == '+')
            continue;
        }

        int v = ToDigit(c);

        if (v < 0 || v >= radix)
          return false;

        hasDigit = true;
      }

      return hasDigit;
    }

    /// <summary>
    /// Try Convert from radix to radix
    /// </summary>
    public static bool TryConvert(string value, int fromRadix, int toRadix, out string result) {
      result = null;

      if (string.IsNullOrEmpty(value))
        return false;
      else if (fromRadix <= 1 || fromRadix > 36)
        return false;
      else if (toRadix <= 1 || toRadix > 36)
        return false;

      if (fromRadix == toRadix) {
        if (IsValid(value, fromRadix)) {
          result = value;

          return true;
        }

        return false;
      }

      bool first = true;
      bool sign = false;
      bool hasDigit = false;

      BigInteger number = 0;

      for (int i = 0; i < value.Length; ++i) {
        char c = value[i];

        if (first) {
          first = false;

          if (c == '+')
            continue;
          else if (c == '-') {
            sign = true;

            continue;
          }
        }

        int digit = ToDigit(c);

        if (digit < 0 || digit >= fromRadix)
          return false;

        number = number * fromRadix + digit;
        hasDigit = true;
      }

      if (!hasDigit)
        return false;

      StringBuilder sb = new StringBuilder();

      for (; number > 0; number /= toRadix) {
        int v = (int)(number % toRadix);

        sb.Append(FromDigit(v));
      }

      if (sb.Length <= 0)
        sb.Append('0');
      else if (sign)
        sb.Append('-');

      sb.Reverse();

      result = sb.ToString();

      return true;
    }

    /// <summary>
    /// Convert from radix to radix
    /// </summary>
    public static String Convert(string value, int fromRadix, int toRadix) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));
      else if (string.IsNullOrEmpty(value))
        throw new FormatException("Empty String can't be converted");
      else if (fromRadix <= 1 || fromRadix > 36)
        throw new ArgumentOutOfRangeException(nameof(fromRadix));
      else if (toRadix <= 1 || toRadix > 36)
        throw new ArgumentOutOfRangeException(nameof(toRadix));

      if (TryConvert(value, fromRadix, toRadix, out string result))
        return result;
      else
        throw new FormatException("Invalid value format.");
    }

    /// <summary>
    /// Try Detect Mantissa and Radix
    /// </summary>
    public static bool TryDetect(string value, out string mantissa, out int radix) {
      mantissa = null;
      radix = -1;

      if (string.IsNullOrEmpty(value))
        return false;

      value = string.Concat(value.Where(c => !char.IsWhiteSpace(c) && c != '_'));

      if (IsValid(value, 10)) {
        mantissa = value.TrimStart('+');
        radix = 10;

        return true;
      }

      Match match = Regex.Match(value, @"(?:\-|\+)?0(?<digit>[bodxz])", RegexOptions.IgnoreCase);

      if (match.Success) {
        char r = char.ToUpper(match.Groups["digit"].Value[0]);
        radix = RadixFromPrefix(r);

        mantissa = value.Substring(match.Length);

        if (value[0] == '-')
          mantissa = "-" + mantissa;

        if (IsValid(mantissa, radix))
          return true;
      }
      else {
        match = Regex.Match(value, @"\((?<number>[0-9]+)\)");

        if (!match.Success)
          return false;

        if (!int.TryParse(match.Groups["number"].Value, NumberStyles.Any, CultureInfo.InvariantCulture, out radix)) {
          radix = -1;

          return false;
        }

        mantissa = value.Substring(0, value.Length - match.Length);

        if (IsValid(mantissa, radix))
          return true;
      }

      mantissa = null;
      radix = -1;

      return false;
    }

    /// <summary>
    /// Try Convert To Decimal (with radix autodetection)
    /// </summary>
    public static bool TryToDecimal(string value, out string result) {
      if (TryDetect(value, out string mantissa, out int radix))
        return TryConvert(mantissa, radix, 10, out result);
      else {
        result = null;

        return false;
      }
    }

    /// <summary>
    /// Convert To Decimal (with radix autodetection)
    /// </summary>
    public static String ToDecimal(string value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      if (TryToDecimal(value, out string result))
        return result;
      else
        throw new FormatException("Invalid format");
    }

    #endregion Public 
  }
}
