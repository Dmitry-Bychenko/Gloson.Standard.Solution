using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StringExtensions {
    #region Private Data

    private static Dictionary<char, char> s_Pairs = new Dictionary<char, char>() {
      {'(', ')'},
      {')', '('},

      {'[', ']'},
      {']', '['},

      {'{', '}'},
      {'}', '{'},

      {'<', '>'},
      {'>', '<'},
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// Reversed Parentheses: ( -> ); ] -> [; <= -> =>; /* -> */ etc. 
    /// </summary>
    public static string ParenthesesReversed(this string value) {
      if (null == value)
        return value;

      StringBuilder sb = new StringBuilder(value.Length);

      for (int i = value.Length - 1; i >= 0; --i) {
        char c = value[i];

        sb.Append(s_Pairs.TryGetValue(c, out char v) ? v : c);
      }

      return sb.ToString();
    }

    #endregion Public
  }
}
