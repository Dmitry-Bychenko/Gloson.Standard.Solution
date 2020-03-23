using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gloson.Text.Parsing.Library.PLSql {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// PL/SQL Token Descriptions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class PLSqlTokenDescriptions {
    #region Private Data

    // Hard keywords
    private static readonly TokenKeyWords s_Hard = new TokenKeyWords(false, new string[] {
      "ALL", "ALTER", "AND", "ANY", "ARRAY", "ARROW", "AS", "ASC", "AT", "BEGIN", "BETWEEN", "BY", "CASE",
      "CHECK", "CLUSTERS", "CLUSTER", "COLAUTH", "COLUMNS", "COMPRESS", "CONNECT", "CRASH", "CREATE", "CURRENT",
      "DECIMAL", "DECLARE", "DEFAULT", "DELETE", "DESC", "DISTINCT", "DROP", "ELSE", "END", "EXCEPTION",
      "EXCLUSIVE", "EXISTS", "FETCH", "FORM", "FOR", "FROM", "GOTO", "GRANT", "GROUP", "HAVING", "IDENTIFIED",
      "IF", "IN", "INDEXES", "INDEX", "INSERT", "INTERSECT", "INTO", "IS", "LIKE", "LOCK", "MINUS", "MODE",
      "NOCOMPRESS", "NOT", "NOWAIT", "NULL", "OF", "ON", "OPTION", "OR", "ORDER,OVERLAPS", "PRIOR", "PROCEDURE",
      "PUBLIC", "RANGE", "RECORD", "RESOURCE", "REVOKE", "SELECT", "SHARE", "SIZE", "SQL", "START", "SUBTYPE",
      "TABAUTH", "TABLE", "THEN", "TO", "TYPE", "UNION", "UNIQUE", "UPDATE", "USE", "VALUES", "VIEW", "VIEWS",
      "WHEN", "WHERE", "WITH" });

    // Hard keyword
    private static readonly TokenDescription s_HardKeyword = TokenDescription.Create(s_Hard, classification: TokenClassification.Keyword, options: TokenDescriptionOptions.IgnoreCase);

    // Line comment
    private static readonly TokenDescription s_LineComment = TokenDescription.Create("(--).*$", classification: TokenClassification.WhiteSpace);

    // Identifier
    private static readonly TokenDescription s_Identifier = TokenDescription.Create(@"\p{L}+[\p{L}0-9_$#]*", classification: TokenClassification.Identifier, options: TokenDescriptionOptions.IgnoreCase);

    // Quotation
    private static readonly TokenDescription s_Quotation = TokenDescription.Create(TryStartQuotation, TryStopQuotation, classification: TokenClassification.Identifier);
    // String
    private static readonly TokenDescription s_String = TokenDescription.Create(TryStartString, TryStopString, classification: TokenClassification.String);

    // Operators
    private static readonly TokenDescription s_Operators = TokenDescription.Create(new string[] {
      "!=", "~=", ">=", "<=", "<>", ">", "<", ":=", "=", "||", "+", "-", "*", "/" },
      1800,
      classification: TokenClassification.Operation);

    // Rules
    private static readonly TokenDescriptionRules s_Rules = new TokenDescriptionRules() {
      Default,
      HardKeyword,
      Integer,
      FloatingPoint,
      StringStandard,
      QuotationStandard,
      MultilineComment,
      LineComment,
      Identifier,
      Operators,
    };

    #endregion Private Data

    #region Algorithm

    private static bool IsLineBreak(string value, char quot, int index, int upTo) {
      if (index < 0 || index >= value.Length)
        return false;
      else if (value[index] != quot)
        return false;

      if (index < value.Length - 1 && value[index + 1] == quot)
        return false;

      int count = 0;

      for (int i = index; i >= upTo; --i)
        if (value[i] == quot)
          count += 1;
        else
          break;

      return count % 2 != 0;
    }

    // Start Quotation
    private static Tuple<int, int> TryStartQuotation(string source, int startPosition) {
      if (source[startPosition] != '"')
        return new Tuple<int, int>(-1, -1);

      return new Tuple<int, int>(startPosition, startPosition + 1);
    }

    // Stop Quotation
    private static Tuple<int, int> TryStopQuotation(string source, int startPosition, string prefix) {
      for (int i = startPosition; i < source.Length; ++i) {
        if (IsLineBreak(source, '"', i, startPosition))
          return new Tuple<int, int>(i, i + 1);
      }

      return new Tuple<int, int>(-1, -1);
    }

    // Start String
    private static Tuple<int, int> TryStartString(string source, int startPosition) {
      string prefix = source.Substring(startPosition);

      if (prefix.StartsWith("'", StringComparison.OrdinalIgnoreCase))
        return new Tuple<int, int>(startPosition, startPosition + 1);
      else if (prefix.StartsWith("q'", StringComparison.OrdinalIgnoreCase) ||
               prefix.StartsWith("n'", StringComparison.OrdinalIgnoreCase))
        return new Tuple<int, int>(startPosition, startPosition + 3);
      else if (prefix.StartsWith("qn'", StringComparison.OrdinalIgnoreCase) ||
               prefix.StartsWith("nq'", StringComparison.OrdinalIgnoreCase))
        return new Tuple<int, int>(startPosition, startPosition + 4);

      return new Tuple<int, int>(-1, -1);
    }

    // Stop String
    private static Tuple<int, int> TryStopString(string source, int startPosition, string prefix) {
      bool special = false;
      char esc = '\0';

      if (prefix.StartsWith("q'", StringComparison.OrdinalIgnoreCase) ||
               prefix.StartsWith("nq'", StringComparison.OrdinalIgnoreCase) ||
               prefix.StartsWith("qn'", StringComparison.OrdinalIgnoreCase)) {
        special = true;
        esc = prefix[prefix.Length - 1];
      }

      if (!special) {
        for (int i = startPosition; i < source.Length; ++i) {
          if (IsLineBreak(source, '\'', i, startPosition))
            return new Tuple<int, int>(i, i + 1);
        }

        return new Tuple<int, int>(-1, -1);
      }

      // ---

      if (esc == '(')
        esc = ')';
      else if (esc == '[')
        esc = ']';
      else if (esc == '{')
        esc = '}';

      string toFind = new string(new char[] { esc, '\'' });

      int index = source.IndexOf(toFind, startPosition);

      if (index >= 0)
        return new Tuple<int, int>(index, index + 2);

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
        return s_String;
      }
    }

    /// <summary>
    /// Quotation
    /// </summary>
    public static TokenDescription QuotationStandard {
      get {
        return s_Quotation;
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
    /// Hard keywords
    /// </summary>
    public static TokenKeyWords HardKeyWords {
      get {
        return s_Hard;
      }
    }

    /// <summary>
    /// Operators
    /// </summary>
    public static TokenDescription Operators {
      get {
        return s_Operators;
      }
    }

    /// <summary>
    /// Extract Quotation
    /// </summary>
    public static String ExtractQuotation(String value) {
      if (string.IsNullOrWhiteSpace(value))
        return value;

      if (value.Length <= 1)
        return value;

      if (value[0] != '"' && value[value.Length - 1] != '"')
        return value;

      StringBuilder sb = new StringBuilder(value.Length - 2);

      for (int i = 1; i < value.Length - 1; ++i) {
        char ch = value[i];

        sb.Append(ch);

        if (ch == '"')
          i += 1;
      }

      return sb.ToString();
    }

    /// <summary>
    /// Extract String
    /// </summary>
    public static String ExtractString(String value) {
      if (string.IsNullOrWhiteSpace(value))
        return value;

      if (value.Length <= 1)
        return value;

      if (value[value.Length - 1] != '\'')
        return value;

      bool special;
      int start;
      char esc;

      if (value.StartsWith("'", StringComparison.OrdinalIgnoreCase)) {
        start = 1;
        special = false;
        esc = '\0';
      }
      else if (value.StartsWith("n'", StringComparison.OrdinalIgnoreCase)) {
        start = 2;
        special = false;
        esc = '\0';
      }
      else if (value.StartsWith("q'", StringComparison.OrdinalIgnoreCase)) {
        if (value.Length <= 3)
          return value;

        start = 2;
        special = true;
        esc = value[2];
      }
      else if (value.StartsWith("qn'", StringComparison.OrdinalIgnoreCase) ||
               value.StartsWith("qn'", StringComparison.OrdinalIgnoreCase)) {
        if (value.Length <= 4)
          return value;

        start = 3;
        special = true;
        esc = value[3];
      }
      else
        return value;

      if (esc == '(')
        esc = ')';
      else if (esc == '[')
        esc = ']';
      else if (esc == '{')
        esc = '}';

      if (special && value[value.Length - 2] != esc)
        return value;

      if (special)
        return value.Substring(start, value.Length - start - 1);

      StringBuilder sb = new StringBuilder(value.Length - 2);

      for (int i = start; i < value.Length - 1; ++i) {
        char ch = value[i];

        sb.Append(ch);

        if (ch == '\'')
          i += 1;
      }

      return sb.ToString();
    }

    #endregion Public
  }

}
