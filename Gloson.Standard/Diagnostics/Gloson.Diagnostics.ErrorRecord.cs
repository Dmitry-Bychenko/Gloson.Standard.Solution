using Gloson.IO;
using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Diagnostics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Error Code
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ErrorRecord
    : IEquatable<ErrorRecord>,
      IComparable<ErrorRecord> {

    #region Algorithm

    private static readonly int SeverityMaxLength = Enum
      .GetNames(typeof(ErrorSeverity))
      .Max(item => item.Length);

    private static readonly int PriorityMaxLength = Enum
      .GetNames(typeof(ErrorPriority))
      .Max(item => item.Length);


    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ErrorRecord(string fileName = null,
                       string description = null,
                       string errorCategory = null,
                       int errorCode = 0,
                       ErrorSeverity severity = ErrorSeverity.Major,
                       ErrorPriority priority = ErrorPriority.Medium,
                       int line = -1,
                       int column = -1) {
      FileName = fileName?.Trim() ?? "";
      Description = description?.Trim() ?? "";
      ErrorCategory = (errorCategory ?? "").Trim().PadRight(3, '_').Substring(0, 3).ToUpperInvariant();
      ErrorCode = errorCode < 0 ? 0 : errorCode;
      Severity = severity;
      Priority = priority;
      Line = line < 0 ? -1 : line;
      Column = column < 0 ? -1 : column;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(ErrorRecord left, ErrorRecord right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (null == left)
        return -1;
      else if (null == right)
        return 1;

      int result = string.Compare(left.FileName, right.FileName, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = -left.Priority.CompareTo(right.Priority);

      if (result != 0)
        return result;

      result = -left.Severity.CompareTo(right.Severity);

      if (result != 0)
        return result;

      result = string.Compare(left.ErrorCategory, right.ErrorCategory, StringComparison.Ordinal);

      if (result != 0)
        return result;

      result = left.ErrorCode.CompareTo(right.ErrorCode);

      if (result != 0)
        return result;

      result = left.Line.CompareTo(right.Line);

      if (result != 0)
        return result;

      result = left.Column.CompareTo(right.Column);

      if (result != 0)
        return result;

      result = string.Compare(left.Description, right.Description, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      result = string.Compare(left.Description, right.Description, StringComparison.Ordinal);

      if (result != 0)
        return result;

      return 0;
    }

    /// <summary>
    /// Severity
    /// </summary>
    public ErrorSeverity Severity { get; } = ErrorSeverity.Major;

    /// <summary>
    /// Priority
    /// </summary>
    public ErrorPriority Priority { get; } = ErrorPriority.Medium;

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; } = "";

    /// <summary>
    /// Category
    /// </summary>
    public string ErrorCategory { get; } = "UNK";

    /// <summary>
    /// Category
    /// </summary>
    public int ErrorCode { get; } = 0;

    /// <summary>
    /// Code
    /// </summary>
    public string Code => $"{ErrorCategory.PadRight(3, '_')}{ErrorCode:d6}";

    /// <summary>
    /// File Name
    /// </summary>
    public string FileName { get; } = "";

    /// <summary>
    /// Line
    /// </summary>
    public int Line { get; } = -1;

    /// <summary>
    /// Column
    /// </summary>
    public int Column { get; } = -1;

    /// <summary>
    /// Records
    /// </summary>
    public IEnumerable<string> Records {
      get {
        yield return Priority.ToString();
        yield return Severity.ToString();

        yield return Code;
        yield return Description;
        yield return FileName;

        if (Line >= 0 && Column >= 0)
          yield return $"{Line:000000}:{Column:0000}";
        else if (Line >= 0)
          yield return $"{Line:000000}";
        else if (Column >= 0)
          yield return $"?:{Column:0000}";
        else
          yield return "";
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      return String.Join(" ", Records
        .Where(item => !string.IsNullOrWhiteSpace(item))
        .Select(item => Regex.Replace(item.Trim(), @"\s+", " ")));
    }

    /// <summary>
    /// To Report
    /// </summary>
    public string ToReport(string fileNameRoot = null,
                           int shift = 0) {
      fileNameRoot = fileNameRoot?.Trim() ?? "";
      string pad = shift <= 0 ? "" : new string(' ', shift);

      var lines = Description.SplitToLines().ToArray();

      StringBuilder sb = new StringBuilder();

      sb.Append(pad);

      sb.Append(Priority.ToString().PadLeft(PriorityMaxLength));
      sb.Append(" : ");
      sb.Append(Severity.ToString().PadLeft(SeverityMaxLength));
      sb.Append(" ");

      if (lines.Length > 0)
        sb.Append(lines[0].Trim());

      sb.Append(PathHelper.Subtract(FileName, fileNameRoot));

      sb.Append(" ");

      if (Line >= 0 && Column >= 0)
        sb.Append($"{Line:000000}:{Column:0000}");
      else if (Line >= 0)
        sb.Append($"{Line:000000}");
      else if (Column >= 0)
        sb.Append($"?:{Column:0000}");

      for (int i = 1; i < lines.Length; ++i) {
        sb.AppendLine();
        sb.Append(new string(' ', PriorityMaxLength + SeverityMaxLength + 4));
        sb.Append(lines[i].Trim());
      }

      return sb.ToString();
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(ErrorRecord left, ErrorRecord right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (null == left || null == right)
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(ErrorRecord left, ErrorRecord right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (null == left || null == right)
        return true;
      else
        return !left.Equals(right);
    }

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >(ErrorRecord left, ErrorRecord right) => Compare(left, right) > 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator >=(ErrorRecord left, ErrorRecord right) => Compare(left, right) >= 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator <(ErrorRecord left, ErrorRecord right) => Compare(left, right) < 0;

    /// <summary>
    /// More
    /// </summary>
    public static bool operator <=(ErrorRecord left, ErrorRecord right) => Compare(left, right) <= 0;

    #endregion Operators

    #region IEquatable<ErrorRecord>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ErrorRecord other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return string.Equals(FileName, other.FileName, StringComparison.OrdinalIgnoreCase) &&
             Severity == other.Severity &&
             Priority == other.Priority &&
             string.Equals(Description, other.Description, StringComparison.Ordinal) &&
             string.Equals(ErrorCategory, other.ErrorCategory, StringComparison.OrdinalIgnoreCase) &&
             ErrorCode == other.ErrorCode &&
             Line == other.Line &&
             Column == other.Column;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return obj is ErrorRecord other
        ? Equals(other)
        : false;
    }

    /// <summary>
    /// Get Hash Code
    /// </summary>
    public override int GetHashCode() {
      unchecked {
        return StringComparer.OrdinalIgnoreCase.GetHashCode(FileName) ^
               (Line << 16) ^
               (Column << 24) ^
               ErrorCode;
      }
    }

    #endregion IEquatable<ErrorRecord>

    #region IComparable<ErrorRecord>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(ErrorRecord other) => Compare(this, other);

    #endregion IComparable<ErrorRecord>
  }
}
