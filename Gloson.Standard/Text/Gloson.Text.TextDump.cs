using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String / Char dumps
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TextDump {
    #region Private Data

    private static readonly Dictionary<char, string> s_KnownCharacters = new Dictionary<char, string>() {
      { '\0', "\\0" },
      { '\r', "\\r" },
      { '\n', "\\n" },
      { '\t', "\\t" },
      { '\b', "\\b" },
      { '\v', "\\v" },
      { '\f', "\\f" },
      { '\\', "\\" },
    };

    #endregion Private Data

    #region Algorithm

    private static string CoreDump(char value) {
      if (s_KnownCharacters.TryGetValue(value, out string known))
        return known;

      if (value < ' ')
        return $"\\x{((int)value):x2}";

      if (value <= 0xFF)
        return value.ToString();

      if (char.IsWhiteSpace(value) || char.IsSurrogate(value) || char.IsSeparator(value))
        return $"\\u{((int)value):x4}";

      var category = char.GetUnicodeCategory(value);

      if (category == UnicodeCategory.PrivateUse)
        return $"\\u{((int)value):x4}";

      return value.ToString();
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Dump
    /// </summary>
    public static string Dump(char value) => value == '\''
      ? "'\\'"
      : $"'{CoreDump(value)}'";

    /// <summary>
    /// Dump Hex
    /// </summary>
    public static string DumpHex(char value) => $"\\u{((int)value):x4}";

    /// <summary>
    /// Dump
    /// </summary>
    public static string Dump(string value) {
      if (value is null)
        return "[null]";

      StringBuilder sb = new StringBuilder(value.Length + 256);

      sb.Append('"');

      foreach (char c in value)
        sb.Append(c == '"' ? "\\\"" : CoreDump(c));

      sb.Append('"');

      return sb.ToString();
    }

    /// <summary>
    /// Dump Hex
    /// </summary>
    public static string DumpHex(string value) => value is null
      ? "[null]"
      : string.Concat(value.Select(c => DumpHex(c)));

    #endregion Public
  }

}
