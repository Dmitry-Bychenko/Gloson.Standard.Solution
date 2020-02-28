using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;

using Gloson.Text.NaturalLanguages.Library;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Number Words interface
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface INumberToWords {
    /// <summary>
    /// Number to words
    /// </summary>
    string NumberToNominative(long value, GrammaticalGender gender);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Number to words
  /// </summary>
  // https://www.tools4noobs.com/online_tools/number_spell_words/
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class NumberToWords {
    #region Internal Class

    private class DefaultNumberToWords : INumberToWords {
      #region Private Data

      private static readonly List<(long value, string name)> s_Names = new List<(long value, string name)> {
        ( 1_000_000_000_000_000_000, "quintillion" ),
        (     1_000_000_000_000_000, "quadrillion" ),
        (         1_000_000_000_000, "trillion"    ),
        (             1_000_000_000, "billion"     ),
        (                 1_000_000, "million"     ),
        (                     1_000, "thousand"    ),
        (                         1, ""            ),
      };

      private static readonly string[] s_digits = new string[] {
        "zero", "one", "two", "three", "four", "five", "six", "seven", "eight", "nine",
         "ten", "eleven", "twelve", "thirteen", "fourteen", "fifteen", "sixteen", "seventeen", "eighteen", "nineteen"
      };

      private static readonly string[] s_decades = new string[] {
        "zero", "ten", "twenty", "thirty", "fourty", "fifty", "sixty", "seventy", "eighty", "ninety", "hundred",
      };

      #endregion Private Data

      #region Algorithm

      private static string ShortTerm(long value) {
        if (value <= 0)
          return "";

        StringBuilder sb = new StringBuilder();

        long h = value / 100;

        value %= 100;

        if (h > 0) {
          sb.Append(s_digits[h]);

          sb.Append(' ');
          sb.Append("hundred");

          //if (h > 1)
          //  sb.Append('s');
        }

        if (value == 0)
          return sb.ToString();
        else if (value < 20) {
          if (sb.Length > 0)
            sb.Append(' ');

          sb.Append(s_digits[value]);
        }
        else {
          if (sb.Length > 0)
            sb.Append(' ');

          sb.Append(s_decades[value / 10]);

          value %= 10;

          if (value > 0) {
            if (sb.Length > 0)
              sb.Append('-');

            sb.Append(s_digits[value]);
          }
        }

        return sb.ToString();
      }

      #endregion Algorithm

      #region Public

      public string NumberToNominative(long value, GrammaticalGender gender) {
        if (0 == value)
          return "zero";

        StringBuilder sb = new StringBuilder();

        if (value < 0)
          sb.Append("minus");

        foreach (var tuple in s_Names) {
          long count = Math.Abs(value / tuple.value);

          value %= tuple.value;

          if (count > 0) {
            if (sb.Length > 0)
              sb.Append(' ');

            sb.Append(ShortTerm(count));

            if (tuple.value > 1)
              sb.Append(' ');

            sb.Append(tuple.name);

            //if (count > 1 && tuple.value > 1)
            //  sb.Append('s');
          }
        }

        return sb.ToString();
      }

      #endregion Public
    }

    #endregion Internal Class

    #region Private Data

    private static readonly ConcurrentDictionary<CultureInfo, INumberToWords> s_Items;

    #endregion Private Data

    #region Create

    static NumberToWords() {
      s_Items = new ConcurrentDictionary<CultureInfo, INumberToWords>();

      Register(CultureInfo.GetCultureInfo("Ru"), new RuNumberToWords());
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static INumberToWords Default { get; } = new DefaultNumberToWords();

    /// <summary>
    /// Register
    /// </summary>
    public static void Register(CultureInfo culture, INumberToWords detector) {
      if (null == culture)
        culture = CultureInfo.CurrentCulture;

      if (null == detector)
        s_Items.TryRemove(culture, out detector);
      else
        s_Items.AddOrUpdate(culture, detector, (key, existing) => detector);
    }

    /// <summary>
    /// Find
    /// </summary>
    public static INumberToWords Find(CultureInfo culture) {
      if (null == culture)
        culture = CultureInfo.CurrentCulture;

      while (culture != null) {
        if (s_Items.TryGetValue(culture, out var result))
          return result;

        int p = culture.Name.LastIndexOf('-');

        if (p < 0)
          return Default;

        culture = CultureInfo.GetCultureInfo(culture.Name.Substring(0, p));
      }

      return Default;
    }

    /// <summary>
    /// To Words
    /// </summary>
    public static string ToNominative(long value, GrammaticalGender gender, CultureInfo culture) =>
      Find(culture).NumberToNominative(value, gender);

    /// <summary>
    /// To Words
    /// </summary>
    public static string ToNominative(long value, GrammaticalGender gender) =>
      Find(null).NumberToNominative(value, gender);

    /// <summary>
    /// To Words
    /// </summary>
    public static string ToNominative(long value, CultureInfo culture) =>
      Find(culture).NumberToNominative(value, GrammaticalGender.Masculine);

    /// <summary>
    /// To Words
    /// </summary>
    public static string ToNominative(long value) =>
      Find(null).NumberToNominative(value, GrammaticalGender.Masculine);

    #endregion Public
  }

}
