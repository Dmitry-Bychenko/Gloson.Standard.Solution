using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gloson.Text.Parsing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Key words support
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TokenKeyWords : IReadOnlyList<string>, IEquatable<TokenKeyWords> {
    #region Internal classes

    /// <summary>
    /// If letter can be used in identifier 
    /// </summary>
    /// <param name="value">letter to check</param>
    /// <returns>can be used within identifier</returns>
    public delegate bool IsIdentifierLetter(char value);

    #endregion Internal classes

    #region Private Data

    // Is identifier
    private IsIdentifierLetter m_IsIdentifierLetter;
    // Is case sensitive
    private bool m_IsCaseSensitive;
    // KeyWords
    private List<string> m_Items = new List<string>();

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenKeyWords(IsIdentifierLetter identifier, bool isCaseSensitive, IEnumerable<string> keyWords)
      : base() {

      if (null == keyWords)
        throw new ArgumentNullException("keyWords");

      m_IsIdentifierLetter = identifier == null ? IsStandardIdentifier : identifier;
      m_IsCaseSensitive = isCaseSensitive;

      HashSet<string> hs = new HashSet<string>(isCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

      foreach (var line in keyWords) {
        if (string.IsNullOrWhiteSpace(line))
          continue;

        hs.Add(line);
      }

      m_Items.AddRange(hs);

      m_Items.Sort((left, right) => -left.Length.CompareTo(right.Length));
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenKeyWords(IsIdentifierLetter identifier, IEnumerable<string> keyWords)
      : this(identifier, true, keyWords) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenKeyWords(bool isCaseSensitive, IEnumerable<string> keyWords)
      : this(null, isCaseSensitive, keyWords) {
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenKeyWords(IEnumerable<string> keyWords)
      : this(null, true, keyWords) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Is standard identifier
    /// </summary>
    public static bool IsStandardIdentifier(char value) {
      return char.IsLetterOrDigit(value) || (value == '_');
    }

    /// <summary>
    /// Is case sensitive
    /// </summary>
    public bool IsCaseSensitive {
      get {
        return m_IsCaseSensitive;
      }
    }

    /// <summary>
    /// Identifier detector
    /// </summary>
    public IsIdentifierLetter IdentifierDetector {
      get {
        return m_IsIdentifierLetter;
      }
    }

    /// <summary>
    /// Keyword at
    /// </summary>
    /// <param name="source">source string</param>
    /// <param name="startAt">start index at</param>
    /// <returns>keyword or null</returns>
    public string KeyWordAt(string source, int startAt) {
      if (string.IsNullOrWhiteSpace(source))
        return null;

      if ((startAt < 0) || (startAt > source.Length))
        return null;

      string chunk = source.Substring(startAt);

      StringComparison comparison = m_IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

      foreach (string item in m_Items)
        if (chunk.StartsWith(item, comparison)) {
          if (item.Length >= chunk.Length)
            return item;

          char letter = chunk[item.Length];

          if (!m_IsIdentifierLetter(letter))
            return item;
        }

      return null;
    }

    /// <summary>
    /// Is Key word
    /// </summary>
    public bool IsKeyWord(string value) {
      if (string.IsNullOrEmpty(value))
        return false;

      StringComparison comparison = m_IsCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;

      foreach (string item in m_Items)
        if (item.Equals(value, comparison))
          return true;

      return false;
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      var list = m_Items.ToList();

      list.Sort((left, right) => {
        int v = string.Compare(left, right, StringComparison.OrdinalIgnoreCase);

        if (v != 0)
          return v;

        return string.Compare(left, right, StringComparison.Ordinal);
      }
      );

      return string.Join(", ", list.Select(item => item).Take(10)) + "...";
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(TokenKeyWords left, TokenKeyWords right) {
      if (object.ReferenceEquals(left, right))
        return true;
      else if (object.ReferenceEquals(left, null))
        return false;
      else if (object.ReferenceEquals(null, right))
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(TokenKeyWords left, TokenKeyWords right) {
      if (object.ReferenceEquals(left, right))
        return false;
      else if (object.ReferenceEquals(left, null))
        return true;
      else if (object.ReferenceEquals(null, right))
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region IReadOnlyList<string>

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<string> GetEnumerator() {
      return m_Items.GetEnumerator();
    }

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
      return GetEnumerator();
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count {
      get {
        return m_Items.Count;
      }
    }

    /// <summary>
    /// Ietms
    /// </summary>
    public string this[int index] {
      get {
        return m_Items[index];
      }
    }

    #endregion IReadOnlyList<string>

    #region IEquatable<TokenKeyWords>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(TokenKeyWords other) {
      if (object.ReferenceEquals(this, other))
        return true;
      else if (object.ReferenceEquals(null, other))
        return false;

      if (m_IsCaseSensitive != other.m_IsCaseSensitive)
        return false;
      else if (m_Items.Count != other.m_Items.Count)
        return false;

      HashSet<string> hs = new HashSet<string>(
        m_Items,
        m_IsCaseSensitive ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase);

      if (other.m_Items.Any(item => !hs.Contains(item)))
        return false;

      unchecked {
        if (Enumerable.Range(0, char.MaxValue)
             .Any(index => m_IsIdentifierLetter((char)index) != other.m_IsIdentifierLetter((char)index)))
          return false;
      }

      return true;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return Equals(obj as TokenKeyWords);
    }

    /// <summary>
    /// Hash code
    /// </summary>
    public override int GetHashCode() {
      return (m_Items.Count << 1) | (m_IsCaseSensitive ? 1 : 0);
    }

    #endregion IEquatable<TokenKeyWords>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Description based on keywords
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TokenDescriptionKeyWords : TokenDescription {
    #region Private Data

    // Key words
    private TokenKeyWords m_KeyWords;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenDescriptionKeyWords(TokenKeyWords keyWords, int priority)
      : base(priority, keyWords.IsCaseSensitive ? TokenDescriptionOptions.None : TokenDescriptionOptions.IgnoreCase) {

      if (object.ReferenceEquals(null, keyWords))
        throw new ArgumentNullException("keyWords");

      m_KeyWords = keyWords;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenDescriptionKeyWords(TokenKeyWords keyWords) : this(keyWords, 800) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Key words
    /// </summary>
    public TokenKeyWords KeyWords {
      get {
        return m_KeyWords;
      }
    }

    /// <summary>
    /// Is Start (always false)
    /// </summary>
    protected override Tuple<int, int> CoreMatchStart(string source, int checkAt, IReadOnlyList<Token> context) {
      return new Tuple<int, int>(-1, -1);
    }

    /// <summary>
    /// Is Stop (always false)
    /// </summary>
    protected override Tuple<int, int> CoreMatchStop(string source, int startAt, string prefix, IReadOnlyList<Token> context) {
      return new Tuple<int, int>(-1, -1);
    }

    /// <summary>
    /// Try match entire
    /// </summary>
    protected override Tuple<int, int> CoreMatchEntire(string source, int checkAt, IReadOnlyList<Token> context) {
      string key = m_KeyWords.KeyWordAt(source, checkAt);

      if (null == key)
        return new Tuple<int, int>(-1, -1);

      return new Tuple<int, int>(checkAt, checkAt + key.Length);
    }

    /// <summary>
    /// Mode
    /// </summary>
    protected override bool CoreIsStartStopMode() {
      return false;
    }

    #endregion Public
  }

}
