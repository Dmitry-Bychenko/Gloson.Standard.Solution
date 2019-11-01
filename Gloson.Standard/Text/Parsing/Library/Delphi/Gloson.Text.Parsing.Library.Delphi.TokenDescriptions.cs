using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text.Parsing.Library.Delphi {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Delphi Token Description Library
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  internal static class DelphiTokenDescriptionLibrary {
    #region Private Data

    // Decimal + Hexadecimal Integer
    private static readonly TokenDescription s_Integer = TokenDescription.Create(@"-?(\$[0-9a-fA-F]+)|([0-9]+)", classification: TokenClassification.Number);

    // String Standard
    private static readonly TokenDescription s_StringStandard = TokenDescription.Create((source, checkAt) => TryMatchString(source, checkAt), classification: TokenClassification.String);

    #endregion Private Data

    #region Algorithm

    // Try Match Delphi String
    private static Tuple<int, int> TryMatchString(string source, int checkAt) {
      int state = 0;

      if (source[checkAt] != '\'' && source[checkAt] != '#')
        return new Tuple<int, int>(-1, -1);

      for (int i = checkAt; i < source.Length; ++i) {
        char c = source[i];

        if (state == 0) {
          if (c == '\'')
            state = 3;
          else if (c == '#')
            state = 1;
          else
            return new Tuple<int, int>(checkAt, i);
        }
        else if (state == 1) {
          if (c >= '0' && c <= '9')
            state = 4;
          else if (c == '$')
            state = 2;
          else
            return new Tuple<int, int>(checkAt, i);
        }
        else if (state == 2) {
          if (c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F')
            continue;
          else if (c == '#')
            state = 1;
          else if (c == '\'')
            state = 3;
          else
            return new Tuple<int, int>(checkAt, i);
        }
        else if (state == 3) {
          if (c == '\'') {
            state = 0;
          }
        }
        else if (state == 4) {
          if (c >= '0' && c <= '9')
            continue;
          else if (c == '#')
            state = 1;
          else if (c == '\'')
            state = 3;
          else
            return new Tuple<int, int>(checkAt, i);
        }
        else if (state == 5) {
          if (c == '\'')
            state = 0;
        }
      }

      return new Tuple<int, int>(checkAt, source.Length);
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Integer both decimal + hexadecimal 
    /// </summary>
    public static TokenDescription Integer {
      get {
        return s_Integer;
      }
    }

    /// <summary>
    /// String
    /// </summary>
    public static TokenDescription String {
      get {
        return s_StringStandard;
      }
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Delphi (PAS) Token Descriptions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DelphiTokenDescriptions {
    #region Private Data

    // Keywords
    private static readonly TokenKeyWords s_KeyWords = new TokenKeyWords(false, new string[] {
      "and", "array", "as", "asm", "at", "automated", "begin", "case", "class", "const",
      "constructor", "destructor", "dispinterface", "div", "do", "downto", "else", "end",
      "except", "exports", "file", "finalization", "finally", "for", "function", "goto",
      "if", "implementation", "in", "inherited", "initialization", "inline", "interface",
      "is", "label", "library", "mod", "nil", "not", "object", "of", "on", "or", "out",
      "packed", "private", "procedure", "program", "property", "protected", "public",
      "published", "raise", "record", "repeat", "resourcestring", "set", "shl", "shr",
      "string", "then", "threadvar", "to", "try", "type", "unit", "until", "uses", "var",
      "while", "with", "xor" });

    // Hard keyword
    private static readonly TokenDescription s_Keyword = TokenDescription.Create(s_KeyWords, classification: TokenClassification.Keyword);
    // Identifier
    private static readonly TokenDescription s_Identifier = TokenDescription.Create(@"[A-Za-z_]+[A-Za-z0-9_]*", classification: TokenClassification.Identifier);

    // Line Comment
    private static readonly TokenDescription s_CommentLineComment = TokenDescription.Create(@"\/\/.*$", 400, classification: TokenClassification.WhiteSpace);
    // Multiline Comment
    private static readonly TokenDescription s_CommentMultilineComment = TokenDescription.Create(Regex.Escape(@"(*"), Regex.Escape(@"*)"), classification: TokenClassification.WhiteSpace);
    // Multiline Comment (2)
    private static readonly TokenDescription s_CommentMultilineComment2 = TokenDescription.Create(@"\{(?!\$)", @"\}", classification: TokenClassification.WhiteSpace);

    // Directive
    private static readonly TokenDescription s_Directive = TokenDescription.Create(@"\{\$", @"\}", classification: TokenClassification.Pragma);
    // Directive 2
    private static readonly TokenDescription s_Directive2 = TokenDescription.Create(@"\(\*\$", @"\*\)", classification: TokenClassification.Pragma);

    // Operators
    private static readonly TokenDescription s_Operators = TokenDescription.Create(new string[] {
      ">=", "<=", "<>", ">", "<", "=", ":=", "+", "-", "*", "/" },
      900,
      classification: TokenClassification.Operation);

    #endregion Private Data

    #region Algorithm

    // Rules
    private static TokenDescriptionRules s_Rules = new TokenDescriptionRules() {
      Default,
      Keyword,
      Integer,
      FloatingPoint,
      Identifier,
      Directive,
      Directive2,
      CommentLineComment,
      CommentMultilineComment,
      CommentMultilineComment2,

      DelphiString,
      Operator,
    };

    #endregion Algorithm

    #region Create
    #endregion Create

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
    public static TokenDescription Keyword {
      get {
        return s_Keyword;
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
    /// Integer
    /// </summary>
    public static TokenDescription Integer {
      get {
        return DelphiTokenDescriptionLibrary.Integer;
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
    /// Directive
    /// </summary>
    public static TokenDescription Directive {
      get {
        return s_Directive;
      }
    }

    /// <summary>
    /// Directive 2 
    /// </summary>
    public static TokenDescription Directive2 {
      get {
        return s_Directive2;
      }
    }

    /// <summary>
    /// Line comment
    /// </summary>
    public static TokenDescription CommentLineComment {
      get {
        s_CommentLineComment.Title = "Line comment";

        return s_CommentLineComment;
      }
    }

    /// <summary>
    /// Multiline comment
    /// </summary>
    public static TokenDescription CommentMultilineComment {
      get {
        return s_CommentMultilineComment;
      }
    }

    /// <summary>
    /// Multiline comment 2
    /// </summary>
    public static TokenDescription CommentMultilineComment2 {
      get {
        return s_CommentMultilineComment2;
      }
    }

    /// <summary>
    /// String / Char
    /// </summary>
    public static TokenDescription DelphiString {
      get {
        return DelphiTokenDescriptionLibrary.String;
      }
    }

    /// <summary>
    /// Extract string
    /// </summary>
    public static String ExtractString(String value) {
      if (string.IsNullOrEmpty(value))
        return "";

      bool inApos = false;

      string v = null;

      StringBuilder sb = new StringBuilder();

      for (int i = 0; i < value.Length;) {
        char ch = value[i];
        char nextCh = i >= value.Length - 1 ? '\0' : value[i + 1];

        if (inApos) {
          if (ch == '\'') {
            if (nextCh == '\'') {
              sb.Append(ch);

              i += 1;
            }
            else
              inApos = false;
          }
          else
            sb.Append(ch);

          i += 1;

          continue;
        }

        if (char.IsWhiteSpace(ch)) {
          i += 1;

          continue;
        }
        else if (ch == '\'') {
          i += 1;
          inApos = true;

          continue;
        }
        else if (ch != '#')
          break;

        if (nextCh == '$') {
          i += 2;
          // Hex
          v = string.Concat(value
            .Skip(i)
            .TakeWhile(c => c >= '0' && c <= '9' ||
                            c >= 'a' && c <= 'f' ||
                            c >= 'A' && c <= 'F')
            .Take(2));

          if (v.Length > 0) {
            sb.Append((char)(Convert.ToInt32(v, 16)));
            i += v.Length;
          }
        }
        else {
          i += 1;
          // Dec
          v = string.Concat(value
           .Skip(i)
           .TakeWhile(c => c >= '0' && c <= '9')
           .Take(3));

          if (v.Length > 0) {
            sb.Append((char)(int.Parse(v)));
            i += v.Length;
          }
        }
      }

      return sb.ToString();
    }

    /// <summary>
    /// Operators
    /// </summary>
    public static TokenDescription Operator {
      get {
        return s_Operators;
      }
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Delphi DFM) Token Descriptions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DelphiDfmTokenDescriptions {
    #region Private Data

    // Keywords
    private static readonly TokenKeyWords s_KeyWords = new TokenKeyWords(false, new string[] {
      "end", "object", "item", "inherited", "inline", "="});

    // Hard keyword
    private static readonly TokenDescription s_Keyword = TokenDescription.Create(s_KeyWords, options: TokenDescriptionOptions.IgnoreCase);
    // Identifier
    private static readonly TokenDescription s_Identifier = TokenDescription.Create(@"[A-Za-z_]+[A-Za-z0-9_]*", options: TokenDescriptionOptions.IgnoreCase);

    // Line Comment
    private static readonly TokenDescription s_CommentLineComment = TokenDescription.Create(@"//.*$");
    // Multiline Comment
    private static readonly TokenDescription s_CommentMultilineComment = TokenDescription.Create(@"\(\*", @"\*\)");
    // Multiline Comment (2)
    private static readonly TokenDescription s_CommentMultilineComment2 = TokenDescription.Create(@"\{(?!\$)", Regex.Escape(@"}"));

    // Directive
    private static readonly TokenDescription s_Directive = TokenDescription.Create(@"\{\$", Regex.Escape(@"}"));
    // Directive 2
    private static readonly TokenDescription s_Directive2 = TokenDescription.Create(@"\(\*\$", @"\*\)");

    // Operators
    private static readonly TokenDescription s_Operators = TokenDescription.Create(new string[] {
      ">=", "<=", "<>", ">", "<", "=", ":=", "+", "-", "*", "/" }, 900);

    private static readonly TokenDescription s_OpenBrackets = TokenDescription.Create(new string[] {
      "<", "[", "{", "("}, 950);

    private static readonly TokenDescription s_CloseBrackets = TokenDescription.Create(new string[] {
      ">", "]", "}", ")"}, 950);

    private static readonly TokenDescription s_Boolean = TokenDescription.Create(new string[] {
      "true", "false", }, 950, TokenDescriptionOptions.IgnoreCase);

    #endregion Private Data

    #region Algorithm

    // Rules
    private static TokenDescriptionRules s_Rules = new TokenDescriptionRules() {
      Default,
      Keyword,
      Integer,
      FloatingPoint,
      Identifier,

      DelphiString,
      Operator,

      s_OpenBrackets,
      s_CloseBrackets,

      Boolean,
    };

    #endregion Algorithm

    #region Create
    #endregion Create

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
    public static TokenDescription Keyword {
      get {
        return s_Keyword;
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
    /// Integer
    /// </summary>
    public static TokenDescription Integer {
      get {
        return DelphiTokenDescriptionLibrary.Integer;
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
    /// String / Char
    /// </summary>
    public static TokenDescription DelphiString {
      get {
        return DelphiTokenDescriptionLibrary.String;
      }
    }

    /// <summary>
    /// Operators
    /// </summary>
    public static TokenDescription Operator {
      get {
        return s_Operators;
      }
    }

    /// <summary>
    /// Operators
    /// </summary>
    public static TokenDescription OpenBracket {
      get {
        return s_OpenBrackets;
      }
    }

    /// <summary>
    /// Operators
    /// </summary>
    public static TokenDescription CloseBracket {
      get {
        return s_CloseBrackets;
      }
    }

    /// <summary>
    /// Boolean
    /// </summary>
    public static TokenDescription Boolean {
      get {
        return s_Boolean;
      }
    }

    #endregion Public
  }

}
