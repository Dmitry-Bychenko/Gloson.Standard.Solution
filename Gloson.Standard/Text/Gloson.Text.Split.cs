using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// New Line standards
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum NewLine {
    Smart = 0,
    Default = 1,
    R = 2,
    N = 3,
    RN = 4,
    NR = 5,
    Windows = RN,
    Unix = N,
    Apple = NR,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// New Line Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class NewLineExtensions {
    #region Public

    /// <summary>
    /// Delimiter
    /// </summary>
    /// <param name="newLine">New Line</param>
    /// <returns>delimiter or null if doesn't exists</returns>
    public static string Delimiter(this NewLine newLine) {
      return newLine switch
      {
        NewLine.Smart => null,
        NewLine.Default => Environment.NewLine,
        NewLine.N => "\n",
        NewLine.R => "\r",
        NewLine.RN => "\r\n",
        NewLine.NR => "\n\r",
        _ => null,
      };
    }

    /// <summary>
    /// Join
    /// </summary>
    /// <param name="newLine">Delimiter</param>
    /// <param name="items">Items to join</param>
    /// <returns>Joined string</returns>
    public static string Join(this NewLine newLine, params object[] items) {
      if (null == items)
        throw new ArgumentNullException(nameof(items));

      return string.Join(Delimiter(newLine) ?? Environment.NewLine, items);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StringExtensions {
    #region Algorithm

    private static IEnumerable<string> CoreSplitToLinesSmart(string source,
                                                             int count,
                                                             StringSplitOptions splitOptions) {

      if (string.IsNullOrEmpty(source))
        yield break;
      else if (count == 1) {
        yield return source;

        yield break;
      }

      int index = 0;
      int position = 0;

      for (int i = 0; i < source.Length; ++i) {
        char current = source[i];

        if (current == '\n' || current == '\r') {
          char next = i < source.Length - 1 ? source[i + 1] : '\0';

          if (position > i || splitOptions != StringSplitOptions.RemoveEmptyEntries) {
            index += 1;

            yield return source.Substring(position, i - position);
          }

          if (next != current && (next == '\n' || next == '\r'))
            i += 1;

          position = i + 1;

          if (count > 0 && index >= count - 1)
            break;
        }
      }

      // Tail if any
      if (position >= source.Length) {
        if (splitOptions != StringSplitOptions.RemoveEmptyEntries)
          yield return "";
      }
      else
        yield return source.Substring(position);
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Split to Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newLine"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   NewLine newLine,
                                                   int count,
                                                   StringSplitOptions splitOptions) {
      if (string.IsNullOrEmpty(source))
        yield break;
      else if (count == 1) {
        yield return source;

        yield break;
      }

      string delimiter = newLine.Delimiter();

      if (string.IsNullOrEmpty(delimiter)) {
        foreach (string line in CoreSplitToLinesSmart(source, count, splitOptions))
          yield return line;

        yield break;
      }

      int position = 0;
      int index = 0;

      while (true) {
        int next = source.IndexOf(delimiter, position);

        if (next < 0)
          break;

        string item = source.Substring(position, next - position - delimiter.Length);

        position = next + delimiter.Length;

        if (splitOptions == StringSplitOptions.RemoveEmptyEntries && string.IsNullOrEmpty(item))
          continue;

        index += 1;

        yield return item;

        if (count > 0 && count - index <= 1)
          break;
      }

      string tail = source.Substring(position);

      if (splitOptions != StringSplitOptions.RemoveEmptyEntries || !string.IsNullOrEmpty(tail))
        yield return tail;
    }

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newLine"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   NewLine newLine,
                                                   int count) =>
      SplitToLines(source, newLine, count, StringSplitOptions.None);

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newLine"></param>
    /// <param name="splitOptions"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   NewLine newLine,
                                                   StringSplitOptions splitOptions) =>
      SplitToLines(source, newLine, 0, splitOptions);

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="newLine"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   NewLine newLine) =>
      SplitToLines(source, newLine, 0, StringSplitOptions.None);

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <param name="splitOptions"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   int count,
                                                   StringSplitOptions splitOptions) =>
      SplitToLines(source, NewLine.Smart, count, splitOptions);

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="count"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   int count) =>
      SplitToLines(source, NewLine.Smart, count, StringSplitOptions.None);

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <param name="splitOptions"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source,
                                                   StringSplitOptions splitOptions) =>
      SplitToLines(source, NewLine.Smart, 0, splitOptions);

    /// <summary>
    /// Split To Lines
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static IEnumerable<string> SplitToLines(this string source) =>
      SplitToLines(source, NewLine.Smart, 0, StringSplitOptions.None);

    /// <summary>
    /// Split Csv
    /// </summary>
    /// <param name="source">CSV String to split</param>
    /// <param name="delimiter">Delimiter</param>
    /// <param name="quotation">Quotation</param>
    public static IEnumerable<string> SplitCsv(this string source, char delimiter, char quotation) {
      if (string.IsNullOrEmpty(source))
        yield break;

      StringBuilder sb = new StringBuilder();
      bool inQuotation = false;

      for (int i = 0; i < source.Length; ++i) {
        char ch = source[i];

        if (inQuotation) {
          if (ch == quotation) {
            i += 1;

            if (i >= source.Length || source[i] != quotation) {
              i -= 1;
              inQuotation = false;
            }
            else
              sb.Append(ch);
          }
          else
            sb.Append(ch);
        }
        else if (ch == quotation) {
          inQuotation = true;
        }
        else if (ch == delimiter) {
          yield return sb.ToString();

          sb.Clear();
        }
        else
          sb.Append(ch);
      }

      if (inQuotation)
        throw new FormatException($"Dangling {quotation} quotation");

      yield return sb.ToString();
    }

    /// <summary>
    /// Split Csv
    /// </summary>
    /// <param name="source">CSV String to split</param>
    /// <param name="delimiter">Delimiter</param>
    /// <returns></returns>
    public static IEnumerable<string> SplitCsv(this string source, char delimiter) =>
      SplitCsv(source, delimiter, '"');

    /// <summary>
    /// Split Csv
    /// </summary>
    /// <param name="source">CSV String to split</param>
    /// <returns></returns>
    public static IEnumerable<string> SplitCsv(this string source) =>
      SplitCsv(source, ',', '"');

    #endregion Public
  }

}
