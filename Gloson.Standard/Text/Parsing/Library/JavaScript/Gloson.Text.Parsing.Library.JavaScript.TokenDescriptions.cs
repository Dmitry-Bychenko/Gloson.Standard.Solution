using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gloson.Text.Parsing.Library.JavaScript {

  internal class RegexTokenDescription : TokenDescription {
    protected override bool CoreIsStartStopMode() {
      return false;
    }

    protected override Tuple<int, int> CoreMatchEntire(string source, int checkAt, IReadOnlyList<Token> context) {
      if (source[checkAt] != '/')
        return new Tuple<int, int>(-1, -1);

      if (checkAt < source.Length - 1)
        if (source[checkAt + 1] == '*' || source[checkAt + 1] == '/')
          return new Tuple<int, int>(-1, -1);

      var lastToken = context
       .Where(token => token.Description != JavaScriptTokenDescriptions.LineComment &&
                       token.Description != JavaScriptTokenDescriptions.MultilineComment)
       .LastOrDefault();

      var lastDesc = lastToken?.Description;

      if (lastDesc == JavaScriptTokenDescriptions.FloatingPoint ||
          lastDesc == JavaScriptTokenDescriptions.Identifier ||
          lastDesc == JavaScriptTokenDescriptions.Integer ||
          lastDesc == JavaScriptTokenDescriptions.IdentifierVariable)
        return new Tuple<int, int>(-1, -1);

      if (lastToken.Text == ")" || lastToken.Text == "]")
        return new Tuple<int, int>(-1, -1);

      for (int i = checkAt + 1; i < source.Length; ++i) {
        if (source[i] == '/' && source[i - 1] != '\\') {
          return new Tuple<int, int>(checkAt, i + 1);
        }
      }

      return new Tuple<int, int>(-1, -1);
    }

    protected override Tuple<int, int> CoreMatchStart(string source, int checkAt, IReadOnlyList<Token> context) {
      return new Tuple<int, int>(-1, -1);

      //if (source[checkAt] != '/')
      //  return new Tuple<int, int>(-1, -1);

      //if (checkAt < source.Length - 1) 
      //  if (source[checkAt + 1] == '*' || source[checkAt + 1] == '/')
      //    return new Tuple<int, int>(-1, -1);

      //var lastDesc = context
      //  .Where(token => token.Description != JavaScriptTokenDescriptions.LineComment &&
      //                  token.Description != JavaScriptTokenDescriptions.MultilineComment)
      //  .LastOrDefault()
      // ?.Description;

      //if (lastDesc == JavaScriptTokenDescriptions.FloatingPoint ||
      //    lastDesc == JavaScriptTokenDescriptions.Identifier ||
      //    lastDesc == JavaScriptTokenDescriptions.Integer ||
      //    lastDesc == JavaScriptTokenDescriptions.IdentifierVariable)
      //  return new Tuple<int, int>(-1, -1);

      //return new Tuple<int, int>(checkAt, checkAt + 1);
    }

    protected override Tuple<int, int> CoreMatchStop(string source, int startAt, string prefix, IReadOnlyList<Token> context) {
      return new Tuple<int, int>(-1, -1);

      //if (source[startAt] != '/')
      //  return new Tuple<int, int>(-1, -1);

      //if (startAt == 0)
      //  return new Tuple<int, int>(startAt, startAt + 1);
      //else if (source[startAt - 1] == '\\')
      //  return new Tuple<int, int>(-1, -1);
      //else
      //  return new Tuple<int, int>(startAt, startAt + 1);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    private RegexTokenDescription(int priority, TokenDescriptionOptions options)
      : base(priority, options) {
    }

    public RegexTokenDescription() : this(TokenDescription.DefaultPriority, TokenDescriptionOptions.None) {
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Java Script Token Descriptions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class JavaScriptTokenDescriptions {
    #region Private Data

    // Hard keywords
    private static readonly TokenKeyWords s_Hard = new TokenKeyWords(new string[] {
      "abstract", "arguments", "await", "boolean", "break", "byte", "case", "catch", "char", "class",
      "const", "continue", "debugger", "default", "delete", "do", "double", "else", "enum", "eval",
      "export", "extends", "false", "final", "finally", "float", "for", "function", "goto", "if",
      "implements", "import", "in", "instanceof", "int", "interface", "let", "long", "native", "new",
      "null", "package", "private", "protected", "public", "return", "short", "static", "super", "switch",
      "synchronized", "this", "throw", "throws", "transient", "true", "try", "typeof", "var", "void",
      "volatile", "while", "with", "yield" });

    // Hard keyword
    private static readonly TokenDescription s_HardKeyword = TokenDescription.Create(s_Hard, classification: TokenClassification.Keyword);

    // Line comment
    private static readonly TokenDescription s_LineComment = TokenDescription.Create("(//).*$", classification: TokenClassification.WhiteSpace);

    // Identifier
    private static readonly TokenDescription s_Identifier = TokenDescription.Create(@"[A-Za-z]+[A-Za-z0-9_]*", classification: TokenClassification.Identifier);

    // Identifier variable
    private static readonly TokenDescription s_IdentifierVariable = TokenDescription.Create(@"\$[A-Za-z]+[A-Za-z0-9_]*", classification: TokenClassification.Identifier);

    // String Standard
    private static readonly TokenDescription s_StringStandard = TokenDescription.Create((source, checkAt) => TryMatchString(source, checkAt), classification: TokenClassification.String);

    // Regular Expression Standard
    private static readonly TokenDescription s_RegexStandard = new RegexTokenDescription();

    // Rules
    private static TokenDescriptionRules s_Rules = new TokenDescriptionRules() {
      Default,
      HardKeyword,
      Integer,
      FloatingPoint,
      StringStandard,
      RegexStandard,
      MultilineComment,
      LineComment,
      Identifier,
      IdentifierVariable,
    };

    #endregion Private Data

    #region Algorithm

    // Try matching standard string 
    private static Tuple<int, int> TryMatchString(string source, int checkAt) {
      if (string.IsNullOrEmpty(source))
        return new Tuple<int, int>(-1, -1);

      if (checkAt >= source.Length - 1)
        return new Tuple<int, int>(-1, -1);

      char quot = source[checkAt];

      if (quot != '"' && quot != '\'')
        return new Tuple<int, int>(-1, -1);

      for (int i = checkAt + 1; i < source.Length; ++i)
        if (source[i] == quot)
          return new Tuple<int, int>(checkAt, i + 1);
        else if (source[i] == '\\')
          i += 1;

      return new Tuple<int, int>(-1, -1);
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Rules
    /// </summary>
    public static TokenDescriptionRules Rules {
      get {
        return s_Rules;
      }
    }

    /// <summary>
    /// Default
    /// </summary>
    public static TokenDescription Default {
      get {
        return TokenDescriptionLibrary.Default;
      }
    }

    /// <summary>
    /// Hard keyword
    /// </summary>
    public static TokenDescription HardKeyword {
      get {
        return s_HardKeyword;
      }
    }

    /// <summary>
    /// Integer
    /// </summary>
    public static TokenDescription Integer {
      get {
        return TokenDescriptionLibrary.Integer;
      }
    }

    /// <summary>
    /// Floating Point
    /// </summary>
    public static TokenDescription FloatingPoint {
      get {
        return TokenDescriptionLibrary.FloatingPoint;
      }
    }

    /// <summary>
    /// String
    /// </summary>
    public static TokenDescription StringStandard {
      get {
        return s_StringStandard;
      }
    }

    /// <summary>
    /// Regex
    /// </summary>
    public static TokenDescription RegexStandard {
      get {
        return s_RegexStandard;
      }
    }

    /// <summary>
    /// Multiline comment
    /// </summary>
    public static TokenDescription MultilineComment {
      get {
        return TokenDescriptionLibrary.CommentMultilineComment;
      }
    }

    /// <summary>
    /// Line comment
    /// </summary>
    public static TokenDescription LineComment {
      get {
        return s_LineComment;
      }
    }

    /// <summary>
    /// Identifier
    /// </summary>
    public static TokenDescription Identifier {
      get {
        return s_Identifier;
      }
    }

    /// <summary>
    /// Identifier variable
    /// </summary>
    public static TokenDescription IdentifierVariable {
      get {
        return s_IdentifierVariable;
      }
    }

    /// <summary>
    /// Hard keywords
    /// </summary>
    public static TokenKeyWords HardKeyWords {
      get {
        return s_Hard;
      }
    }

    /// <summary>
    /// Extract string
    /// </summary>
    public static String ExtractString(String value) {
      if (string.IsNullOrWhiteSpace(value))
        return value;

      if (value.Length <= 1)
        return value;

      char quot = value[0];

      if (quot != '"' && quot != '\'')
        return value;

      if (value[0] != quot && value[value.Length - 1] != quot)
        return value;

      String num = "";

      StringBuilder hsb = new StringBuilder();

      StringBuilder sb = new StringBuilder(value.Length);

      for (int i = 1; i < value.Length - 1; ++i) {
        char ch = value[i];

        if (ch != '\\') {
          sb.Append(ch);

          continue;
        }

        i += 1;

        if (i >= value.Length - 1) {
          sb.Append(ch);

          continue;
        }

        ch = value[i];

        if (ch == 't')
          sb.Append('\t');
        else if (ch == 'b')
          sb.Append('\b');
        else if (ch == 'n')
          sb.Append('\n');
        else if (ch == 'r')
          sb.Append('\r');
        else if (ch == 'f')
          sb.Append('\f');
        else if (ch == '\'')
          sb.Append('\'');
        else if (ch == '"')
          sb.Append('"');
        else if (ch == '\\')
          sb.Append('\\');
        else if (ch >= '0' && ch <= '7') {
          hsb.Clear();

          for (int j = 0; j < 2 && (i + j < value.Length - 1); ++j) {
            ch = value[i + j];

            if (ch >= '0' && ch <= '7')
              hsb.Append(ch);
            else
              break;
          }

          num = hsb.ToString();

          if (num.Length == 3 && string.Compare(num, "377", StringComparison.Ordinal) > 0)
            num = num.Substring(0, 2);

          if (num.Length > 0)
            sb.Append((char)(Convert.ToInt32(num, 8)));

          i += num.Length;
        }
        else if (ch == 'u') {
          num = string.Concat(value
            .Skip(i)
            .Where(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F')
            .Take(4));

          if (num.Length > 0) {
            sb.Append((char)(Convert.ToInt32(num, 16)));
            i += num.Length;
          }
        }
        else
          sb.Append(ch);
      }

      return sb.ToString();
    }

    /// <summary>
    /// Extract string
    /// </summary>
    public static String ExtractRegex(String value) {
      if (string.IsNullOrWhiteSpace(value))
        return value;

      if (value.Length <= 1)
        return value;
      else if (value[0] != '/' && value[value.Length - 1] != '/')
        return value;

      StringBuilder hsb = new StringBuilder();

      StringBuilder sb = new StringBuilder(value.Length);

      for (int i = 1; i < value.Length - 1; ++i) {
        char ch = value[i];

        if (ch != '\\') {
          sb.Append(ch);

          continue;
        }

        i += 1;

        if (i >= value.Length - 1) {
          sb.Append(ch);

          continue;
        }

        ch = value[i];

        if (ch == 't')
          sb.Append('\t');
        else if (ch == 'b')
          sb.Append('\b');
        else if (ch == 'n')
          sb.Append('\n');
        else if (ch == 'r')
          sb.Append('\r');
        else if (ch == 'f')
          sb.Append('\f');
        else if (ch == '\'')
          sb.Append('\'');
        else if (ch == '"')
          sb.Append('"');
        else if (ch == '\\')
          sb.Append('\\');
        else if (ch == '/')
          sb.Append('/');
        else
          sb.Append(ch);
      }

      return sb.ToString();
    }

    #endregion Public
  }

}
