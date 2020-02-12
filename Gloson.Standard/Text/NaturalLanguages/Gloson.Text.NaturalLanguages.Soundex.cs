using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Gloson.Text;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Soundex (English)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Soundex {
    #region Private Data

    private static Dictionary<char, int> s_Correspondence = new Dictionary<char, int>(CharacterComparer.OrdinalIgnoreCase) {
      { 'b', 1},
      { 'f', 1},
      { 'p', 1},
      { 'v', 1},

      { 'c', 2},
      { 'g', 2},
      { 'j', 2},
      { 'k', 2},
      { 'q', 2},
      { 's', 2},
      { 'x', 2},
      { 'z', 2},

      { 'd', 3},
      { 't', 3},

      { 'l', 4},

      { 'm', 5},
      { 'n', 5},

      { 'r', 6},
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Encode To Soundex
    /// </summary>
    /// <see cref="https://ru.wikipedia.org/wiki/Soundex"/>
    public static string Encode(string value) {
      if (string.IsNullOrEmpty(value))
        return value;

      int size = 4;

      StringBuilder sb = new StringBuilder();

      foreach (char x in value.Normalize(NormalizationForm.FormD)) {
        char c = char.ToUpper(x);

        if (c < 'A' || c > 'Z')
          continue;

        if (sb.Length == 0) {
          sb.Append(c);

          continue;
        }

        if (c == 'H' || c == 'W')
          continue;

        if (s_Correspondence.TryGetValue(c, out int v)) {
          if (sb[sb.Length - 1] == '0' + v)
            continue;

          sb.Append(v);
        }
        else if (sb[sb.Length - 1] != '0')
          sb.Append('0');

        if (sb.Length >= size * 2 + 1)
          break;
      }

      string result = sb.ToString().Replace("0", "").PadRight(size, '0');

      return result.Length > size
        ? result.Substring(0, size)
        : result;
    }

    #endregion Public
  }

}
