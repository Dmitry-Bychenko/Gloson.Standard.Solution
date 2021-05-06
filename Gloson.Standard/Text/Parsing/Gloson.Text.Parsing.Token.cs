using System;
using System.Collections.Generic;

namespace Gloson.Text.Parsing {

  //-----------------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token Classification
  /// </summary>
  //
  //-----------------------------------------------------------------------------------------------------------------------------

  public enum TokenClassification {
    /// <summary>
    /// Unknown
    /// </summary>
    Unknown = 0,
    /// <summary>
    /// WhiteSpace
    /// </summary>
    WhiteSpace = 1,
    /// <summary>
    /// Pragma
    /// </summary>
    Pragma = 2,
    /// <summary>
    /// Number
    /// </summary>
    Number = 3,
    /// <summary>
    /// Character
    /// </summary>
    Character = 4,
    /// <summary>
    /// String
    /// </summary>
    String = 5,
    /// <summary>
    /// Keyword 
    /// </summary>
    Keyword = 7,
    /// <summary>
    /// Operation
    /// </summary>
    Operation = 8,
    /// <summary>
    /// Syntax (auxliary) element
    /// </summary>
    SyntaxElement = 9,
    /// <summary>
    /// Identifier
    /// </summary>
    Identifier = 10,
  }

  //-----------------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Token  
  /// </summary>
  //
  //-----------------------------------------------------------------------------------------------------------------------------

  public class Token {
    #region Internal class

    private class TokenComparer : IComparer<Token> {
      public int Compare(Token x, Token y) {
        if (object.ReferenceEquals(x, y))
          return 0;
        else if (x is null)
          return -1;
        else if (y is null)
          return 1;

        int result = x.StartLine.CompareTo(y.StartLine);

        if (result != 0)
          return result;

        result = x.StartColumn.CompareTo(y.StartColumn);

        if (result != 0)
          return result;

        result = x.StopLine.CompareTo(y.StopLine);

        if (result != 0)
          return result;

        result = x.StopColumn.CompareTo(y.StopColumn);

        if (result != 0)
          return result;

        return result;
      }
    }

    private class EssenceEqualityComparer : IEqualityComparer<Token> {
      public bool Equals(Token x, Token y) {
        if (ReferenceEquals(x, y))
          return true;
        else if (x is null || y is null)
          return false;

        return x.Description == y.Description &&
               string.Equals(x.Text,
                             y.Text,
                             x.Description.Options.HasFlag(TokenDescriptionOptions.IgnoreCase)
                                ? StringComparison.OrdinalIgnoreCase
                                : StringComparison.Ordinal);
      }

      public int GetHashCode(Token token) {
        if (token is null)
          return 0;

        return token.Description.GetHashCode() ^
               (token.Text ?? "").ToUpperInvariant().GetHashCode();
      }
    }

    #endregion Internal class

    #region Private data

    // Location comparer
    private static readonly TokenComparer s_LocationComparer = new();

    private static readonly EssenceEqualityComparer s_EssenceComparer = new();

    #endregion Private data 

    #region Algorithm
    #endregion Algorithm

    #region Create

    // Standard constructor
    internal Token(TokenDescription descriptor, int startLine, int startColumn)
      : base() {

      Description = descriptor;
      StartLine = startLine;
      StartColumn = startColumn;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Location comparer
    /// </summary>
    public static IComparer<Token> LocationComparer {
      get {
        return s_LocationComparer;
      }
    }

    /// <summary>
    /// Essence comparer
    /// </summary>
    public static IEqualityComparer<Token> EssenceComparer {
      get {
        return s_EssenceComparer;
      }
    }

    /// <summary>
    /// Tag
    /// </summary>
    public object Tag {
      get;
      set;
    }

    /// <summary>
    /// Text
    /// </summary>
    public string Text {
      get;
      set;
    }

    /// <summary>
    /// Is White Space
    /// </summary>
    public bool IsWhiteSpace {
      get {
        if (Description is null)
          return false;

        return Description.IsWhiteSpace;
      }
    }

    /// <summary>
    /// Descriptor
    /// </summary>
    public TokenDescription Description {
      get;
      private set;
    }

    /// <summary>
    /// Classification
    /// </summary>
    public TokenClassification Classification {
      get {
        if (Description is null)
          return TokenClassification.Unknown;

        return Description.Classification;
      }
    }

    /// <summary>
    /// Start line (zero based)
    /// </summary>
    public int StartLine {
      get;
      private set;
    }

    /// <summary>
    /// Start column (zero based)
    /// </summary>
    public int StartColumn {
      get;
      private set;
    }

    /// <summary>
    /// Stop line (zero based)
    /// </summary>
    public int StopLine {
      get;
      internal set;
    }

    /// <summary>
    /// Stop column (zero based)
    /// </summary>
    public int StopColumn {
      get;
      internal set;
    }

    /// <summary>
    /// Text
    /// </summary>
    public override string ToString() {
      return Text;
    }

    /// <summary>
    /// Equals To Text
    /// </summary>
    public bool EqualsToText(string value) {
      if (value is null)
        return false;

      if (Classification == TokenClassification.String ||
          Classification == TokenClassification.Character)
        return string.Equals(value, Text);

      return string.Equals(
        value,
        Text,
        Description.Options.HasFlag(TokenDescriptionOptions.IgnoreCase)
          ? StringComparison.OrdinalIgnoreCase
          : StringComparison.Ordinal);
    }

    /// <summary>
    /// Text Comparer
    /// </summary>
    public IComparer<String> TextComparer {
      get {
        if (Description is null)
          return StringComparer.Ordinal;

        return Description.Options.HasFlag(TokenDescriptionOptions.IgnoreCase)
          ? StringComparer.OrdinalIgnoreCase
          : StringComparer.Ordinal;
      }
    }

    /// <summary>
    /// Text Equality Comparer
    /// </summary>
    public IEqualityComparer<String> TextEqualityComparer {
      get {
        if (Description is null)
          return StringComparer.Ordinal;

        return Description.Options.HasFlag(TokenDescriptionOptions.IgnoreCase)
          ? StringComparer.OrdinalIgnoreCase
          : StringComparer.Ordinal;
      }
    }

    #endregion Public
  }
}
