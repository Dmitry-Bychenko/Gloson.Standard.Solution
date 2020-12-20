using System;
using System.Collections.Generic;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Memory Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class MemoryExtensions {
    #region Public

    /// <summary>
    /// Memory<char> as String
    /// </summary>
    /// <param name="value">Text to Split</param>
    /// <param name="delimiter">Delimiter</param>
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> value, string delimiter) {
      if (null == delimiter)
        throw new ArgumentNullException(nameof(delimiter));

      int last = 0;

      while (true) {
        int at = value.Span[last..].IndexOf(delimiter);

        if (at < 0) {
          yield return value[last..];

          yield break;
        }

        yield return value.Slice(last, at);

        last += at + delimiter.Length;
      }
    }

    /// <summary>
    /// Memory<char> as String to Split by Environment.NewLine
    /// </summary>
    /// <param name="value">Value to Split</param>
    /// <returns></returns>
    public static IEnumerable<ReadOnlyMemory<char>> Split(this ReadOnlyMemory<char> value) =>
      Split(value, Environment.NewLine);

    #endregion Public
  }

}
