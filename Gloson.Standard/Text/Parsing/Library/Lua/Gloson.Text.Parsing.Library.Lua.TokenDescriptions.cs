using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Gloson.Text.Parsing.Library.Lua {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Lua Token Descriptions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class LuaTokenDescriptions {
    #region Private Data

    // Hard keywords
    private static readonly TokenKeyWords s_Hard = new TokenKeyWords(new string[] {
      "and", "break", "do", "else", "elseif", "end", "false", "for", "function", "if", "in",
      "local", "nil", "not", "or", "repeat", "return", "then", "true", "until", "while"
    });

    // Hard keyword
    private static readonly TokenDescription s_HardKeyword = TokenDescription.Create(s_Hard);

    // Identifier
    private static readonly TokenDescription s_Identifier = TokenDescription.Create("[A-Za-z_]+[A-Za-z0-9_]*");

    // Line comment
    private static readonly TokenDescription s_LineComment = TokenDescription.Create("(--).*$", classification: TokenClassification.WhiteSpace);

    // Multiline Comment
    private static readonly TokenDescription s_CommentMultilineComment = TokenDescription.Create(
      Regex.Escape(@"--[["),
      Regex.Escape(@"]]"),
      classification: TokenClassification.WhiteSpace);

    // String
    private static readonly TokenDescription s_String = TokenDescription.Create(TryStartString, TryStopString);

    // Rules
    private static TokenDescriptionRules s_Rules = new TokenDescriptionRules() {
      Default,
      HardKeyword,
      Integer,
      FloatingPoint,
      StringStandard,
      MultilineComment,
      LineComment,
      Identifier,
    };

    #endregion Private Data

    #region Algorithm

    // Extract simple string
    private static string CoreExtractSimpleString(string value) {
      StringBuilder sb = new StringBuilder();
      String num = "";

      StringBuilder hsb = new StringBuilder();

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
        else if (ch >= '0' && ch <= '9') {
          hsb.Clear();

          for (int j = 0; j < 2 && (i + j < value.Length - 1); ++j) {
            ch = value[i + j];

            if (ch >= '0' && ch <= '9')
              hsb.Append(ch);
            else
              break;
          }

          num = hsb.ToString();

          if (num.Length > 0)
            sb.Append((char)(Convert.ToInt32(num)));

          i += num.Length;
        }
        else
          sb.Append(ch);
      }

      return sb.ToString();
    }

    // Start String
    private static Tuple<int, int> TryStartString(string source, int startPosition) {
      if (source[startPosition] == '\'')
        return new Tuple<int, int>(startPosition, startPosition + 1);
      else if (source[startPosition] == '"')
        return new Tuple<int, int>(startPosition, startPosition + 1);
      else if (source[startPosition] != '[')
        return new Tuple<int, int>(-1, -1);
      else if (startPosition < source.Length - 1 && source[startPosition + 1] == '[')
        return new Tuple<int, int>(startPosition, startPosition + 2);

      int count = source
        .Skip(startPosition)
        .TakeWhile(c => c == '=')
        .Count();

      if (count <= 0)
        return new Tuple<int, int>(-1, -1);

      return new Tuple<int, int>(startPosition, count + 1);
    }

    // Stop String
    private static Tuple<int, int> TryStopString(string source, int startPosition, string prefix) {
      char quot = prefix[0];

      if (quot == '[') {
        string find = prefix == "[["
          ? "]]"
          : new string('=', prefix.Length - 1) + "]";

        int index = source.IndexOf(find, startPosition);

        if (index < 0)
          return new Tuple<int, int>(-1, -1);

        return new Tuple<int, int>(index, index + find.Length);
      }

      for (int i = startPosition; i < source.Length; ++i) {
        char ch = source[i];

        if (ch != quot)
          continue;

        if (i > 0 && source[i - 1] == '\\')
          continue;

        return new Tuple<int, int>(i, i + 1);
      }

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
    /// Identifier
    /// </summary>
    public static TokenDescription Identifier {
      get {
        return s_Identifier;
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
    /// Line comment
    /// </summary>
    public static TokenDescription LineComment {
      get {
        return s_LineComment;
      }
    }

    /// <summary>
    /// Nultiline comment
    /// </summary>
    public static TokenDescription MultilineComment {
      get {
        return s_CommentMultilineComment;
      }
    }

    /// <summary>
    /// String
    /// </summary>
    public static TokenDescription StringStandard {
      get {
        return s_String;
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
    /// Extract String
    /// </summary>
    public static string ExtractString(string value) {
      if (string.IsNullOrEmpty(value))
        return "";

      if (value.Length <= 1)
        return value;

      if ((value[0] == '\'' && value[value.Length - 1] == '\'') ||
          (value[0] == '"' && value[value.Length - 1] == '"'))
        return CoreExtractSimpleString(value);

      if (value[0] != '[' || value[value.Length - 1] != ']')
        return value;

      // [[ ... ]]
      if (value.StartsWith("[[")) {
        if (!value.EndsWith("]]"))
          return value;

        if (value.IndexOf("]]") < value.Length - 2)
          return value;

        return value.Substring(2, value.Length - 4);
      }

      // [=...= ???  =...=]

      int count = value.Skip(1).TakeWhile(c => c == '=').Count();

      if (count <= 0)
        return value;

      int length = value.Length - (count + 1) * 2;

      if (length < 0)
        return value;

      return value.Substring(count + 1, length);
    }

    #endregion Public
  }

}
