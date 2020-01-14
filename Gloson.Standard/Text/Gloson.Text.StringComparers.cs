using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Natural Comparer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StringNaturalComparer 
    : IComparer<String> {

    #region Private Data

    private IComparer<string> m_Comparer;

    #endregion Private Data

    #region Algorithm

    private static List<string> ToChunks(string value) {
      List<string> result = new List<string>();

      StringBuilder sb = null;

      bool isDigit = false;

      foreach (var c in value) {
        if (sb == null) {
          sb = new StringBuilder(value.Length);
          isDigit = char.IsDigit(c);
          sb.Append(c);
        }
        else if (isDigit != char.IsDigit(c)) {
          result.Add(sb.ToString());

          sb.Clear();

          isDigit = char.IsDigit(c);
        }

        sb.Append(c);
      }

      if (sb != null)
        result.Add(sb.ToString());

      return result;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="comparer">Comparer to be based on</param>
    public StringNaturalComparer(IComparer<string> comparer) {
      if (null == comparer)
        comparer = StringComparer.Ordinal;

      m_Comparer = comparer;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StringNaturalComparer() 
      : this((IComparer<string>) null) { }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StringNaturalComparer(CultureInfo culture, bool ignoreCase) {
      if (null == culture)
        culture = CultureInfo.CurrentCulture;

      CompareOptions options = CompareOptions.None;

      if (ignoreCase)
        options |= CompareOptions.IgnoreCase;

      m_Comparer = culture.CompareInfo.GetStringComparer(options);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StringNaturalComparer(CultureInfo culture) 
      : this(culture, false) { }

    #endregion Create

    #region IComparer<String>

    /// <summary>
    /// Compare
    /// </summary>
    public int Compare(string left, string right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(left, 0))
        return -1;
      else if (ReferenceEquals(right, 0))
        return 1;

      var lefts = ToChunks(left);
      var rights = ToChunks(right);

      int result;

      for (int i = 0; i < Math.Min(lefts.Count, rights.Count); ++i) {
        string leftChunk = lefts[i];
        string rightChunk = rights[i];

        if (char.IsDigit(leftChunk[0]) && char.IsDigit(rightChunk[0])) {
          result = leftChunk.Length.CompareTo(rightChunk.Length);

          if (result != 0)
            return result;
        }

        result = m_Comparer.Compare(leftChunk, rightChunk);

        if (result != 0)
          return result;
      }

      return lefts.Count.CompareTo(rights.Count);
    }

    #endregion IComparer<String>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Standard Comparer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class StringStandardComparer : IComparer<String> {
    #region Private Data

    private IComparer<string> m_Comparer;
    private IComparer<string> m_IgnoreCaseComparer;

    #endregion Private Data

    #region Algorithm

    private static List<string> ToChunks(string value) {
      List<string> result = new List<string>();

      StringBuilder sb = null;

      bool isDigit = false;

      foreach (var c in value) {
        if (sb == null) {
          sb = new StringBuilder(value.Length);
          isDigit = char.IsDigit(c);
          sb.Append(c);
        }
        else if (isDigit != char.IsDigit(c)) {
          result.Add(sb.ToString());

          sb.Clear();

          isDigit = char.IsDigit(c);
        }

        sb.Append(c);
      }

      if (sb != null)
        result.Add(sb.ToString());

      return result;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public StringStandardComparer(CultureInfo culture) {
      if (null == culture)
        culture = CultureInfo.InvariantCulture;

      Culture = culture;

      if (Culture == culture) {
        m_Comparer = StringComparer.Ordinal;
        m_IgnoreCaseComparer = StringComparer.OrdinalIgnoreCase;
      }
      else {
        m_Comparer = Culture.CompareInfo.GetStringComparer(CompareOptions.None);
        m_IgnoreCaseComparer = Culture.CompareInfo.GetStringComparer(CompareOptions.IgnoreCase);
      }
    }

    /// <summary>
    /// Standard Constructor (ordinal)
    /// </summary>
    public StringStandardComparer() 
      : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Culture
    /// </summary>
    public CultureInfo Culture { get; }

    #endregion Public

    #region IComparer<String>

    /// <summary>
    /// Compare
    /// </summary>
    public int Compare(string left, string right) {
      if (ReferenceEquals(left, right))
        return 0;
      else if (ReferenceEquals(left, 0))
        return -1;
      else if (ReferenceEquals(right, 0))
        return 1;

      var lefts = ToChunks(left);
      var rights = ToChunks(right);

      int result;

      for (int i = 0; i < Math.Min(lefts.Count, rights.Count); ++i) {
        string leftChunk = lefts[i];
        string rightChunk = rights[i];

        if (char.IsDigit(leftChunk[0]) && char.IsDigit(rightChunk[0])) {
          result = leftChunk.Length.CompareTo(rightChunk.Length);

          if (result != 0)
            return result;
        }

        result = m_IgnoreCaseComparer.Compare(leftChunk, rightChunk);

        if (result != 0)
          return result;

        result = m_Comparer.Compare(leftChunk, rightChunk);

        if (result != 0)
          return result;
      }

      return lefts.Count.CompareTo(rights.Count);
    }

    #endregion IComparer<String>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Comparer
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class StringComparers {
    #region Public

    /// <summary>
    /// Natural Ordinal Comparer
    /// </summary>
    public static IComparer<String> NaturalOrdinalComparer { get; } = 
      new StringNaturalComparer(StringComparer.Ordinal);

    /// <summary>
    /// Natural Ordinal IgnoreCase Comparer
    /// </summary>
    public static IComparer<String> NaturalOrdinalIgnoreCaseComparer { get; } =
      new StringNaturalComparer(StringComparer.OrdinalIgnoreCase);

    /// <summary>
    /// Natural Comparer
    /// </summary>
    public static IComparer<String> NaturalCurrentCultureComparer { get; } =
      new StringNaturalComparer(StringComparer.CurrentCulture);

    /// <summary>
    /// Natural IgnoreCase Comparer
    /// </summary>
    public static IComparer<String> NaturalCurrentCultureIgnoreCaseComparer { get; } =
      new StringNaturalComparer(StringComparer.CurrentCultureIgnoreCase);

    /// <summary>
    /// Standard Ordinal Comparer
    /// </summary>
    public static IComparer<String> StandardOrdinalComparer { get; } = 
      new StringStandardComparer();

    /// <summary>
    /// Standard Current Culture Comparer
    /// </summary>
    public static IComparer<String> StandardCurrentCultureComparer { get; } =
      new StringStandardComparer(CultureInfo.CurrentCulture);

    /// <summary>
    /// Natural Comparer based on comparer
    /// </summary>
    public static IComparer<String> NaturalComparer(IComparer<String> source) => 
      new StringNaturalComparer(source);

    /// <summary>
    /// Natural Comparer
    /// </summary>
    public static IComparer<String> NaturalComparer(CultureInfo culture, bool ignoreCase) {
      if (ReferenceEquals(null, culture))
        culture = CultureInfo.CurrentCulture;

      CompareOptions options = CompareOptions.None;

      if (ignoreCase)
        options |= CompareOptions.IgnoreCase;

      return new StringNaturalComparer(culture.CompareInfo.GetStringComparer(options));
    }

    /// <summary>
    /// Natural Comparer
    /// </summary>
    public static IComparer<String> NaturalComparer(CultureInfo culture) {
      if (ReferenceEquals(null, culture))
        culture = CultureInfo.CurrentCulture;

      CompareOptions options = CompareOptions.None;

      return new StringNaturalComparer(culture.CompareInfo.GetStringComparer(CompareOptions.None));
    }

    /// <summary>
    /// Standard Comparer
    /// </summary>
    public static IComparer<String> StandardComparer(CultureInfo culture) =>
      new StringStandardComparer(culture);

    #endregion Public
  }

}
