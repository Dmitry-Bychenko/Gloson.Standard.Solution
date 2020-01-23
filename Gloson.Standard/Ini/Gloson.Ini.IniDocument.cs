using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;

using Gloson.Globalization;
using Gloson.Text;

namespace Gloson.Ini {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini Document Section
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniDocumentSection 
    : IEquatable<IniDocumentSection>,
      IDictionary<string, string> {

    #region Private Data

    private Dictionary<string, IniDocumentRecord> m_Records;

    private string m_Comments = "";

    #endregion Private Data

    #region Algorithm

    internal void CoreUpdateRecords() {
      if (null == m_Records)
        return;

      if (m_Records.Comparer == Comparer)
        return;

      Dictionary<string, IniDocumentRecord> dict = new Dictionary<string, IniDocumentRecord>(Comparer);

      foreach (var record in m_Records) {
        if (dict.TryGetValue(record.Key, out var old))
          old.Section = null;
        else
          dict.Add(record.Key, record.Value);
      }

      m_Records.Clear();
      m_Records = dict;
    }

    internal void CoreAddRecord(IniDocumentRecord record) {
      if (null == record)
        return;

      if (record.Section == this)
        return;

      if (record.Section != null)
        record.Section.CoreRemoveRecord(record);

      if (m_Records.TryGetValue(record.Name, out var old))
        CoreRemoveRecord(old);

      m_Records.Add(record.Name, record);

      record.Section = this;
    }

    internal void CoreRemoveRecord(IniDocumentRecord record) {
      if (null == record)
        return;

      if (record.Section != this)
        return;

      m_Records.Remove(record.Name);

      record.Section = null;
    }

    #endregion Algorithm

    #region Create 

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public IniDocumentSection(string name, IniDocument document) {
      if (null == name)
        throw new ArgumentNullException(nameof(name));

      Name = string.Concat(name.Where(c => !char.IsControl(c)));

      if (null != document)
        document.CoreAddSection(this);

      m_Records = new Dictionary<string, IniDocumentRecord>(Comparer);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Comments
    /// </summary>
    public string Comments { 
      get {
        return m_Comments;
      }
      set {
        if (null == value)
          value = "";

        m_Comments = value;
      }
    }

    /// <summary>
    /// Document
    /// </summary>
    public IniDocument Document { get; internal set; }

    /// <summary>
    /// Comparer
    /// </summary>
    public StringComparer Comparer {
      get {
        if (Document != null)
          return Document.Comparer;
        else if (m_Records != null) {
          StringComparer comparer = m_Records.Comparer as StringComparer;

          if (null != comparer)
            return comparer;
        }

        return StringComparer.OrdinalIgnoreCase;
      }
    }

    /// <summary>
    /// To String (Name)
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Records
    /// </summary>
    public IEnumerable<IniDocumentRecord> Records => m_Records.Values;

    /// <summary>
    /// Add
    /// </summary>
    public IniDocumentRecord AddRecord((string, object) record) => new IniDocumentRecord(this, record.Item1, record.Item2);

    /// <summary>
    /// Add
    /// </summary>
    public IniDocumentRecord AddRecord(string name, object value) => new IniDocumentRecord(this, name, value);

    /// <summary>
    /// Add
    /// </summary>
    public IniDocumentRecord AddRecord(KeyValuePair<string, object> pair) => new IniDocumentRecord(this, pair.Key, pair.Value);

    /// <summary>
    /// Add
    /// </summary>
    public IniDocumentRecord AddRecord(string name) => new IniDocumentRecord(this, name, null);

    /// <summary>
    /// Add
    /// </summary>
    public IniDocumentRecord AddRecord(IniDocumentRecord record) {
      if (null == record)
        throw new ArgumentNullException(nameof(record));

      CoreAddRecord(record);

      return record;
    }

    /// <summary>
    /// Add Records
    /// </summary>
    public IniDocumentSection AddRecords(IEnumerable<IniDocumentRecord> records) {
      if (null == records)
        throw new ArgumentNullException(nameof(records));

      foreach (var record in records)
        AddRecord(record);

      return this;
    }

    /// <summary>
    /// Add Records
    /// </summary>
    public IniDocumentSection AddRecords(params IniDocumentRecord[] records) {
      if (null == records)
        throw new ArgumentNullException(nameof(records));

      foreach (var record in records)
        AddRecord(record);

      return this;
    }

    /// <summary>
    /// Add Records
    /// </summary>
    public IniDocumentSection AddRecords(IEnumerable<(string, object)> tuples) {
      if (null == tuples)
        throw new ArgumentNullException(nameof(tuples));

      foreach (var tuple in tuples)
        AddRecord(tuple);

      return this;
    }

    /// <summary>
    /// Add Records
    /// </summary>
    public IniDocumentSection AddRecords(params (string, object)[] tuples) {
      if (null == tuples)
        throw new ArgumentNullException(nameof(tuples));

      foreach (var tuple in tuples)
        AddRecord(tuple);

      return this;
    }

    /// <summary>
    /// Add Records
    /// </summary>
    public IniDocumentSection AddRecords(IEnumerable<KeyValuePair<string, object>> pairs) {
      if (null == pairs)
        throw new ArgumentNullException(nameof(pairs));

      foreach (var pair in pairs)
        AddRecord(pair);

      return this;
    }

    /// <summary>
    /// Add Records
    /// </summary>
    public IniDocumentSection AddRecords(params KeyValuePair<string, object>[] pairs) {
      if (null == pairs)
        throw new ArgumentNullException(nameof(pairs));

      foreach (var pair in pairs)
        AddRecord(pair);

      return this;
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(string recordName) {
      if (m_Records.TryGetValue(recordName, out var record)) {
        CoreRemoveRecord(record);

        return true;
      }

      return false;
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator == (IniDocumentSection left, IniDocumentSection right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (ReferenceEquals(null, left) || ReferenceEquals(null, right))
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Non equals
    /// </summary>
    public static bool operator !=(IniDocumentSection left, IniDocumentSection right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (ReferenceEquals(null, left) || ReferenceEquals(null, right))
        return true;
      else
        return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<IniDocumentSection

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(IniDocumentSection other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return Comparer == other.Comparer &&
             Comparer.Equals(Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as IniDocumentSection);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Comparer.GetHashCode(Name);

    #endregion IEquatable<IniDocumentSection

    #region IDictionary<string, string>

    /// <summary>
    /// Keys
    /// </summary>
    public ICollection<string> Keys => m_Records.Keys;

    /// <summary>
    /// Values
    /// </summary>
    public ICollection<string> Values => m_Records.Values.Select(m => m.Value).ToList();

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Records.Count;

    /// <summary>
    /// Is ReadOnly
    /// </summary>
    public bool IsReadOnly => false;

    /// <summary>
    /// Indexer
    /// </summary>
    public string this[string key] { 
      get {
        return m_Records.TryGetValue(key, out var record) 
          ? record.Value 
          : "";
      }
      set {
        if (null == key)
          throw new ArgumentNullException(nameof(key));

        if (m_Records.TryGetValue(key, out var record))
          record.Value = value;
        else
          AddRecord(key, value);
      }
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(string key, string value) => AddRecord(key, value);

    /// <summary>
    /// Contains Key
    /// </summary>
    public bool ContainsKey(string key) => m_Records.ContainsKey(key);

    /// <summary>
    /// Try Get Value
    /// </summary>
    public bool TryGetValue(string key, out string value) {
      if (m_Records.TryGetValue(key, out var record)) {
        value = record?.Value;

        return true;
      }

      value = null;

      return false;
    }

    /// <summary>
    /// Add
    /// </summary>
    public void Add(KeyValuePair<string, string> item) => AddRecord((item.Key, item.Value));

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      var data = m_Records.Values.ToList();

      foreach (var item in data)
        CoreRemoveRecord(item);
    }

    /// <summary>
    /// Contains
    /// </summary>
    public bool Contains(KeyValuePair<string, string> item) {
      return m_Records.TryGetValue(item.Key, out var record) && string.Equals(record.Value, item.Value);
    }

    /// <summary>
    /// CopyTo
    /// </summary>
    public void CopyTo(KeyValuePair<string, string>[] array, int arrayIndex) {
      if (null == array)
        throw new ArgumentNullException(nameof(array));

      if (arrayIndex < array.GetLowerBound(0) || arrayIndex + Count > array.GetUpperBound(0))
        throw new ArgumentOutOfRangeException(nameof(arrayIndex));

      int i = arrayIndex;

      foreach (var pair in m_Records) {
        array[i] = new KeyValuePair<string, string>(pair.Key, pair.Value.Value);

        i += 1; 
      }
    }

    /// <summary>
    /// False
    /// </summary>
    public bool Remove(KeyValuePair<string, string> item) {
      if (m_Records.TryGetValue(item.Key, out var record) && string.Equals(record.Value, item.Value)) {
        CoreRemoveRecord(record);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Get Enumerator
    /// </summary>
    public IEnumerator<KeyValuePair<string, string>> GetEnumerator() {
      return m_Records
        .Select(pair => new KeyValuePair<string, string>(pair.Key, pair.Value.Value))
        .GetEnumerator();
    }

    /// <summary>
    /// Enumeartor
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IDictionary<string, string>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini Document Record
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniDocumentRecord 
    : IEquatable<IniDocumentRecord> {

    #region Private Data

    private string m_Comments = "";

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public IniDocumentRecord(IniDocumentSection section, string name, object value) {
      if (null == section)
        throw new ArgumentNullException(nameof(section));
      else if (null == name)
        throw new ArgumentNullException(nameof(name));

      Name = name;

      using (new TemporalCultureInfo()) {
        Value = value?.ToString();
      }

      if (section != null)
        section.CoreAddRecord(this);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Document
    /// </summary>
    public IniDocument Document => Section?.Document;

    /// <summary>
    /// Section
    /// </summary>
    public IniDocumentSection Section { get; internal set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name { get; }

    /// <summary>
    /// Value
    /// </summary>
    public string Value { get; set; }

    /// <summary>
    /// Comments
    /// </summary>
    public string Comments {
      get {
        return m_Comments;
      }
      set {
        if (null == value)
          value = "";

        m_Comments = value;
      }
    }

    /// <summary>
    /// Comparer
    /// </summary>
    public StringComparer Comparer => Document?.Comparer ?? StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// ToString - Name
    /// </summary>
    public override string ToString() => Name;

    #endregion Public

    #region IEquatable<IniDocumentRecord>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(IniDocumentRecord other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return Comparer == other.Comparer &&
             Comparer.Equals(this.Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as IniDocumentRecord);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => Comparer.GetHashCode(Name);

    #endregion IEquatable<IniDocumentRecord>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ini Document
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class IniDocument 
    : IReadOnlyDictionary<string, IniDocumentSection> {

    #region Private Data

    private Dictionary<string, IniDocumentSection> m_Sections;

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

    internal void CoreAddSection(IniDocumentSection section) {
      if (null == section)
        return;
      else if (section.Document == this)
        return;

      if (section.Document != null)
        section.Document.CoreRemoveSection(section);

      if (m_Sections.TryGetValue(section.Name, out var old))
        CoreRemoveSection(old);

      m_Sections.Add(section.Name, section);
      section.Document = this;

      section.CoreUpdateRecords();
    }

    internal void CoreRemoveSection(IniDocumentSection section) {
      if (null == section)
        return;
      else if (section.Document != null)
        return;

      if (m_Sections.TryGetValue(section.Name, out var actual) && actual == section) {
        section.Document = null;
        m_Sections.Remove(section.Name);
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public IniDocument(StringComparer comparer, IniFileCommentKind commentKind) {
      if (null == comparer)
        comparer = StringComparer.OrdinalIgnoreCase;

      m_Sections = new Dictionary<string, IniDocumentSection>(comparer);
      CommentKind = commentKind;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public IniDocument(StringComparer comparer) : this(comparer, IniFileCommentKind.Standard) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public IniDocument() : this(StringComparer.OrdinalIgnoreCase, IniFileCommentKind.Standard) { }

    /// <summary>
    /// Load from Lines
    /// </summary>
    public static IniDocument Load(IEnumerable<string> lines, StringComparer comparer) {
      IniDocument result = new IniDocument(comparer);

      IniDocumentSection section = null;
      List<string> comments = new List<string>();

      int index = 0;

      foreach (string line in lines) {
        index += 1;

        if (string.IsNullOrWhiteSpace(line))
          continue;

        if (IniFileComment.TryParse(line, out var iniComment)) {
          comments.Add(iniComment.Value);
        }
        else if (IniFileSection.TryParse(line, out var iniSection)) {
          string commentText = string.Join(Environment.NewLine, comments);

          if (result.TryGetSection(iniSection.Value, out section)) {
            if (!string.IsNullOrEmpty(commentText)) {
              if (string.IsNullOrEmpty(section.Comments))
                section.Comments = commentText;
              else
                section.Comments = string.Join(Environment.NewLine, section.Comments, commentText);
            }
          }
          else {
            section = new IniDocumentSection(iniSection.Value, result);
            section.Comments = commentText;
          }

          comments.Clear();
        }
        else if (IniFileRecord.TryParse(line, out var iniRecord)) {
          if (null == section)
            section = new IniDocumentSection("", result);

          section.AddRecord(iniRecord.Name, iniRecord.Value).Comments = string.Join(Environment.NewLine, comments);

          comments.Clear();
        }
        else
          throw new FormatException($"Syntax error in line {index}");
      }

      return result;
    }

    /// <summary>
    /// Load from Lines
    /// </summary>
    public static IniDocument Load(IEnumerable<string> lines) => Load(lines, StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Load From File
    /// </summary>
    public static IniDocument LoadFromFile(string fileName, Encoding encoding, StringComparer comparer) {
      IniDocument result = Load(File.ReadLines(fileName, encoding), comparer);

      result.FileName = fileName;
      result.FileEncoding = encoding;

      return result;
    }

    /// <summary>
    /// Load From File
    /// </summary>
    public static IniDocument LoadFromFile(string fileName, Encoding encoding) {
      return LoadFromFile(fileName, encoding, null);
    }

    /// <summary>
    /// Load From File
    /// </summary>
    public static IniDocument LoadFromFile(string fileName, StringComparer comparer) {
      return LoadFromFile(fileName, Encoding.UTF8, comparer);
    }

    /// <summary>
    /// Load From File
    /// </summary>
    public static IniDocument LoadFromFile(string fileName) {
      return LoadFromFile(fileName, Encoding.UTF8, null);
    }

    /// <summary>
    /// Save To file
    /// </summary>
    public void SaveToFile(string fileName, Encoding encoding) {
      File.WriteAllLines(fileName, Lines, encoding);
    }

    /// <summary>
    /// Save To file
    /// </summary>
    public void SaveToFile(string fileName) => SaveToFile(fileName, FileEncoding);

    /// <summary>
    /// Save To file
    /// </summary>
    public void SaveToFile() => SaveToFile(FileName, FileEncoding);

    #endregion Create

    #region Public

    /// <summary>
    /// File Name
    /// </summary>
    public string FileName { get; private set; } = "";

    /// <summary>
    /// File Encoding
    /// </summary>
    public Encoding FileEncoding { get; private set; } = Encoding.UTF8;

    /// <summary>
    /// Comment Kind
    /// </summary>
    public IniFileCommentKind CommentKind { get; }

    /// <summary>
    /// Comparer
    /// </summary>
    public StringComparer Comparer { get; private set; } = StringComparer.OrdinalIgnoreCase;

    /// <summary>
    /// Add Section
    /// </summary>
    public IniDocumentSection AddSection(string name) {
      if (name == null)
        throw new ArgumentNullException(nameof(name));

      if (m_Sections.TryGetValue(name, out var result))
        return result;

      return new IniDocumentSection(name, this);
    }

    /// <summary>
    /// Add Section
    /// </summary>
    public IniDocumentSection AddSection(IniDocumentSection section) {
      if (null == section)
        throw new ArgumentNullException(nameof(section));

      CoreAddSection(section);

      return section;
    }

    /// <summary>
    /// Add Sections
    /// </summary>
    public IniDocument AddSections(IEnumerable<IniDocumentSection> sections) {
      if (null == sections)
        throw new ArgumentNullException(nameof(sections));

      foreach (IniDocumentSection section in sections)
        AddSection(section);

      return this;
    }

    /// <summary>
    /// Add Sections
    /// </summary>
    public IniDocument AddSections(params IniDocumentSection[] sections) {
      if (null == sections)
        throw new ArgumentNullException(nameof(sections));

      foreach (IniDocumentSection section in sections)
        AddSection(section);

      return this;
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      var sections = m_Sections.Values.ToList();

      foreach (var section in sections) 
        CoreRemoveSection(section);

      m_Sections.Clear();  
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(string sectionName) {
      if (m_Sections.TryGetValue(sectionName, out var section)) {
        CoreRemoveSection(section);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(string sectionName, string recordName) {
      sectionName ??= "";
      recordName ??= "";

      return m_Sections.TryGetValue(sectionName, out var section) &&
             section.Remove(recordName);   
    }

    /// <summary>
    /// Try Get Section
    /// </summary>
    public bool TryGetSection(string name, out IniDocumentSection section) => m_Sections.TryGetValue(name, out section);

    /// <summary>
    /// Try Get Value
    /// </summary>
    public bool TryGetValue(string sectionName, string recordName, out string value) {
      value = null;

      return m_Sections.TryGetValue(sectionName, out var section) &&
             section.TryGetValue(recordName, out value);
    }

    /// <summary>
    /// Read / Write values
    /// </summary>
    public string this[string sectionName, string recordName] {
      get {
        return TryGetValue(sectionName, recordName, out var result)
          ? result
          : null;
      }
      set {
        if (null == value)
          Remove(sectionName, recordName);
        else {
          IniDocumentSection section = AddSection(sectionName);

          section.AddRecord((recordName, value));
        }
      }
    }

    /// <summary>
    /// Lines
    /// </summary>
    public IEnumerable<string> Lines {
      get {
        bool first = true;

        foreach (IniDocumentSection section in m_Sections.Values) {
          if (!first)
            yield return "";

          first = false;

          foreach (string comment in section.Comments.SplitToLines())
            yield return comment;

          yield return $"[{section.Name}]";

          if (section.Count <= 0)
            continue;

          yield return "";

          foreach (IniDocumentRecord record in section.Records) {
            foreach (string comment in record.Comments.SplitToLines())
              yield return comment;

            yield return $"{EncodeName(record.Name)}={EncodeValue(record.Value)}";
          }
        }
      }
    }

    /// <summary>
    /// Sections
    /// </summary>
    public IEnumerable<IniDocumentSection> Sections => m_Sections.Values;

    /// <summary>
    /// Records
    /// </summary>
    public IEnumerable<IniDocumentRecord> Records => m_Sections.Values.SelectMany(section => section.Records);

    #endregion Public

    #region IReadOnlyDictionary<string, IniDocumentSection>

    /// <summary>
    /// Keys
    /// </summary>
    public IEnumerable<string> Keys => m_Sections.Keys;

    /// <summary>
    /// Values
    /// </summary>
    public IEnumerable<IniDocumentSection> Values => m_Sections.Values;

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Sections.Count;

    /// <summary>
    /// Section
    /// </summary>
    public IniDocumentSection this[string key] => m_Sections[key];

    /// <summary>
    /// Conatins Key
    /// </summary>
    bool IReadOnlyDictionary<string, IniDocumentSection>.ContainsKey(string key) => m_Sections.ContainsKey(key);

    /// <summary>
    /// Try Get Value
    /// </summary>
    bool IReadOnlyDictionary<string, IniDocumentSection>.TryGetValue(string key, out IniDocumentSection value) => m_Sections.TryGetValue(key, out value);

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<KeyValuePair<string, IniDocumentSection>> GetEnumerator() => m_Sections.GetEnumerator();

    /// <summary>
    /// Enumeartor
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Sections.GetEnumerator();

    #endregion IReadOnlyDictionary<string, IniDocumentSection>
  }

}
