using Gloson.Text;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;

namespace Gloson.Biology {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Fasta
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Fasta :
    IEquatable<Fasta>,
    IFormattable {

    #region Private Data

    private static readonly Regex s_WhiteSpaces = new(@"\s+");

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="description">Description</param>
    /// <param name="sequence">Sequence</param>
    /// <param name="comments">Comments</param>
    public Fasta(string description, string sequence, string comments) {
      if (description is null)
        throw new ArgumentNullException(nameof(description));
      else if (sequence is null)
        throw new ArgumentNullException(nameof(sequence));

      if (string.IsNullOrWhiteSpace(description))
        throw new ArgumentException("Empty description is not allowed", nameof(description));
      else if (string.IsNullOrWhiteSpace(sequence))
        throw new ArgumentException("Empty sequence is not allowed", nameof(sequence));

      description = description.Trim();

      if (description.Any(c => char.IsControl(c)))
        throw new ArgumentException("Description must not contain control symbols.", nameof(description));

      sequence = s_WhiteSpaces.Replace(sequence.Trim().TrimEnd('*').TrimEnd(), "").ToUpper();

      if (sequence.Any(c => !(c >= 'A' && c <= 'Z')))
        throw new ArgumentException("Sequence must contain A..Z letters only", nameof(sequence));

      if (sequence.Length <= 0)
        throw new ArgumentException("Empty sequence is not allowed", nameof(sequence));

      Description = description;
      Sequence = sequence;
      Comments = comments ?? "";
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="description">Description</param>
    /// <param name="sequence">Sequence</param>
    /// <param name="comments">Comments</param>
    public Fasta(string description, string sequence)
      : this(description, sequence, "") { }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string text, out Fasta result) {
      result = null;

      if (text is null)
        return false;

      var lines = text
        .SplitToLines()
        .Select(item => item.Trim())
        .Where(item => !string.IsNullOrEmpty(item));

      List<string> comments = new();
      List<string> seq = new();

      string description = null;

      foreach (string line in lines) {
        if (line.StartsWith(";") || line.StartsWith("#"))
          comments.Add(line[1..]);
        else if (line.StartsWith(">")) {
          if (description is not null)
            return false;

          description = line[1..].Trim();

          if (string.IsNullOrEmpty(description))
            return false;
          else if (description.Any(c => char.IsControl(c)))
            return false;
        }
        else {
          string s = s_WhiteSpaces.Replace(line.TrimEnd('*'), "");

          if (s.Any(c => !(c >= 'A' && c <= 'Z')))
            return false;

          seq.Add(s);
        }
      }

      string sequence = string.Concat(seq);

      if (sequence.Length <= 0)
        return false;
      else if (string.IsNullOrWhiteSpace(description))
        return false;

      result = new Fasta(
        description,
        sequence,
        string.Join(Environment.NewLine, comments));

      return true;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static Fasta Parse(string text) => TryParse(text, out Fasta result)
      ? result
      : throw new FormatException("Failed to parse into Fasta");

    #endregion Create

    #region Public

    /// <summary>
    /// Description
    /// </summary>
    public string Description { get; }

    /// <summary>
    /// Sequence
    /// </summary>
    public string Sequence { get; }

    /// <summary>
    /// Comments
    /// </summary>
    public string Comments { get; }

    /// <summary>
    /// Aminos
    /// </summary>
    public IEnumerable<AminoAcid> Aminos => Sequence.Select(c => (AminoAcid)c);

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(Fasta left, Fasta right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (right is null || left is null)
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Fasta left, Fasta right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (right is null || left is null)
        return true;
      else
        return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<Fasta>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Fasta other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return string.Equals(Sequence, other.Sequence);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as Fasta);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Sequence.GetHashCode();

    #endregion IEquatable<Fasta>

    #region IFormatable

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => ToString(0);

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(int length) {
      int n = length <= 0 ? 80 : length;

      var comments = Comments
        .SplitToLines()
        .Select(line => $"{line}");

      var descriptions = new string[] { $">{Description}" };
      var body = Enumerable
        .Range(0, Sequence.Length / n + (Sequence.Length % n == 0 ? 0 : 1))
        .Select(i => i >= Sequence.Length / n
           ? Sequence[(i * n)..]
           : Sequence.Substring(i * n, n));

      return string.Join(Environment.NewLine, comments.Concat(descriptions).Concat(body));
    }

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider) {
      string raw = format;

      format = format?.Trim().ToUpper();

      if (string.IsNullOrWhiteSpace(format) || "g" == format)
        return ToString();

      if (format.StartsWith("t") || format.StartsWith("s")) {
        if (int.TryParse(format[1..], out int size))
          return ToString(size);
      }

      throw new FormatException($"Invalid format {raw}");
    }

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format) => ToString(format, CultureInfo.InvariantCulture);

    #endregion IFormatable
  }

}
