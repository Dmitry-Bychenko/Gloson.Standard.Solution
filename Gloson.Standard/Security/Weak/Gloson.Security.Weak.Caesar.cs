using System;
using System.Text;

namespace Gloson.Security.Weak {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Caesar 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Caesar {
    #region Algorithm

    /// <summary>
    /// Encrypt
    /// </summary>
    /// <param name="source">Plain Text</param>
    /// <param name="shift">Shift</param>
    /// <param name="ranges">Ranges to encrypt</param>
    /// <returns>Encrypted text</returns>
    public static String Encrypt(string source, int shift, params (char left, char right)[] ranges) {
      if (string.IsNullOrEmpty(source))
        return source;
      else if (ranges is null || ranges.Length == 0)
        return source;

      StringBuilder sb = new StringBuilder(source.Length);

      foreach (char c in source) {
        int index = -1;

        for (int i = 0; i < ranges.Length; ++i)
          if (ranges[i].left <= c && c <= ranges[i].right) {
            index = i;

            break;
          }

        if (index < 0)
          sb.Append(c);
        else {
          int length = ranges[index].right - ranges[index].left + 1;

          int delta = ((shift % length) + length) % length;
          int next = ranges[index].left + (c - ranges[index].left + delta) % length;

          sb.Append((char)next);
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// Decrypt
    /// </summary>
    /// <param name="source">Encrypted Text</param>
    /// <param name="shift">Shift</param>
    /// <param name="ranges">Ranges to encrypt</param>
    /// <returns>Plain text</returns>
    public static String Decrypt(string source, int shift, params (char left, char right)[] ranges) =>
      Encrypt(source, -shift, ranges);

    /// <summary>
    /// Rotate 13 Caesar Encryption
    /// </summary>
    public static String Rotate13(string source) =>
      Encrypt(source, 13, ('a', 'z'), ('A', 'Z'));

    #endregion Algorithm
  }

}
