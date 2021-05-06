using System;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StringExtensions {
    #region Public

    /// <summary>
    /// Quotation Add
    /// </summary>
    /// <param name="value"></param>
    /// <param name="openQuotation"></param>
    /// <param name="openEscapement"></param>
    /// <param name="closeQuotation"></param>
    /// <param name="closeEscapement"></param>
    /// <returns></returns>
    public static string QuotationAdd(this string value,
                                           char openQuotation,
                                           char openEscapement,
                                           char closeQuotation,
                                           char closeEscapement) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      StringBuilder sb = new(2 * value.Length + 2);

      sb.Append(openQuotation);

      foreach (char c in value) {
        if (c == openQuotation || c == openEscapement)
          sb.Append(openEscapement);
        else if (c == closeQuotation || c == closeEscapement)
          sb.Append(closeEscapement);

        sb.Append(c);
      }

      sb.Append(closeQuotation);

      return sb.ToString();
    }

    /// <summary>
    /// Quotation Add
    /// </summary>
    /// <param name="value"></param>
    /// <param name="quotation"></param>
    /// <param name="escapement"></param>
    /// <returns></returns>
    public static string QuotationAdd(this string value,
                                           char quotation,
                                           char escapement) =>
      QuotationAdd(value, quotation, escapement, quotation, escapement);

    /// <summary>
    /// Quotation Add
    /// </summary>
    /// <param name="value"></param>
    /// <param name="quotation"></param>
    /// <returns></returns>
    public static string QuotationAdd(this string value,
                                           char quotation) =>
      QuotationAdd(value, quotation, quotation, quotation, quotation);

    /// <summary>
    /// Quotation Add
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string QuotationAdd(this string value) =>
      QuotationAdd(value, '"', '"', '"', '"');

    /// <summary>
    /// Try Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <param name="openQuotation"></param>
    /// <param name="openEscapement"></param>
    /// <param name="closeQuotation"></param>
    /// <param name="closeEscapement"></param>
    /// <returns></returns>
    public static bool TryQuotationRemove(this string value,
                                           out string result,
                                               char openQuotation,
                                               char openEscapement,
                                               char closeQuotation,
                                               char closeEscapement) {
      result = null;

      if (value is null)
        return false;
      else if (value.Length <= 1)
        return false;
      else if (value[0] != openQuotation || value[^1] != closeQuotation)
        return false;

      StringBuilder sb = new(value.Length);

      for (int i = 1; i < value.Length - 1; ++i) {
        char ch = value[i];

        if (ch == openEscapement) {
          if (i == value.Length - 2)
            return false;

          i += 1;
          ch = value[i];

          if (ch != openEscapement && ch != openQuotation)
            return false;
        }
        else if (ch == openQuotation)
          return false;
        else if (ch == closeEscapement) {
          if (i == value.Length - 2)
            return false;

          i += 1;
          ch = value[i];

          if (ch != closeEscapement && ch != closeQuotation)
            return false;
        }
        else if (ch == closeQuotation)
          return false;

        sb.Append(ch);
      }

      return true;
    }

    /// <summary>
    /// Try Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <param name="quotation"></param>
    /// <param name="escapement"></param>
    /// <returns></returns>
    public static bool TryQuotationRemove(this string value,
                                           out string result,
                                               char quotation,
                                               char escapement) =>
      TryQuotationRemove(value, out result, quotation, escapement, quotation, escapement);

    /// <summary>
    /// Try Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <param name="quotation"></param>
    /// <returns></returns>
    public static bool TryQuotationRemove(this string value,
                                           out string result,
                                               char quotation) =>
      TryQuotationRemove(value, out result, quotation, quotation, quotation, quotation);

    /// <summary>
    /// Try Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="result"></param>
    /// <returns></returns>
    public static bool TryQuotationRemove(this string value,
                                           out string result) =>
      TryQuotationRemove(value, out result, '"', '"', '"', '"');

    /// <summary>
    /// Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="openQuotation"></param>
    /// <param name="openEscapement"></param>
    /// <param name="closeQuotation"></param>
    /// <param name="closeEscapement"></param>
    /// <returns></returns>
    public static string QuotationRemove(this string value,
                                                char openQuotation,
                                                char openEscapement,
                                                char closeQuotation,
                                                char closeEscapement) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      if (value.Length <= 1)
        throw new FormatException("Incorrect value length");
      else if (value[0] != openQuotation || value[^1] != closeQuotation)
        throw new FormatException("Doesn't have start/final quotation marks");

      StringBuilder sb = new(value.Length);

      for (int i = 1; i < value.Length - 1; ++i) {
        char ch = value[i];

        if (ch == openEscapement) {
          if (i == value.Length - 2)
            throw new FormatException($"Dangling escapement '{openEscapement}'.");

          i += 1;
          ch = value[i];

          if (ch != openEscapement && ch != openQuotation)
            throw new FormatException($"Incorrect escapement '{openEscapement}'.");
        }
        else if (ch == openQuotation)
          throw new FormatException($"Dangling quotation '{openQuotation}'.");
        else if (ch == closeEscapement) {
          if (i == value.Length - 2)
            throw new FormatException($"Dangling escapement '{closeEscapement}'.");

          i += 1;
          ch = value[i];

          if (ch != closeEscapement && ch != closeQuotation)
            throw new FormatException($"Incorrect escapement '{closeEscapement}'.");
        }
        else if (ch == closeQuotation)
          throw new FormatException($"Dangling quotation '{closeQuotation}'.");

        sb.Append(ch);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="quotation"></param>
    /// <param name="escapement"></param>
    /// <returns></returns>
    public static string QuotationRemove(this string value,
                                                char quotation,
                                                char escapement) =>
      QuotationRemove(value, quotation, escapement, quotation, escapement);

    /// <summary>
    /// Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <param name="quotation"></param>
    /// <returns></returns>
    public static string QuotationRemove(this string value,
                                                char quotation) =>
      QuotationRemove(value, quotation, quotation, quotation, quotation);

    /// <summary>
    /// Remove Quotation
    /// </summary>
    /// <param name="value"></param>
    /// <returns></returns>
    public static string QuotationRemove(this string value) =>
      QuotationRemove(value, '"', '"', '"', '"');

    #endregion Public
  }

}
