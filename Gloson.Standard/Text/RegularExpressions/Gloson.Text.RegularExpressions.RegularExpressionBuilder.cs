using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text.RegularExpressions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Regular Expression Builder
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class RegularExpressionBuilder {
    #region Algorithm

    private static string Multiplicator(string what, double from, double to) {
      if (to < from)
        return "";
      else if (to <= 0)
        return "";
      else if (string.IsNullOrEmpty(what))
        return "";

      if (from == to) {
        if (from == 1)
          return what;
        else
          return $"{what}{{{from}}}";
      }
      else if (from == 0) {
        if (to == 1)
          return $"{what}?";
        else if (double.IsPositiveInfinity(to))
          return $"{what}*";
      }
      else if (from == 1 && double.IsPositiveInfinity(to))
        return $"{what}+";

      return $"{what}{{{from},{to}}}";
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Pattern from wildcard
    /// </summary>
    /// <param name="value">wildcard</param>
    /// <param name="manyJoker">many joker, usually '*'</param>
    /// <param name="oneJoker">one joker, usually '?'</param>
    /// <param name="escape">escape symbol</param>
    /// <returns>Regular expression pattern</returns>
    public static string FromWildCardPattern(string value, char manyJoker, char oneJoker, char escape) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      StringBuilder sb = new StringBuilder();

      double from = 0;
      double to = 0;

      for (int i = 0; i < value.Length; ++i) {
        char c = value[i];

        if (c == escape) {
          if (from > 0 || to > 0)
            sb.Append(Multiplicator(".", from, to));

          from = 0;
          to = 0;

          i += 1;

          if (i < value.Length) {
            c = value[i];
            sb.Append(Regex.Escape(c.ToString()));
          }

          continue;
        }

        if (c == manyJoker)
          to = double.PositiveInfinity;
        else if (c == oneJoker) {
          from += 1;
          to += 1;
        }
        else {
          if (from > 0 || to > 0)
            sb.Append(Multiplicator(".", from, to));

          from = 0;
          to = 0;

          sb.Append(Regex.Escape(c.ToString()));
        }
      }

      sb.Append(Multiplicator(".", from, to));

      return sb.ToString();
    }

    /// <summary>
    /// Pattern from wildcard
    /// </summary>
    /// <param name="value">wildcard</param>
    /// <param name="manyJoker">many joker, usually '*'</param>
    /// <param name="oneJoker">many joker, usually '?'</param>
    /// <returns>Regular expression pattern</returns>
    public static string FromWildCardPattern(string value, char manyJoker, char oneJoker) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      StringBuilder sb = new StringBuilder();

      double from = 0;
      double to = 0;

      foreach (char c in value) {
        if (c == manyJoker) 
          to = double.PositiveInfinity;
        else if (c == oneJoker) {
          from += 1;
          to += 1;
        }
        else {
          if (from > 0 || to > 0)
            sb.Append(Multiplicator(".", from, to));

          from = 0;
          to = 0;

          sb.Append(Regex.Escape(c.ToString()));
        }
      }

      sb.Append(Multiplicator(".", from, to));

      return sb.ToString();
    }

    /// <summary>
    /// Pattern from wildcard
    /// </summary>
    /// <param name="value">wildcard</param>
    /// <returns>Regular expression pattern</returns>
    public static string FromWildCardPattern(string value) => FromWildCardPattern(value, '*', '?');

    /// <summary>
    /// Pattern from wildcard
    /// </summary>
    /// <param name="value">wildcard</param>
    /// <param name="manyJoker">many joker, usually '*'</param>
    /// <param name="oneJoker">one joker, usually '?'</param>
    /// <param name="escape">escape</param>
    /// <returns>Regular expression pattern</returns>
    public static string FromWildCardAnchoredPattern(string value, char manyJoker, char oneJoker, char escape) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      return $"^{FromWildCardPattern(value, manyJoker, oneJoker, escape)}$";
    }

    /// <summary>
    /// Pattern from wildcard
    /// </summary>
    /// <param name="value">wildcard</param>
    /// <param name="manyJoker">many joker, usually '*'</param>
    /// <param name="oneJoker">many joker, usually '?'</param>
    /// <returns>Regular expression pattern</returns>
    public static string FromWildCardAnchoredPattern(string value, char manyJoker, char oneJoker) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      return $"^{FromWildCardPattern(value, manyJoker, oneJoker)}$"; 
    }

    /// <summary>
    /// Pattern from wildcard
    /// </summary>
    /// <param name="value">wildcard</param>
    /// <returns>Regular expression pattern</returns>
    public static string FromWildCardAnchoredPattern(string value) => FromWildCardAnchoredPattern(value, '*', '?');

    /// <summary>
    /// If options pattern: abc[XY[Z]]def where abcdef, abcXYdef, abcXYZdef are valid is valid
    /// </summary>
    public static bool IsOptionsPatternValid(string value) {
      if (null == value)
        return false;

      int bracketsCount = 0;

      foreach (char ch in value) {
        if (ch == '[')
          bracketsCount += 1;
        else if (ch == ']') {
          if (--bracketsCount < 0)
            return false;
        }
      }

      return bracketsCount == 0;
    }

    /// <summary>
    /// From options pattern: abc[XY[Z]]def where abcdef, abcXYdef, abcXYZdef are valid
    /// </summary>
    /// <param name="value">value to convert to regex pattern</param>
    /// <returns></returns>
    public static string FromOptionsPattern(string value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      StringBuilder sb = new StringBuilder(value.Length * 2 + 8);

      int bracketsCount = 0;

      foreach (char ch in value) {
        if (ch == '[') {
          sb.Append("(?:");
          bracketsCount += 1;
        }
        else if (ch == ']') {
          sb.Append(")?");
          bracketsCount -= 1;

          if (bracketsCount < 0)
            throw new FormatException("Too many ']' symbols");
        }
        else
          sb.Append(Regex.Escape(ch.ToString()));
      }

      if (bracketsCount > 0)
        throw new FormatException("Too many '[' symbols");

      return sb.ToString();
    }

    /// <summary>
    /// From options pattern: abc[XY[Z]]def where abcdef, abcXYdef, abcXYZdef are valid
    /// </summary>
    /// <param name="value">value to convert to regex pattern</param>
    /// <returns></returns>
    public static string FromOptionsAnchoredPattern(string value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      return $"^{FromOptionsPattern(value)}$";
    }

    #endregion Public
  }

}
