using System;
using System.Collections.Generic;

namespace Gloson.Text.Parsing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token description options
  /// </summary>
  //
  //------------------------------------------------------------------------------------------------------------------- 

  [Flags]
  public enum TokenDescriptionOptions {
    /// <summary>
    /// None
    /// </summary>
    None = 0,
    /// <summary>
    /// Ignore case
    /// </summary>
    IgnoreCase = 1,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Match Kind
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum TokenMatchKind {
    /// <summary>
    /// None or any
    /// </summary>
    None = 0,
    /// <summary>
    /// Entire token match
    /// </summary>
    Entire = 1,
    /// <summary>
    /// Start token match
    /// </summary>
    Start = 2,
    /// <summary>
    /// Stop token match
    /// </summary>
    Stop = 3
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Describes the token
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class TokenDescription {
    #region Internal classes

    // Location delegate
    public delegate Tuple<int, int> Locator(string source, int startPosition);
    // Location delegate
    public delegate Tuple<int, int> LocatorStop(string source, int startPosition, string prefix);

    /// <summary>
    /// Description match
    /// </summary>
    public struct TokenDescriptionMatch {
      #region Prviate data
      #endregion Prviate data 

      #region Create

      /// <summary>
      /// Standard constructor
      /// </summary>
      public TokenDescriptionMatch(TokenDescription description, int from, int to, TokenMatchKind kind) {
        Description = description;
        From = from;
        To = to;
        Kind = kind;
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Empty match
      /// </summary>
      public static readonly TokenDescriptionMatch EmptyMatch = new TokenDescriptionMatch(null, -1, -1, TokenMatchKind.None);

      /// <summary>
      /// Description
      /// </summary>
      public TokenDescription Description {
        get;
        private set;
      }

      /// <summary>
      /// From matching (including)
      /// </summary>
      public int From {
        get;
        private set;
      }

      /// <summary>
      /// Stop matching (excluding)
      /// </summary>
      public int To {
        get;
        private set;
      }

      /// <summary>
      /// Kind
      /// </summary>
      public TokenMatchKind Kind {
        get;
        private set;
      }

      /// <summary>
      /// Length
      /// </summary>
      public int Length {
        get {
          if (From <= 0) {
            if (To < 0)
              return 0;
            else
              return To; // To - 1 !!!!!!!!!!!!!!
          }
          else if (To < 0)
            return short.MaxValue;
          else if (To <= From)
            return 0;
          else
            return To - From;
        }
      }

      /// <summary>
      /// Is Match
      /// </summary>
      public bool IsMatch {
        get {
          if (From >= 0 && To > 0)
            return To > From;

          return From >= 0 || To > 0;
        }
      }

      /// <summary>
      /// Extract
      /// </summary>
      public string Extract(string source) {
        if (!IsMatch)
          return null;

        int start = From <= 0 ? 0 : From;
        int stop = To > source.Length ? source.Length : To; // ????

        if (To < 0)
          return source[start..];
        else
          return source.Substring(start, stop - start);
      }

      #endregion Public
    }

    #endregion Internal classes

    #region Constants

    /// <summary>
    /// Default priority
    /// </summary>
    public const int DefaultPriority = 1000;

    #endregion Constants

    #region Private Data

    // Title (debug only)
    private string m_Title = "Unknown";

    #endregion Private Data

    #region Algorithm

    /// <summary>
    /// Match start sequence
    /// </summary>
    /// <returns>start:length</returns>
    protected abstract Tuple<int, int> CoreMatchStart(string source,
                                                      int checkAt,
                                                      IReadOnlyList<Token> context);

    /// <summary>
    /// Match stop sequence
    /// </summary>
    /// <returns>start:length</returns>
    protected abstract Tuple<int, int> CoreMatchStop(string source,
                                                     int startAt,
                                                     string prefix,
                                                     IReadOnlyList<Token> context);

    /// <summary>
    /// Match entire sequence
    /// </summary>
    /// <returns>start:length</returns>
    protected abstract Tuple<int, int> CoreMatchEntire(string source,
                                                       int checkAt,
                                                       IReadOnlyList<Token> context);

    /// <summary>
    /// Is in Start/Stop mode 
    /// </summary>
    protected abstract bool CoreIsStartStopMode();

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    protected TokenDescription(int priority,
                               TokenDescriptionOptions options,
                               TokenClassification classification = TokenClassification.Unknown) {
      Priority = priority;
      Options = options;
      Classification = classification;
    }

    /// <summary>
    /// Create
    /// </summary>
    public static TokenDescription Create(string startPattern,
                                          string stopPattern,
                                          int priority = DefaultPriority,
                                          TokenDescriptionOptions options = TokenDescriptionOptions.None,
                                          TokenClassification classification = TokenClassification.Unknown) {
      return new TokenDescriptionRegex(startPattern, stopPattern, priority, options) {
        Classification = classification
      };
    }

    /// <summary>
    /// Create
    /// </summary>
    public static TokenDescription Create(string entirePattern,
                                          int priority = DefaultPriority,
                                          TokenDescriptionOptions options = TokenDescriptionOptions.None,
                                          TokenClassification classification = TokenClassification.Unknown) {
      return new TokenDescriptionRegex(entirePattern, priority, options) {
        Classification = classification
      };
    }

    /// <summary>
    /// Create
    /// </summary>
    public static TokenDescription Create(TokenDescriptionFunc.Locator startLocator,
                                          TokenDescriptionFunc.LocatorStop stopLocator,
                                          int priority = DefaultPriority,
                                          TokenDescriptionOptions options = TokenDescriptionOptions.None,
                                          TokenClassification classification = TokenClassification.Unknown) {
      return new TokenDescriptionFunc(startLocator, stopLocator, priority, options) {
        Classification = classification
      };
    }

    /// <summary>
    /// Create
    /// </summary>
    public static TokenDescription Create(TokenDescriptionFunc.Locator entireLocator,
                                          int priority = DefaultPriority,
                                          TokenDescriptionOptions options = TokenDescriptionOptions.None,
                                          TokenClassification classification = TokenClassification.Unknown) {
      return new TokenDescriptionFunc(entireLocator, priority, options) {
        Classification = classification
      };
    }

    /// <summary>
    /// Create
    /// </summary>
    public static TokenDescription Create(TokenKeyWords keyWords,
                                          int priority = DefaultPriority - 200,
                                          TokenClassification classification = TokenClassification.Unknown) {
      return new TokenDescriptionKeyWords(keyWords, priority) {
        Classification = classification
      };
    }

    /// <summary>
    /// Create
    /// </summary>
    public static TokenDescription Create(IEnumerable<string> words,
                                          int priority = DefaultPriority,
                                          TokenDescriptionOptions options = TokenDescriptionOptions.None,
                                          TokenClassification classification = TokenClassification.Unknown) {
      bool caseSensitive = !((options & TokenDescriptionOptions.IgnoreCase) == TokenDescriptionOptions.IgnoreCase);
      TokenKeyWords keyWords = new TokenKeyWords(caseSensitive, words);

      return new TokenDescriptionKeyWords(keyWords, priority) {
        Classification = classification
      };
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Is White Space (commentary)
    /// </summary>
    public bool IsWhiteSpace {
      get {
        return Classification == TokenClassification.WhiteSpace ||
               Classification == TokenClassification.Pragma;
      }
    }

    public TokenClassification Classification {
      get;
      private set;
    }

    /// <summary>
    /// Title (debug only)
    /// </summary>
    public string Title {
      get {
        return m_Title;
      }
      set {
        if (string.IsNullOrWhiteSpace(value))
          value = "unknown";

        m_Title = value;
      }
    }

    /// <summary>
    /// Priority
    /// </summary>
    public int Priority {
      get;
      private set;
    }

    /// <summary>
    /// Options
    /// </summary>
    public TokenDescriptionOptions Options {
      get;
      private set;
    }

    /// <summary>
    /// Can perform entire match
    /// </summary>
    public bool CanEntireMatch {
      get {
        return !CoreIsStartStopMode();
      }
    }

    /// <summary>
    /// Try match start
    /// </summary>
    public bool TryMatchStart(string source,
                              int checkAt,
                              IReadOnlyList<Token> context,
                              out TokenDescriptionMatch match) {
      match = TokenDescriptionMatch.EmptyMatch;

      if (string.IsNullOrEmpty(source))
        return false;
      else if (checkAt < 0 || checkAt >= source.Length)
        return false;

      Tuple<int, int> location = CoreMatchStart(source, checkAt, context);

      if (location.Item1 == checkAt && location.Item2 > 0) {
        match = new TokenDescriptionMatch(this, location.Item1, location.Item2, TokenMatchKind.Start);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Try match stop
    /// </summary>
    public bool TryMatchStop(string source,
                             int startAt,
                             IReadOnlyList<Token> context,
                             out TokenDescriptionMatch match,
                             string prefix) {
      match = TokenDescriptionMatch.EmptyMatch;

      if (string.IsNullOrEmpty(source))
        return false;
      else if (startAt < 0 || startAt >= source.Length)
        return false;

      Tuple<int, int> location = CoreMatchStop(source, startAt, prefix, context);

      if (location.Item1 >= startAt && location.Item2 > 0) {
        match = new TokenDescriptionMatch(this, location.Item1, location.Item2, TokenMatchKind.Stop);

        return true;
      }

      return false;
    }

    /// <summary>
    /// Try match entire
    /// </summary>
    public bool TryMatchEntire(string source,
                               int checkAt,
                               IReadOnlyList<Token> context,
                               out TokenDescriptionMatch match) {
      match = TokenDescriptionMatch.EmptyMatch;

      if (string.IsNullOrEmpty(source))
        return false;
      else if (checkAt < 0 || checkAt >= source.Length)
        return false;

      Tuple<int, int> location = CoreMatchEntire(source, checkAt, context);

      if (location.Item1 == checkAt && location.Item2 > 0) {
        match = new TokenDescriptionMatch(this, location.Item1, location.Item2, TokenMatchKind.Entire);

        return true;
      }

      return false;
    }

    /// <summary>
    /// To String (Title)
    /// </summary>
    public override string ToString() {
      return m_Title;
    }

    #endregion Public
  }
}
