using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text.RegularExpressions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Regular Escapement Mode
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum RegexEscapement {
    /// <summary>
    /// Complete ('-', ']' added)
    /// </summary>
    Complete = 0,
    /// <summary>
    /// Standard
    /// </summary>
    Standard = 1,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Regular Expression Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class RegexHelper {
    #region Private Data

    private static readonly HashSet<char> s_ExtraSymbols = new HashSet<char>() { '-', ']'};

    #endregion Private Data

    #region Public

    /// <summary>
    /// Escape
    /// </summary>
    public static string Escape(string value, RegexEscapement mode) {
      if (string.IsNullOrEmpty(value))
        return value;

      if (mode == RegexEscapement.Standard)
        return Regex.Escape(value);

      return string.Concat(value
        .Select(c => s_ExtraSymbols.Contains(c)
          ? "\\" + c.ToString()
          : Regex.Escape(c.ToString())));
    }

    /// <summary>
    /// Escape
    /// </summary>
    public static string Escape(string value) => Escape(value, RegexEscapement.Complete);

    #endregion Public
  }
}
