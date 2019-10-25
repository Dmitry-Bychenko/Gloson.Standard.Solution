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

    #endregion Public
  }

}
