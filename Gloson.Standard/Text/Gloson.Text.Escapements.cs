using System.Linq;
using System.Text;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StringExtensions {
    #region Public

    /// <summary>
    /// Escapement
    /// </summary>
    /// <param name="value">String To Escape characters in</param>
    /// <param name="escapement">Escapement character</param>
    /// <param name="needBeEscaped">Addition characters which should be escaped</param>
    /// <returns>Escaped string</returns>
    public static string Escape(this string value, char escapement, params char[] needBeEscaped) {
      if (string.IsNullOrEmpty(value))
        return value;

      StringBuilder sb = new StringBuilder(2 * value.Length);

      foreach (char c in value) {
        if (c == escapement)
          sb.Append(escapement);
        else if (needBeEscaped is not null && needBeEscaped.Contains(c))
          sb.Append(escapement);

        sb.Append(c);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Escapement by duplicating escaped characters
    /// </summary>
    /// <param name="value">String To Escape characters in</param>
    /// <param name="needBeDuplicated">Characters which should be duplicated</param>
    /// <returns>Escaped string</returns>
    public static string EscapeByDuplicating(this string value, params char[] needBeDuplicated) {
      if (string.IsNullOrEmpty(value))
        return value;

      StringBuilder sb = new StringBuilder(2 * value.Length);

      foreach (char c in value) {
        if (needBeDuplicated is not null && needBeDuplicated.Contains(c))
          sb.Append(c);

        sb.Append(c);
      }

      return sb.ToString();
    }

    #endregion Public
  }
}
