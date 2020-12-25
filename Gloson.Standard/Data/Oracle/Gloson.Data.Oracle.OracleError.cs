using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Runtime.Serialization;
using System.Text.RegularExpressions;

namespace Gloson.Data.Oracle {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Oracle Error
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class OracleError
    : IEquatable<OracleError>,
      IComparable<OracleError>,
      ISerializable {

    #region Algorithm

    private static readonly Regex s_Regex =
      new Regex(@"^\s*(?<name>[\w\*\d]+)\s*\-\s*(?<number>[0-9]{1,5})\s*:\s*(?<message>.*)$");

    #endregion Algorithm

    #region Create

    // Serializable 
    internal OracleError(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      Prefix = info.GetString("Prefix");
      Number = info.GetInt32("Number");
      Message = info.GetString("Message");
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="prefix">Prefix</param>
    /// <param name="number">Number</param>
    /// <param name="message">Message</param>
    public OracleError(string prefix, int number, string message) {
      if (prefix is null)
        throw new ArgumentNullException(nameof(prefix));
      else if (number <= -100_000 || number >= 100_000)
        throw new ArgumentOutOfRangeException(nameof(number));
      else if (message is null)
        throw new ArgumentNullException(nameof(message));

      Prefix = prefix.Trim().ToUpperInvariant();
      Number = Math.Abs(number);
      Message = message.Trim();
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out OracleError result) {
      result = null;

      if (string.IsNullOrWhiteSpace(value))
        return false;

      Match match = s_Regex.Match(value);

      if (!match.Success)
        return false;

      result = new OracleError(
        match.Groups["name"].Value,
        int.Parse(match.Groups["number"].Value, CultureInfo.InvariantCulture),
        match.Groups["message"].Value
      );

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static OracleError Parse(string value) {
      if (TryParse(value, out OracleError result))
        return result;
      else
        throw new FormatException($"Failed to parsed to {typeof(OracleError).Name}");
    }

    /// <summary>
    /// Parse Errors
    /// </summary>
    /// <param name="text">Raw Text</param>
    /// <returns>Errors</returns>
    public static IEnumerable<OracleError> ParseErrors(string text) {
      if (string.IsNullOrWhiteSpace(text))
        yield break;

      OracleError current = null;

      foreach (string line in text.SplitToLines()) {
        if (OracleError.TryParse(line, out OracleError error)) {
          if (current is not null)
            yield return current;

          current = error;
        }
        else if (current is null)
          yield break;
        else
          current.Message += Environment.NewLine + line;
      }

      if (current is not null)
        yield return current;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(OracleError left, OracleError right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (left is null)
        return -1;
      else if (right is null)
        return 1;

      int result = string.Compare(left.Prefix, right.Prefix, StringComparison.Ordinal);

      if (result != 0)
        return result;

      result = left.Number.CompareTo(right.Number);

      if (result != 0)
        return result;

      result = string.Compare(left.Message, right.Message, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = string.Compare(left.Message, right.Message, StringComparison.Ordinal);

      if (result != 0)
        return result;

      return result;
    }

    /// <summary>
    /// Prefix
    /// </summary>
    public string Prefix { get; }

    /// <summary>
    /// Number
    /// </summary>
    public int Number { get; }

    /// <summary>
    /// Message
    /// </summary>
    public string Message { get; private set; }

    /// <summary>
    /// Is User Error
    /// </summary>
    public bool IsUserError =>
       string.Equals("ORA", Prefix, StringComparison.InvariantCultureIgnoreCase) &&
      (Number >= 20000 && Number < 21000);

    /// <summary>
    /// Is Fatal Error
    /// </summary>
    public bool IsFatalError =>
       string.Equals("ORA", Prefix, StringComparison.InvariantCultureIgnoreCase) &&
      (Number >= 600 && Number < 700);

    /// <summary>
    /// To String
    /// </summary>
    /// <returns></returns>
    public override string ToString() => $"{Prefix}-{Number:00000}:{Message}";

    #endregion Public

    #region Operators

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(OracleError left, OracleError right) => Compare(left, right) == 0;

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(OracleError left, OracleError right) => Compare(left, right) != 0;

    /// <summary>
    /// More Or Equal
    /// </summary>
    public static bool operator >=(OracleError left, OracleError right) => Compare(left, right) >= 0;

    /// <summary>
    /// Less Or Equal
    /// </summary>
    public static bool operator <=(OracleError left, OracleError right) => Compare(left, right) <= 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(OracleError left, OracleError right) => Compare(left, right) > 0;

    /// <summary>
    /// Less
    /// </summary>
    public static bool operator <(OracleError left, OracleError right) => Compare(left, right) < 0;

    #endregion Operators

    #region IEquatable<OracleError>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(OracleError other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return string.Equals(Prefix, other.Prefix, StringComparison.OrdinalIgnoreCase) &&
             (Number == other.Number) &&
             string.Equals(Message, other.Message, StringComparison.Ordinal);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as OracleError);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Prefix.ToUpperInvariant().GetHashCode() ^ Number;

    #endregion IEquatable<OracleError>

    #region IComparable<OracleError>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(OracleError other) => Compare(this, other);

    #endregion IComparable<OracleError>

    #region ISerializable

    /// <summary>
    /// Get Object Data
    /// </summary>
    public void GetObjectData(SerializationInfo info, StreamingContext context) {
      if (info is null)
        throw new ArgumentNullException(nameof(info));

      info.AddValue("Prefix", Prefix);
      info.AddValue("Number", Number);
      info.AddValue("Message", Message);
    }

    #endregion ISerializable
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Data Exception Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class DataExceptionExtension {
    #region Public

    /// <summary>
    /// Oracle Errors
    /// </summary>
    public static IEnumerable<OracleError> OracleErrors(this DataException source)
      => OracleError.ParseErrors(source?.Message);

    #endregion Public
  }

}
