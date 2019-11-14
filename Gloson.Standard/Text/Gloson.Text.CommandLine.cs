using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// CommandLine
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class CommandLine {
    #region Public

    /// <summary>
    /// Try Split Command Line into parameters
    /// </summary>
    public static bool TrySplit(string value, out string[] items) {
      items = null;

      if (null == value)
        return false;

      bool inQuotation = false;
      List<string> list = new List<string>();

      StringBuilder sb = new StringBuilder(value.Length);

      for (int i = 0; i < value.Length; ++i) {
        char c = value[i];

        if (inQuotation) {
          sb.Append(c);

          if (c == '"')
            inQuotation = false;
        }
        else if (char.IsWhiteSpace(c)) {
          if (sb.Length > 0)
            list.Add(sb.ToString());

          sb.Clear();
        }
        else if (c == '"') {
          sb.Append(c);
          inQuotation = true;
        }
        else
          sb.Append(c);
      }

      if (inQuotation) {
        items = null;

        return false;
      }

      if (sb.Length > 0)
        list.Add(sb.ToString());

      items = list.ToArray();

      return true;
    }

    /// <summary>
    /// Split Command Line into parameters
    /// </summary>
    public static string[] Split(string value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      if (TrySplit(value, out var result))
        return result;
      else
        throw new FormatException("Command line is in incorrect format");
    }

    #endregion Public
  }
}
