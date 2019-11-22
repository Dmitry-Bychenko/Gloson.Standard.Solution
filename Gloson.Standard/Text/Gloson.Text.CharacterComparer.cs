using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Character Comparers
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class CharacterComparer {
    #region Inner Classes

    /// <summary>
    /// Ordinal Comparison
    /// </summary>
    public sealed class OrdinalComparer 
      : IComparer<char>, 
        IEqualityComparer<char> {

      #region Create

      internal OrdinalComparer() { }

      #endregion Create

      #region Public

      public int Compare(char x, char y) => x - y;

      public bool Equals(char x, char y) => x == y;

      public int GetHashCode(char obj) => obj;

      #endregion Public
    }

    /// <summary>
    /// Ordinal Ignore Case Comparison
    /// </summary>
    public sealed class OrdinalIgnoreCaseComparer 
      : IComparer<char>,
        IEqualityComparer<char> {

      #region Create

      internal OrdinalIgnoreCaseComparer() { }

      #endregion Create

      #region Public

      public int Compare(char x, char y) => char.ToUpperInvariant(x) - char.ToUpperInvariant(y);

      public bool Equals(char x, char y) => char.ToUpperInvariant(x) == char.ToUpperInvariant(y);

      public int GetHashCode(char obj) => char.ToUpperInvariant(obj);

      #endregion Public
    }

    /// <summary>
    /// Current Culture Comparison
    /// </summary>
    public sealed class CurrentCultureComparer 
      : IComparer<char>,
        IEqualityComparer<char> {

      #region Create

      internal CurrentCultureComparer() { }

      #endregion Create

      #region Public

      public int Compare(char x, char y) => 
        StringComparer.CurrentCulture.Compare(x.ToString(), y.ToString());

      public bool Equals(char x, char y) => StringComparer.CurrentCulture.Equals(x.ToString(), y.ToString());

      public int GetHashCode(char obj) => StringComparer.CurrentCulture.GetHashCode(obj);

      #endregion Public
    }

    /// <summary>
    /// Current Culture Comparison
    /// </summary>
    public sealed class CurrentCultureIgnoreCaseComparer 
      : IComparer<char>,
        IEqualityComparer<char> {

      #region Create

      internal CurrentCultureIgnoreCaseComparer() { }

      #endregion Create

      #region Public

      public int Compare(char x, char y) =>
        StringComparer.CurrentCultureIgnoreCase.Compare(x.ToString(), y.ToString());

      public bool Equals(char x, char y) => 
        StringComparer.CurrentCultureIgnoreCase.Equals(x.ToString(), y.ToString());

      public int GetHashCode(char obj) => 
        StringComparer.CurrentCultureIgnoreCase.GetHashCode(obj);

      #endregion Public
    }

    #endregion Inner Classes

    #region Public

    /// <summary>
    /// Ordinal character comparer
    /// </summary>
    public static OrdinalComparer Ordinal => new OrdinalComparer();

    /// <summary>
    /// Ordinal Ignore Case character comparer
    /// </summary>
    public static OrdinalIgnoreCaseComparer OrdinalIgnoreCase => new OrdinalIgnoreCaseComparer();

    /// <summary>
    /// Current Culture Comparison
    /// </summary>
    public static CurrentCultureComparer CurrentCulture => new CurrentCultureComparer();

    /// <summary>
    /// Current Culture Ignore Case Comparison
    /// </summary>
    public static CurrentCultureIgnoreCaseComparer CurrentCultureIgnoreCase => new CurrentCultureIgnoreCaseComparer();

    #endregion Public
  }

}
