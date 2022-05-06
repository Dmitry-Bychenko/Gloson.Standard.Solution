using Gloson.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace Gloson.Ini {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Name Value Record
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class NameValueRecord : IEquatable<NameValueRecord> {
    #region Private Data

    private string m_Value = "";

    private string m_Comment = "";

    #endregion Private Data

    #region Create

    private NameValueRecord(string name, string value) {
      Name = name ?? "";
      Value = value ?? "";
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public NameValueRecord(NameValueDocument owner, string name, string value) {
      Name = name ?? "";
      Value = value ?? "";

      if (owner is not null)
        owner.CoreAddRecord(this);
    }

    /// <summary>
    /// Try Parse
    /// </summary>
    public static bool TryParse(string value, out NameValueRecord result) {
      result = null;

      if (string.IsNullOrEmpty(value))
        return false;

      value = value.Trim();

      if (string.IsNullOrEmpty(value))
        return false;

      if (!value.StartsWith("\"") && !value.StartsWith("'")) {
        int p = value.IndexOf('=');

        if (p < 0)
          return false;

        result = new NameValueRecord(value[0..p].Trim(), value[(p + 1)..].Trim());
        return true;
      }

      bool inQuot = false;
      bool inApostroph = false;

      for (int i = 0; i < value.Length; ++i) {
        if (inQuot) {
          if (value[i] == '"')
            inQuot = false;

          continue;
        }
        else if (inApostroph) {
          if (value[i] == '\'')
            inApostroph = false;

          continue;
        }

        if (value[i] == '"') {
          inQuot = true;

          continue;
        }
        else if (value[i] == '\'') {
          inApostroph = true;

          continue;
        }

        if (!inQuot && value[i] == '=') {
          string name = value[0..i].Trim();

          if (name.StartsWith("\"")) {
            if (!name.TryQuotationRemove(out name))
              return false;
          }
          else if (name.StartsWith("'")) {
            if (!name.TryQuotationRemove(out name, '\''))
              return false;
          }

          string v = value[(i + 1)..];

          if (v.StartsWith("\"")) {
            if (!v.TryQuotationRemove(out v))
              return false;
          }
          else if (v.StartsWith("'")) {
            if (!v.TryQuotationRemove(out v, '\''))
              return false;
          }

          result = new NameValueRecord(name, v);
          return true;
        }
      }

      return false;
    }

    /// <summary>
    /// Parse
    /// </summary>
    public static NameValueRecord Parse(string value) {
      if (TryParse(value, out NameValueRecord result))
        return result;

      throw new FormatException("Not valid Ini File record");
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Owner
    /// </summary>
    public NameValueDocument Owner { get; internal set; }

    /// <summary>
    /// Comparer
    /// </summary>
    public StringComparer Comparer => Owner?.Comparer ?? StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Name
    /// </summary>
    public string Value {
      get {
        return m_Value;
      }
      set {
        m_Value = value ?? "";
      }
    }

    /// <summary>
    /// Comment
    /// </summary>
    public string Comments {
      get {
        return m_Comment;
      }
      set {
        m_Comment = value ?? "";
      }
    }

    /// <summary>
    /// To String (Name)
    /// </summary>
    public override string ToString() => Name;

    #endregion Public

    #region IEquatable<NameValueRecord>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(NameValueRecord other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      if (Comparer != other.Comparer)
        return false;

      return Comparer.Equals(Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as NameValueRecord);

    /// <summary>
    /// Get HashCode
    /// </summary>
    public override int GetHashCode() {
      return Comparer.GetHashCode(Name);
    }

    #endregion IEquatable<NameValueRecord>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Name Value Document
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class NameValueDocument {
    #region Private Data

    private readonly Dictionary<string, NameValueRecord> m_Records;

    #endregion Private Data

    #region Algorithm

    private static string EncodeName(string value) {
      if (string.IsNullOrEmpty(value))
        return "";

      if (value.Contains('"') || value.Contains('=') ||
          value.StartsWith("#") || value.StartsWith(";") || value.StartsWith(" ") ||
          value.EndsWith(" "))
        return value.QuotationAdd('"', '"');

      return value;
    }

    private static string EncodeValue(string value) {
      if (string.IsNullOrEmpty(value))
        return "";

      return string.Concat(value.Where(c => !char.IsControl(c)));
    }

    internal void CoreAddRecord(NameValueRecord record) {
      if (record is null)
        return;

      if (record.Owner == this)
        return;

      if (record.Owner is not null)
        record.Owner.CoreRemoveRecord(record);

      if (m_Records.TryGetValue(record.Name, out var old))
        old.Owner.CoreRemoveRecord(old);

      m_Records.Add(record.Name, record);
    }

    internal void CoreRemoveRecord(NameValueRecord record) {
      if (record is null)
        return;

      if (record.Owner != this)
        return;

      m_Records.Remove(record.Name);

      record.Owner = null;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public NameValueDocument(StringComparer comparer, IniFileCommentKind commentKind) {
      Comparer = comparer ?? StringComparer.OrdinalIgnoreCase;
      CommentKind = commentKind;

      m_Records = new Dictionary<string, NameValueRecord>(comparer);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public NameValueDocument(StringComparer comparer) : this(comparer, IniFileCommentKind.Standard) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public NameValueDocument() : this(null, IniFileCommentKind.Standard) { }

    /// <summary>
    /// Load
    /// </summary>
    public static NameValueDocument Load(IEnumerable<string> lines,
                                         StringComparer comparer,
                                         IniFileCommentKind commentKind) {
      if (lines is null)
        throw new ArgumentNullException(nameof(lines));

      NameValueDocument result = new(comparer, commentKind);

      List<string> comments = new();

      int index = 0;

      foreach (string line in lines.Select(x => x.Trim())) {
        index += 1;

        if (string.IsNullOrWhiteSpace(line))
          continue;

        if (line.StartsWith(";") || line.StartsWith("#")) {
          comments.Add(line[1..]);

          continue;
        }

        if (NameValueRecord.TryParse(line, out var record)) {
          record.Comments = string.Join(Environment.NewLine, comments);

          comments.Clear();

          result.Add(record);
        }
        else
          throw new FormatException($"Syntax error in line {index}");
      }

      return result;
    }

    /// <summary>
    /// Load
    /// </summary>
    public static NameValueDocument Load(IEnumerable<string> lines, StringComparer comparer) =>
      Load(lines, comparer, IniFileCommentKind.Standard);

    /// <summary>
    /// Load
    /// </summary>
    public static NameValueDocument Load(IEnumerable<string> lines) =>
      Load(lines, null, IniFileCommentKind.Standard);

    /// <summary>
    /// Load From File
    /// </summary>
    public static NameValueDocument LoadFromFile(string fileName,
                                                 Encoding encoding,
                                                 StringComparer comparer,
                                                 IniFileCommentKind commentKind) {
      NameValueDocument result = Load(File.ReadAllLines(fileName, encoding), comparer, commentKind);

      result.FileEncoding = encoding;

      return result;
    }

    /// <summary>
    /// Load From File
    /// </summary>
    public static NameValueDocument LoadFromFile(string fileName,
                                                 StringComparer comparer,
                                                 IniFileCommentKind commentKind) =>
      LoadFromFile(fileName, Encoding.UTF8, comparer, commentKind);

    /// <summary>
    /// Load From File
    /// </summary>
    public static NameValueDocument LoadFromFile(string fileName,
                                                 StringComparer comparer) =>
      LoadFromFile(fileName, Encoding.UTF8, comparer, IniFileCommentKind.Standard);

    /// <summary>
    /// Load From File
    /// </summary>
    public static NameValueDocument LoadFromFile(string fileName) =>
      LoadFromFile(fileName, Encoding.UTF8, null, IniFileCommentKind.Standard);

    #endregion Create

    #region Public

    /// <summary>
    /// Comparer
    /// </summary>
    public StringComparer Comparer { get; }

    /// <summary>
    /// Comment Kind
    /// </summary>
    public IniFileCommentKind CommentKind { get; }

    /// <summary>
    /// File Encoding
    /// </summary>
    public Encoding FileEncoding { get; set; }

    /// <summary>
    /// Add
    /// </summary>
    public NameValueRecord Add(string name, string value) {
      return new NameValueRecord(this, name, value);
    }

    /// <summary>
    /// Add
    /// </summary>
    public NameValueRecord Add(NameValueRecord item) {
      if (item is null)
        throw new ArgumentNullException(nameof(item));

      CoreAddRecord(item);

      return item;
    }

    /// <summary>
    /// Record
    /// </summary>
    public bool Remove(string name) {
      if (m_Records.TryGetValue(name, out var record)) {
        CoreRemoveRecord(record);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Try Get Value
    /// </summary>
    public bool TryGetValue(string name, out NameValueRecord record) {
      if (m_Records.TryGetValue(name, out record))
        return true;

      record = null;

      return false;
    }

    /// <summary>
    /// Indexer
    /// </summary>
    public string this[string name] {
      get {
        if (m_Records.TryGetValue(name, out var record))
          return record.Value;

        return null;
      }
      set {
        if (value is null)
          Remove(name);
        else
          Add(name, value);
      }
    }

    /// <summary>
    /// Lines
    /// </summary>
    public IEnumerable<string> Lines {
      get {
        foreach (var record in m_Records.Values) {
          foreach (string comment in record.Comments.SplitToLines())
            yield return comment;

          yield return $"{EncodeName(record.Name)}={EncodeValue(record.Value)}";
        }
      }
    }

    #endregion Public
  }
}
