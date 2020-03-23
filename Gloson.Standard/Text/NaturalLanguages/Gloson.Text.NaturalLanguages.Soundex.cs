using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Soundex (English)
  /// </summary>
  /// https://habr.com/ru/post/114947/
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Soundex {
    #region Private Data

    private static readonly Dictionary<char, int> s_Correspondence = new Dictionary<char, int>(CharacterComparer.OrdinalIgnoreCase) {
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

    private static readonly Dictionary<char, int> s_CorrespondenceNext = new Dictionary<char, int>(CharacterComparer.OrdinalIgnoreCase) {
      { 'b', 1},
      { 'p', 1},

      { 'f', 2},
      { 'v', 2},

      { 'c', 3},
      { 'k', 3},
      { 's', 3},

      { 'g', 4},
      { 'j', 4},

      { 'q', 5},
      { 'x', 5},
      { 'z', 5},

      { 'd', 6},
      { 't', 6},

      { 'l', 7},

      { 'm', 8},
      { 'n', 8},

      { 'r', 9},
    };

    #endregion Private Data

    #region Algorithm

    private static string CoreEncode(string value,
                                     Dictionary<char, int> translation,
                                     int size,
                                     bool trim) {
      if (string.IsNullOrEmpty(value)) {
        if (trim)
          return new string('0', size);

        return "";
      }

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

        if (translation.TryGetValue(c, out int v)) {
          if (sb[sb.Length - 1] == '0' + v)
            continue;

          sb.Append(v);
        }
        else if (sb[sb.Length - 1] != '0')
          sb.Append('0');

        if (sb.Length >= size * 2 + 1)
          break;
      }

      string result = trim
        ? sb.ToString().Replace("0", "").PadRight(size, '0')
        : sb.ToString().PadRight(size, '0');

      return result.Length > size && trim
        ? result.Substring(0, size)
        : result;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Encode To Soundex
    /// </summary>
    /// <see cref="https://ru.wikipedia.org/wiki/Soundex"/>
    public static string Encode(string value) => CoreEncode(value, s_Correspondence, 4, true);

    /// <summary>
    /// Encode To Soundex (advanced version)
    /// </summary>
    public static string EncodeAdvanced(string value) => CoreEncode(value, s_CorrespondenceNext, 4, false);

    /// <summary>
    /// Encode To Soundex
    /// </summary>
    public static string Encode(string value, bool advanced, int size, bool trim) {
      if (size <= 0)
        throw new ArgumentOutOfRangeException(nameof(size));

      return CoreEncode(
        value,
        advanced ? s_CorrespondenceNext : s_Correspondence,
        size,
        trim);
    }

    #endregion Public
  }

}
