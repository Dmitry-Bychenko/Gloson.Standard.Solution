using System;
using System.Collections;
using System.Collections.Generic;

namespace Gloson.Text.Parsing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Rules (read-only)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface ITokenDescriptionRules : IReadOnlyList<TokenDescription> {
    /// <summary>
    /// Match
    /// </summary>
    /// <param name="line">Line to find at</param>
    /// <param name="startAt">Start at</param>
    /// <param name="context">Context</param>>
    /// <returns>Match</returns>
    TokenDescription.TokenDescriptionMatch Match(string line,
                                                 int startAt,
                                                 IReadOnlyList<Token> context);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Description Rules Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TokenDescriptionRulesExtensions {
    #region Public

    /// <summary>
    /// Parse
    /// </summary>
    /// <param name="rule">Rule to use</param>
    /// <param name="source">Source lines</param>
    /// <returns>Tokens</returns>
    public static IEnumerable<Token> Parse(this ITokenDescriptionRules rule, IEnumerable<string> source) {
      if (null == rule)
        throw new ArgumentNullException(nameof(rule));

      return Tokenizer.Parse(source, rule);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Description rules
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class TokenDescriptionRules : ITokenDescriptionRules {
    #region Private Data

    // Items
    private readonly List<TokenDescription> m_Items = new List<TokenDescription>();
    // Is updated
    private bool m_Updated = false;

    #endregion Private Data

    #region Algorithm

    // Add new rule
    private void CoreAdd(TokenDescription item) {
      if (null == item)
        return;

      if (m_Items.Contains(item))
        return;

      m_Items.Add(item);

      m_Updated = true;
    }

    // Update (sort)
    private void CoreUpdate() {
      if (!m_Updated)
        return;

      m_Items.Sort((left, right) => left.Priority.CompareTo(right.Priority));

      m_Updated = false;
    }

    // Try match entire 
    private TokenDescription.TokenDescriptionMatch CoreEntireMatch(string line,
                                                                   int startAt,
                                                                   IReadOnlyList<Token> context) {
      // Entire
      foreach (var item in m_Items)
        if (item.TryMatchEntire(line, startAt, context, out var match))
          return match;

      return TokenDescription.TokenDescriptionMatch.EmptyMatch;
    }

    // Try match at least start
    private TokenDescription.TokenDescriptionMatch CoreStartMatch(string line,
                                                                  int startAt,
                                                                  IReadOnlyList<Token> context) {
      // Starts
      foreach (var item in m_Items)
        if (item.TryMatchStart(line, startAt, context, out var match)) {
          // No Stop match, just return the start
          if (!item.TryMatchStop(line, startAt + match.Length, context, out var matchStop, match.Extract(line))) // + 1 removed !!!!!!
            return match;

          // Let's combine start and stop into entire
          TokenDescription.TokenDescriptionMatch matchEntire =
            new TokenDescription.TokenDescriptionMatch(match.Description, match.From, matchStop.To, TokenMatchKind.Entire);

          return matchEntire;
        }

      return TokenDescription.TokenDescriptionMatch.EmptyMatch;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public TokenDescriptionRules() : base() {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<TokenDescription> Items {
      get {
        CoreUpdate();

        return m_Items;
      }
    }

    /// <summary>
    /// Add a description
    /// </summary>
    public void Add(TokenDescription value) {
      if (null == value)
        throw new ArgumentNullException(nameof(value));

      CoreAdd(value);
    }

    /// <summary>
    /// Add range of descriptions
    /// </summary>
    public void AddRange(IEnumerable<TokenDescription> values) {
      if (null == values)
        throw new ArgumentNullException(nameof(values));

      foreach (var value in values)
        CoreAdd(value);
    }

    /// <summary>
    /// Remove at
    /// </summary>
    public void RemoveAt(int index) {
      m_Items.RemoveAt(index);
    }

    /// <summary>
    /// Clear
    /// </summary>
    public void Clear() {
      m_Items.Clear();

      m_Updated = false;
    }

    /// <summary>
    /// Update
    /// </summary>
    public void Update() {
      CoreUpdate();
    }

    /// <summary>
    /// Match (start or entire)
    /// </summary>
    public TokenDescription.TokenDescriptionMatch Match(string line,
                                                        int startAt,
                                                        IReadOnlyList<Token> context) {
      if (string.IsNullOrEmpty(line))
        return TokenDescription.TokenDescriptionMatch.EmptyMatch;
      else if (startAt >= line.Length)
        return TokenDescription.TokenDescriptionMatch.EmptyMatch;

      if (null == context)
        context = new List<Token>();

      CoreUpdate();

      TokenDescription.TokenDescriptionMatch match1 = CoreEntireMatch(line, startAt, context);
      TokenDescription.TokenDescriptionMatch match2 = CoreStartMatch(line, startAt, context);

      if (!match1.IsMatch)
        return match2;
      else if (!match2.IsMatch)
        return match1;

      if (match1.Description.Priority < match2.Description.Priority)    // was < ??? 
        return match1;
      else
        return match2;
    }

    #endregion Public

    #region IReadOnlyList<TokenDescription

    /// <summary>
    /// Indexer
    /// </summary>
    public TokenDescription this[int index] {
      get {
        CoreUpdate();

        return m_Items[index];
      }
    }

    /// <summary>
    /// Count
    /// </summary>
    public int Count {
      get {
        CoreUpdate();

        return m_Items.Count;
      }
    }

    /// <summary>
    /// Enumerator
    /// </summary>
    public IEnumerator<TokenDescription> GetEnumerator() {
      CoreUpdate();

      return m_Items.GetEnumerator();
    }

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() {
      CoreUpdate();

      return m_Items.GetEnumerator();
    }

    #endregion IReadOnlyList<TokenDescription
  }
}
