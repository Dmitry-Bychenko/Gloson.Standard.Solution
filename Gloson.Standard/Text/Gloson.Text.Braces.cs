using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Extensions (Braces)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StringBracesExtensions {
    #region Private Data

    private static readonly Dictionary<char, char> s_Pairs;

    #endregion Private Data

    #region Create

    static StringBracesExtensions() {
      s_Pairs = new Dictionary<char, char>();

      Stack<char> opens = new Stack<char>();

      for (char c = char.MinValue; c < char.MaxValue - 1; ++c) {
        var category = char.GetUnicodeCategory(c);

        if (category == UnicodeCategory.OpenPunctuation)
          opens.Push(c);
        else if (category == UnicodeCategory.ClosePunctuation) {
          char open = opens.Pop();

          s_Pairs.Add(open, c);
          s_Pairs.Add(c, open);
        }
      }

      s_Pairs.Add('<', '>');
      s_Pairs.Add('>', '<');
    }

    #endregion Create

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

    /// <summary>
    /// Parentheses Kind: -1 - open; 0 - nothing; +1 close
    /// </summary>
    public static int ParenthesesKind(this string value) {
      if (string.IsNullOrEmpty(value))
        return 0;

      int result = 0;

      foreach (char c in value) {
        var category = char.GetUnicodeCategory(c);

        int v = 
               c == '<' || category == UnicodeCategory.OpenPunctuation  ? -1 
             : c == '>' || category == UnicodeCategory.ClosePunctuation ? +1 
             : 0;

        if (v == 0)
          continue;

        if (result == 0)
          result = v;
        else if (result != v)
          return 0;
      }

      return result;
    }

    /// <summary>
    /// Valid Parentheses
    /// </summary>
    public static bool ParenthesesValid(string value) {
      if (string.IsNullOrEmpty(value))
        return true;

      Stack<char> opened = new Stack<char>();

      foreach (var c in value) {
        var category = char.GetUnicodeCategory(c);

        if (category == UnicodeCategory.OpenPunctuation)
          opened.Push(c);
        else if (category == UnicodeCategory.ClosePunctuation) 
          if (opened.Count <= 0)
            return false;
          else if (s_Pairs[c] != opened.Pop())
            return false;
      }

      return opened.Count <= 0;
    }

    #endregion Public
  }
}
