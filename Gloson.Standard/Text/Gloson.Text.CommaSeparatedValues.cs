using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Comma Separated Values
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class CommaSeparatedValues {
    #region Public

    /// <summary>
    /// Object to csv
    /// </summary>
    public static string ObjectToCsv(object value, char delimiter, char quotation) {
      string item = value?.ToString() ?? "";

      return item.IndexOf(delimiter) >= 0 || item.IndexOf(quotation) >= 0
        ? item.QuotationAdd(quotation)
        : item;
    }

    /// <summary>
    /// Objects (collection) to scv
    /// </summary>
    public static string ObjectsToCsv<T>(IEnumerable<T> value, char delimiter, char quotation) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      return string.Join(delimiter.ToString(), value
        .Select(item => ObjectToCsv(item, delimiter, quotation)));
    }

    /// <summary>
    /// Parse CSV
    /// </summary>
    public static IEnumerable<string[]> ParseCsv(IEnumerable<string> lines, char delimiter, char quotation) {
      if (null == lines)
        throw new ArgumentNullException(nameof(lines));

      List<string> items = new List<string>();
      bool inQuotation = false;
      StringBuilder sb = new StringBuilder();

      foreach (var line in lines) {
        if (null == line)
          continue;

        for (int i = 0; i < line.Length; ++i) {
          char ch = line[i];

          if (inQuotation) {
            if (ch == quotation) {
              i += 1;

              if (i >= line.Length || line[i] != quotation) {
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
            items.Add(sb.ToString());

            sb.Clear();
          }
          else
            sb.Append(ch);
        }

        // Line completed
        if (!inQuotation) {
          if (sb.Length > 0 || items.Any()) {
            items.Add(sb.ToString());

            yield return items.ToArray();
          }

          sb.Clear();
          items.Clear();
        }
      }

      if (inQuotation)
        throw new FormatException("Unterminated quotation");

      if (items.Any())
        yield return items.ToArray();
    }

    /// <summary>
    /// Parse Csv
    /// </summary>
    public static IEnumerable<string[]> ParseCsv(TextReader reader, char delimiter, char quotation) {
      if (null == reader)
        throw new ArgumentNullException(nameof(reader));

      return ParseCsv(reader.AsEnumerable(), delimiter, quotation);
    }

    /// <summary>
    /// Parse Csv
    /// </summary>
    public static IEnumerable<string[]> ParseCsv(Stream stream, char delimiter, char quotation, Encoding encoding) {
      if (null == stream)
        throw new ArgumentNullException(nameof(stream));

      using StreamReader reader = new StreamReader(stream, encoding, true, 8192, true);
      
      return ParseCsv(reader, delimiter, quotation);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Enumerable Extensions (CSV)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class EnumerableExtensions {
    #region Public

    /// <summary>
    /// To CSV
    /// </summary>
    public static IEnumerable<string> ToCsv<T>(this IEnumerable<IEnumerable<T>> source, char delimiter, char quotation) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      foreach (IEnumerable<T> line in source) {
        if (null == line)
          yield return "";
        else
          yield return CommaSeparatedValues.ObjectsToCsv(line, delimiter, quotation);
      }
    }

    /// <summary>
    /// To CSV
    /// </summary>
    public static IEnumerable<string> ToCsv<T>(this IEnumerable<IEnumerable<T>> source, char delimiter)
      => ToCsv(source, delimiter, '"');

    /// <summary>
    /// To CSV
    /// </summary>
    public static IEnumerable<string> ToCsv<T>(this IEnumerable<IEnumerable<T>> source)
      => ToCsv(source, ',', '"');

    /// <summary>
    /// From Csv
    /// </summary>
    public static IEnumerable<string[]> FromCsv(this IEnumerable<string> source, char delimiter, char quotation) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      return CommaSeparatedValues.ParseCsv(source, delimiter, quotation);
    }

    /// <summary>
    /// From Csv
    /// </summary>
    public static IEnumerable<string[]> FromCsv(this IEnumerable<string> source, char delimiter) =>
       FromCsv(source, delimiter, '"');

    /// <summary>
    /// From Csv
    /// </summary>
    public static IEnumerable<string[]> FromCsv(this IEnumerable<string> source) =>
       FromCsv(source, ',', '"');

    #endregion Public
  }

}
