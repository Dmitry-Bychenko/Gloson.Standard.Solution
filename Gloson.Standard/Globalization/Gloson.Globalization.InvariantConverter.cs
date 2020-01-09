using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Numerics;
using System.Text;

namespace Gloson.Globalization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Invariant Culture Standard Converter (Parser)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class InvariantConverter {
    #region Public

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out byte result) {
      if (null == value) {
        result = byte.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return byte.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             byte.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    [CLSCompliant(false)]
    public static bool TryParse(string value, out sbyte result) {
      if (null == value) {
        result = sbyte.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return sbyte.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             sbyte.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out short result) {
      if (null == value) {
        result = short.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return short.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             short.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    [CLSCompliant(false)]
    public static bool TryParse(string value, out UInt16 result) {
      if (null == value) {
        result = UInt16.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return UInt16.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             UInt16.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out int result) {
      if (null == value) {
        result = int.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return int.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             int.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    [CLSCompliant(false)]
    public static bool TryParse(string value, out uint result) {
      if (null == value) {
        result = uint.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return uint.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             uint.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out long result) {
      if (null == value) {
        result = long.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return long.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             long.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    [CLSCompliant(false)]
    public static bool TryParse(string value, out ulong result) {
      if (null == value) {
        result = ulong.MinValue;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return ulong.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             ulong.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out float result) {
      if (null == value) {
        result = Single.NaN;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return Single.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out double result) {
      if (null == value) {
        result = double.NaN;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return double.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out BigInteger result) {
      if (null == value) {
        result = 0;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      return BigInteger.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             BigInteger.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out bool result) {
      if (null == value) {
        result = false;
        return false;
      }

      value = string.Concat(value.Where(c => !Char.IsWhiteSpace(c) && c != '_'));

      if (value.Equals("Y", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("Yes", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("T", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("TRUE", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("ON", StringComparison.OrdinalIgnoreCase) ||
          value == "1" || value == "+") {

        result = true;
        return true;
      }
      else if (value.Equals("N", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("No", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("F", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("False", StringComparison.OrdinalIgnoreCase) ||
          value.Equals("OFF", StringComparison.OrdinalIgnoreCase) ||
          value == "0" || value == "-") {

        result = false;
        return true;
      }

      result = false;
      return false;
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out DateTime result) {
      if (null == value) {
        result = DateTime.MinValue;

        return false;
      }

      // Dates only
      if (DateTime.TryParseExact(value, "d'.'M'.'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "M'/'d'/'yyyy", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        return true;

      // Date and Time
      if (DateTime.TryParseExact(value, "d'.'M'.'yyyy' 'H':'m", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "d'.'M'.'yyyy' 'H':'m':'s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "d'.'M'.'yyyy' 'H':'m':'s'.'F", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        return true;

      // Date and Time
      if (DateTime.TryParseExact(value, "M'/'d'/'yyyy' 'H':'m", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "M'/'d'/'yyyy' 'H':'m':'s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "M'/'d'/'yyyy' 'H':'m':'s'.'F", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        return true;

      // Date and Time
      if (DateTime.TryParseExact(value, "yyyy'-'M'-'d' 'H':'m", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d' 'H':'m':'s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d' 'H':'m':'s'.'F", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        return true;

      if (DateTime.TryParseExact(value, "yyyy'-'M'-'d'T'H':'m", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d'T'H':'m':'s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d'T'H':'m':'s'.'F", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        return true;

      if (DateTime.TryParseExact(value, "yyyy'-'M'-'d'T'H':'m'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d'T'H':'m':'s'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result) ||
          DateTime.TryParseExact(value, "yyyy'-'M'-'d'T'H':'m':'s'.'F'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
        return true;

      // Time

      if (DateTime.TryParseExact(value, "H':'m", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "H':'m':'s", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result) ||
          DateTime.TryParseExact(value, "H':'m':'s'.'F", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out result))
        return true;

      if (DateTime.TryParseExact(value, "H':'m'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result) ||
          DateTime.TryParseExact(value, "H':'m':'s'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result) ||
          DateTime.TryParseExact(value, "H':'m':'s'.'F'Z'", CultureInfo.InvariantCulture, DateTimeStyles.AssumeUniversal, out result))
        return true;

      result = DateTime.MinValue;

      return false;
    }

    #endregion Public
  }
}
