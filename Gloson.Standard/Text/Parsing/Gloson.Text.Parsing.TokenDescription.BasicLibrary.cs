using System;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text.Parsing {
  
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Description Library 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TokenDescriptionLibrary {
    #region Private Data

    // Default 
    private static readonly TokenDescription s_Default = TokenDescription.Create(@"\S", int.MaxValue, classification: TokenClassification.SyntaxElement);
    // Decimal Integer
    private static readonly TokenDescription s_IntegerDecimal = TokenDescription.Create(@"[0-9]+", classification: TokenClassification.Number);
    // Hexadecimal Integer
    private static readonly TokenDescription s_IntegerHexaDecimal = TokenDescription.Create(@"0x[0-9a-fA-F]+", classification: TokenClassification.Number);
    // Decimal + Hexadecimal Integer
    private static readonly TokenDescription s_Integer = TokenDescription.Create(@"(0x[0-9a-fA-F]+)|([0-9]+)", classification: TokenClassification.Number);

    // Floating point
    private static readonly TokenDescription s_FloatingPoint =
      TokenDescription.Create(@"(([0-9]+\.[0-9]*)|([0-9]*\.[0-9]+)([eE][\-+]?[0-9]+)?)|([0-9]+[eE][\-+]?[0-9]+)", 500, classification: TokenClassification.Number);

    // Descriptor
    private static readonly TokenDescription s_Identifier = TokenDescription.Create(@"[A-Za-z]+[A-Za-z0-9_]*", classification: TokenClassification.Identifier);

    // Line Comment
    private static readonly TokenDescription s_CommentLineComment = TokenDescription.Create(@"//.*$", classification: TokenClassification.WhiteSpace);
    // Command line Line Comment
    private static readonly TokenDescription s_CommentCmdComment = TokenDescription.Create(@"#.*$", classification: TokenClassification.WhiteSpace);
    // Multiline Comment
    private static readonly TokenDescription s_CommentMultilineComment = TokenDescription.Create(Regex.Escape(@"/*"), Regex.Escape(@"*/"), classification: TokenClassification.WhiteSpace);

    // String Standard
    private static readonly TokenDescription s_StringStandard = TokenDescription.Create((source, checkAt) => TryMatchString(source, checkAt), classification: TokenClassification.String);

    // Char Standard
    private static readonly TokenDescription s_CharStandard = TokenDescription.Create((source, checkAt) => TryMatchChar(source, checkAt), classification: TokenClassification.String);

    #endregion Private Data

    #region Algorithm

    // Try matching standard string 
    private static Tuple<int, int> TryMatchString(string source, int checkAt) {
      if (string.IsNullOrEmpty(source))
        return new Tuple<int, int>(-1, -1);

      if (checkAt >= source.Length - 1)
        return new Tuple<int, int>(-1, -1);

      if (source[checkAt] != '"')
        return new Tuple<int, int>(-1, -1);

      for (int i = checkAt + 1; i < source.Length; ++i)
        if (source[i] == '"')
          return new Tuple<int, int>(checkAt, i + 1);
        else if (source[i] == '\\')
          i += 1;

      return new Tuple<int, int>(-1, -1);
    }

    // Try matching standard character 
    private static Tuple<int, int> TryMatchChar(string source, int checkAt) {
      if (string.IsNullOrEmpty(source))
        return new Tuple<int, int>(-1, -1);

      if (checkAt >= source.Length - 1)
        return new Tuple<int, int>(-1, -1);

      if (source[checkAt] != '\'')
        return new Tuple<int, int>(-1, -1);

      for (int i = checkAt + 1; i < source.Length; ++i)
        if (source[i] == '\'')
          return new Tuple<int, int>(checkAt, i + 1);
        else if (source[i] == '\\')
          i += 1;

      return new Tuple<int, int>(-1, -1);
    }

    #endregion Algorithm

    #region Public

    #region Numbers

    /// <summary>
    /// Default
    /// </summary>
    public static TokenDescription Default {
      get {
        return s_Default;
      }
    }

    /// <summary>
    /// Integer decimal
    /// </summary>
    public static TokenDescription IntegerDecimal {
      get {
        return s_IntegerDecimal;
      }
    }

    /// <summary>
    /// Integer hex decimal
    /// </summary>
    public static TokenDescription IntegerHex {
      get {
        return s_IntegerHexaDecimal;
      }
    }

    /// <summary>
    /// Integer both decimal + hexadecimal 
    /// </summary>
    public static TokenDescription Integer {
      get {
        return s_Integer;
      }
    }

    /// <summary>
    /// Floating Point
    /// </summary>
    public static TokenDescription FloatingPoint {
      get {
        return s_FloatingPoint;
      }
    }

    #endregion Numbers

    #region Identifiers

    /// <summary>
    /// Descriptor
    /// </summary>
    public static TokenDescription Identifier {
      get {
        return s_Identifier;
      }
    }

    #endregion Identifiers

    #region Comments

    /// <summary>
    /// Line comment
    /// </summary>
    public static TokenDescription CommentLineComment {
      get {
        return s_CommentLineComment;
      }
    }

    /// <summary>
    /// Command line Line Comment 
    /// </summary>
    public static TokenDescription CommentCmdComment {
      get {
        return s_CommentCmdComment;
      }
    }

    /// <summary>
    /// Command Multiline Comment 
    /// </summary>
    public static TokenDescription CommentMultilineComment {
      get {
        return s_CommentMultilineComment;
      }
    }

    #endregion Comments

    #region Strings

    /// <summary>
    /// String standard
    /// </summary>
    public static TokenDescription StringStandard {
      get {
        return s_StringStandard;
      }
    }

    /// <summary>
    /// Character standard
    /// </summary>
    public static TokenDescription CharStandard {
      get {
        return s_CharStandard;
      }
    }

    #endregion Strings

    #endregion Public
  }

}
