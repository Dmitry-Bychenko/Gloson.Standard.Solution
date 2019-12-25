using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

using Gloson.Text.RegularExpressions;

namespace Gloson.UI.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Type
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum CommandLineType {
    None = 0,
    Boolean = 1,
    Integer = 2,
    FloatingPoint = 3,
    Date = 4,
    Time = 5,
    DateTime = 6,
    String = 7,

    File = 8,
    Directory = 9,
    FileOrDirectory = 10,
    ExistingFile = 11,
    ExistingDirectory = 12,
    ExistingFileOrDirectory = 13
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Descriptions Options
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [Flags]
  public enum CommandLineDescriptorsOptions {
    None = 0,
    CaseSensitive = 1,
    OptionalPrefix = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Descriptors
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class CommandLineDescriptors : IReadOnlyList<CommandLineDescriptor> {
    #region Private Data

    private List<CommandLineDescriptor> m_Items = new List<CommandLineDescriptor>();

    #endregion Private Data

    #region Algorithm

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="caseSensitive">If case Sensitive</param>
    public CommandLineDescriptors(CommandLineDescriptorsOptions options = CommandLineDescriptorsOptions.None) {
      Options = options;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static CommandLineDescriptors Default { get; } = new CommandLineDescriptors();

    /// <summary>
    /// Items (Descriptors)
    /// </summary>
    public IReadOnlyList<CommandLineDescriptor> Items => m_Items;

    /// <summary>
    /// Options
    /// </summary>
    public CommandLineDescriptorsOptions Options { get; }

    /// <summary>
    /// Case Sensitive
    /// </summary>
    public bool CaseSensitive => Options.HasFlag(CommandLineDescriptorsOptions.CaseSensitive);

    /// <summary>
    /// Add item
    /// </summary>
    /// <param name="item"></param>
    /// <returns></returns>
    public CommandLineDescriptor Add(CommandLineDescriptor item) {
      if (null == item)
        throw new ArgumentNullException(nameof(item));

      if (item.Owner == this)
        return item;
      else if (item.Owner != null)
        throw new ArgumentException("item.Owner must be null", nameof(item));

      m_Items.Add(item);

      item.Owner = this;

      return item;
    }

    /// <summary>
    /// Validation Errors
    /// </summary>
    public IEnumerable<String> ValidationErrors {
      get {
        return m_Items
          .SelectMany(item => item
            .ValidationErrors
            .Select(error => $"{item.Name} : {error}"));
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      int nameLength = m_Items.Any() 
        ?  m_Items.Max(item => item.Name.Length) 
        : 0;

      return string.Join(Environment.NewLine, m_Items
        .OrderBy(item => item)
        .Select(item => $"{item.Name.PadRight(nameLength)} {item.Description}"));
    }

    #endregion Public

    #region IReadOnlyList<CommandLineDescriptor>

    /// <summary>
    /// Add Descriptor
    /// </summary>
    public CommandLineDescriptor Add(string name, CommandLineType valueType, string description) => 
      new CommandLineDescriptor(this) { 
        Name = name,
        ValueType = valueType,
        Description = description
      };

    /// <summary>
    /// Indexer
    /// </summary>
    public CommandLineDescriptor this[int index] => m_Items[index];

    /// <summary>
    /// Find By Name
    /// </summary>
    /// <param name="name">name</param>
    /// <returns></returns>
    public CommandLineDescriptor Find(string name) {
      if (string.IsNullOrWhiteSpace(name))
        name = "";

      return m_Items
        .Select(item => new {
          value = item,
          match = item.RegularExpression.Match(name)
        })
        .Where(item => item.match.Success)
        .OrderBy(item => item.value.MatchPriority)
        .ThenByDescending(item => item.match.Length)
        .Select(item => item.value)
        .FirstOrDefault();
    }

    /// <summary>
    /// Descriptor by its name
    /// </summary>
    public CommandLineDescriptor this[string name] {
      get {
        CommandLineDescriptor result = Find(name);

        if (null == result)
          throw new ArgumentException($"Descriptor {name} is not found", nameof(name));

        return result;
      }
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<CommandLineDescriptor> GetEnumerator() => m_Items.GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => m_Items.GetEnumerator();

    #endregion IReadOnlyList<CommandLineDescriptor> 
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Descriptor
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class CommandLineDescriptor 
    : IComparable<CommandLineDescriptor>, 
      IEquatable<CommandLineDescriptor> {

    #region Private Data

    private string m_Name = "";
    private int m_MatchPriority;

    private int m_MinCount;
    private int m_MaxCount;

    private string m_Description = "";
    private string m_HelpInfo = "";
    private string m_DefaultValue = null;

    private Regex m_RegularExpression;

    #endregion Private Data

    #region Algorithm

    private Regex BuildRegular() {
      string patternBody = RegularExpressionBuilder.FromOptionsPattern(Name);

      string countMarker = Owner.Options.HasFlag(CommandLineDescriptorsOptions.OptionalPrefix)
        ? "*"
        : "+";

      string pattrenPrefix = 
        $"[\\s{string.Concat(CommandLineHelper.CommandLinePrefixes.Select(c => Regex.Escape(c.ToString())))}]{countMarker}";

      string pattern = string.Concat(
        "^",
        pattrenPrefix,
       $"(?<name>{patternBody})",
        pattrenPrefix
      );

      RegexOptions options = RegexOptions.None;

      if (!CaseSensitive)
        options |= RegexOptions.IgnoreCase;

      return new Regex(pattern, options);
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public CommandLineDescriptor(CommandLineDescriptors owner) {
      if (null == owner)
        throw new ArgumentNullException(nameof(owner));

      owner.Add(this);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Compare
    /// </summary>
    public static int Compare(CommandLineDescriptor left, CommandLineDescriptor right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(null, left))
        return -1;
      else if (ReferenceEquals(null, right))
        return 1;

      int result = string.Compare(left.Name, right.Name, StringComparison.OrdinalIgnoreCase);

      if (result != 0)
        return result;

      return string.Compare(left.Name, right.Name, StringComparison.Ordinal);
    }

    /// <summary>
    /// Owner
    /// </summary>
    public CommandLineDescriptors Owner { get; internal set; }

    /// <summary>
    /// Name
    /// </summary>
    public string Name {
      get {
        return m_Name;
      }
      set {
        if (string.IsNullOrWhiteSpace(value))
          m_Name = "";
        else
          m_Name = CommandLineHelper.Normalize(value);

        m_RegularExpression = null;
      }
    }

    /// <summary>
    /// Value Type
    /// </summary>
    public CommandLineType ValueType { get; set; }

    /// <summary>
    /// Min Count
    /// </summary>
    public int MinCount {
      get => m_MinCount;
      set {
        if (value < 0)
          throw new ArgumentNullException(nameof(value));

        m_MinCount = value;
      }

    }

    /// <summary>
    /// Max Count
    /// </summary>
    public int MaxCount {
      get => m_MaxCount;
      set {
        if (value < 0)
          throw new ArgumentNullException(nameof(value));

        m_MaxCount = value;
      }

    }

    /// <summary>
    /// Match Priority
    /// </summary>
    public int MatchPriority { 
      get => string.IsNullOrWhiteSpace(Name) ? int.MaxValue : m_MatchPriority; 
      set { m_MatchPriority = value; } 
    }

    /// <summary>
    /// Description
    /// </summary>
    public string Description {
      get => m_Description;
      set {
        m_Description = string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
      }
    }

    /// <summary>
    /// Default Value
    /// </summary>
    public string DefaultValue {
      get => m_DefaultValue;
      set {
        m_DefaultValue = string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
      }
    }

    /// <summary>
    /// Help Info
    /// </summary>
    public string HelpInfo {
      get => m_HelpInfo; 
      set {
        m_HelpInfo = string.IsNullOrWhiteSpace(value) ? "" : value.Trim();
      } 
    }

    /// <summary>
    /// Extra Validator
    /// </summary>
    public Func<CommandLineDescriptor, IEnumerable<string>> ExtraValidator { get; set; }

    /// <summary>
    /// Is Required
    /// </summary>
    public bool IsRequired => MinCount > 0;

    /// <summary>
    /// Case Sensitive
    /// </summary>
    public bool CaseSensitive => Owner == null ? false : Owner.CaseSensitive;

    /// <summary>
    /// Regular Expression To Match
    /// </summary>
    public Regex RegularExpression {
      get {
        if (null == m_RegularExpression)
          m_RegularExpression = BuildRegular();

        return m_RegularExpression;
      }
    }

    /// <summary>
    /// Validation Errors
    /// </summary>
    public IEnumerable<string> ValidationErrors { 
      get {
        if (MinCount > MaxCount)
          yield return $"MaxCount > MinCount";

        var extra = ExtraValidator;

        IEnumerable<string> data = extra == null
          ? null
          : extra(this);

        if (null != data)
          foreach (string line in data)
            if (!string.IsNullOrWhiteSpace(line))
              yield return line;
      }
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      int length = Owner == null ? 0 : Owner.Items.Max(item => item.Name.Length);

      var lines = new string[] {
        Name.PadRight(length),
        IsRequired ? "(*)" : "",
        Description
      };

      return string.Join(" ", lines
        .Where(item => !string.IsNullOrWhiteSpace(item))
        .Select(item => item.Trim()));
    }

    #endregion Public

    #region IComparable<CommandLineDescriptor>

    /// <summary>
    /// Compare To
    /// </summary>
    public int CompareTo(CommandLineDescriptor other) => Compare(this, other);

    #endregion IComparable<CommandLineDescriptor>

    #region IEquatable<CommandLineDescriptor>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(CommandLineDescriptor other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      if (!ReferenceEquals(Owner, other.Owner))
        return false;

      return CaseSensitive
        ? string.Equals(Name, other.Name, StringComparison.Ordinal)
        : string.Equals(Name, other.Name, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as CommandLineDescriptor);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return Name == null
        ? 0
        : Name.ToUpperInvariant().GetHashCode();
    }

    #endregion IEquatable<CommandLineDescriptor>
  }
}
