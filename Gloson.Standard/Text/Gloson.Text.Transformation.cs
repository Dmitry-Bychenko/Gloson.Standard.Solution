using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Text Transformations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class TextTransformation {
    #region Public

    /// <summary>
    /// Compress Repeating Characters: 
    /// "AAAABCBBCDAEEE" -> "A4BCB2CDA3"
    /// </summary>
    public static string CompressRepeatingChars(string value) {
      if (string.IsNullOrEmpty(value))
        return value;

      StringBuilder sb = new StringBuilder(value.Length);
      int count = 0;
      char current = '\0';

      foreach (char c in value) {
        if (sb.Length <= 0 || current != c) {
          if (count > 1)
            sb.Append(count);

          sb.Append(c);

          count = 1;
          current = c;
        }
        else
          count += 1;
      }

      if (count > 1)
        sb.Append(count);

      return sb.ToString();
    }

    /// <summary>
    /// Decompress Repeating Characters: 
    /// "A4BCB2CDA3" -> "AAAABCBBCDAEEE" 
    /// </summary>
    public static string DecompressRepeatingChars(string value) {
      if (string.IsNullOrEmpty(value))
        return value;

      return Regex.Replace(
        value,
       "([^0-9])([0-9]+)",
        m => new string(m.Groups[1].Value[0], int.Parse(m.Groups[2].Value)));
    }

    #endregion Public
  }
}
