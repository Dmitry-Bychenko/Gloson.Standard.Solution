using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Gloson.Text.Parsing.Library.Java {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Java Tokens Descriptions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class JavaTokenDescriptions {
    #region Private Data

    // Hard keywords
    private static readonly TokenKeyWords s_Hard = new TokenKeyWords(new string[] {
      "true", "false", "null" });

    // Soft keywords
    private static readonly TokenKeyWords s_Soft = new TokenKeyWords(new string[] {
      "abstract", "assert", "boolean", "break", "byte", "case", "catch", "char", "class", "const", "continue", "default", "do", "double", "else", "enum", "extends", "final", "finally", "float", "for", "goto", "if", "implements", "import", "instanceof", "int", "interface", "long", "native", "new", "package", "private", "protected", "public", "return", "short", "static", "strictfp", "super", "switch", "synchronized", "this", "throw", "throws", "transient", "try", "void", "volatile", "while" });

    // Hard keyword
    private static readonly TokenDescription s_HardKeyword = TokenDescription.Create(s_Hard);

    // Soft keyword
    private static readonly TokenDescription s_SoftKeyword = TokenDescription.Create(s_Soft);

    // Line comment
    private static readonly TokenDescription s_LineComment = TokenDescription.Create("(//).*$", classification: TokenClassification.WhiteSpace);

    // Identifier
    private static readonly TokenDescription s_Identifier = TokenDescription.Create(@"[A-Za-z]+[A-Za-z0-9_]*");

    // Identifier variable
    private static readonly TokenDescription s_IdentifierVariable = TokenDescription.Create(@"\$[A-Za-z]+[A-Za-z0-9_]*");

    // Identifier attribute
    private static readonly TokenDescription s_IdentifierAttribute = TokenDescription.Create(@"@[A-Za-z]+[A-Za-z0-9_]*");

    // Rules
    private static TokenDescriptionRules s_Rules = new TokenDescriptionRules() {
      Default,
      HardKeyword,
      SoftKeyword,
      Integer,
      FloatingPoint,
      StringStandard,
      CharStandard,
      MultilineComment,
      LineComment,
      Identifier,
      IdentifierVariable,
      IdentifierAttribute,
    };

    #endregion Private Data

    #region Algorithm
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
    public static TokenDescription HardKeyword {
      get {
        return s_HardKeyword;
      }
    }

    /// <summary>
    /// Soft keyword
    /// </summary>
    public static TokenDescription SoftKeyword {
      get {
        return s_SoftKeyword;
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
        return TokenDescriptionLibrary.StringStandard;
      }
    }

    /// <summary>
    /// Char
    /// </summary>
    public static TokenDescription CharStandard {
      get {
        return TokenDescriptionLibrary.CharStandard;
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
    /// Identifier attribute
    /// </summary>
    public static TokenDescription IdentifierAttribute {
      get {
        return s_IdentifierAttribute;
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
    /// Soft keywords
    /// </summary>
    public static TokenKeyWords SoftKeyWords {
      get {
        return s_Soft;
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
      else if (value[0] != '"' && value[value.Length - 1] != '"')
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
        else if (ch == 'a')
          sb.Append('\a');
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
    /// Extract character
    /// </summary>
    public static string ExtractChar(String value) {
      if (string.IsNullOrWhiteSpace(value))
        return value;

      if (value.Length <= 1)
        return value;
      else if (value[0] != '\'' && value[value.Length - 1] != '\'')
        return value;

      value = value.Substring(1, value.Length - 2);

      if (value.Length <= 0)
        return "";

      if (value[0] != '\\')
        return value;

      value = value.Substring(1);

      if (value.Length <= 0)
        return "\\";

      char ch = value[0];
      string num;

      if (ch == 't')
        return "\t";
      else if (ch == 'b')
        return "\b";
      else if (ch == 'n')
        return "\n";
      else if (ch == 'r')
        return "\r";
      else if (ch == 'f')
        return "\f";
      else if (ch == '\'')
        return "'";
      else if (ch == '"')
        return "\"";
      else if (ch == '\\')
        return "\\";
      else if (ch >= '0' && ch <= '7') {
        num = string.Concat(value.Where(c => c >= '0' && c <= '7').Take(3));

        if (num.Length > 0)
          return ((char)Convert.ToInt32(num, 8)).ToString();
        else
          return value;
      }
      else if (ch == 'u') {
        num = string.Concat(value
          .Skip(1)
          .Where(c => c >= '0' && c <= '9' || c >= 'a' && c <= 'f' || c >= 'A' && c <= 'F')
          .Take(4));

        if (num.Length > 0)
          return ((char)Convert.ToInt32(num, 16)).ToString();
        else
          return value;
      }
      else
        return value;
    }

    #endregion Public
  }

}
