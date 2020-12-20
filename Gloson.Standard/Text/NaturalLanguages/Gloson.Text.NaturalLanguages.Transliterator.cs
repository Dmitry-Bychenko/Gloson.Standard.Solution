using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Transliteration case
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum TransliterationCase {
    Lower = 0,
    Name = 1,
    Upper = 2,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Transliterator From Language into To Language
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface ITransliterator {
    /// <summary>
    /// Language From
    /// </summary>
    CultureInfo LanguageFrom { get; }

    /// <summary>
    /// Language From
    /// </summary>
    CultureInfo LanguageTo { get; }

    /// <summary>
    /// Transliterate
    /// </summary>
    /// <param name="value">value to transliterate</param>
    /// <returns></returns>
    string Transliterate(string value);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Empty Transliterator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EmptyTransliterator : ITransliterator {
    #region Create

    /// <summary>
    /// Standard Conctructor
    /// </summary>
    public EmptyTransliterator(CultureInfo culture) {
      if (null == culture)
        culture = CultureInfo.CurrentCulture;

      LanguageFrom = culture;
      LanguageTo = culture;
    }

    #endregion Create

    #region ITransliterator

    /// <summary>
    /// Language From
    /// </summary>
    public CultureInfo LanguageFrom { get; }

    /// <summary>
    /// Language From
    /// </summary>
    public CultureInfo LanguageTo { get; }

    /// <summary>
    /// Transliterate
    /// </summary>
    /// <param name="value">value to transliterate</param>
    /// <returns></returns>
    public string Transliterate(string value) => value;

    #endregion ITransliterator
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Base Transliterator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseTransliterator : ITransliterator {
    #region Private Data

    private int m_SubstitutionLength;

    #endregion Private Data

    #region Algorithm

    /// <summary>
    /// Substitutions
    /// </summary>
    protected abstract IReadOnlyDictionary<string, string> Substitutions { get; }

    protected virtual int SubstitutionLength {
      get {
        if (m_SubstitutionLength > 0)
          return m_SubstitutionLength;

        int value = Substitutions.Keys.Aggregate(0, (s, item) => Math.Max(s, item == null ? 0 : item.Length));

        Interlocked.CompareExchange(ref m_SubstitutionLength, value, 0);

        return m_SubstitutionLength;
      }
    }

    protected virtual TransliterationCase Case(string value, int index) {
      if (index < 0 || index >= value.Length)
        return TransliterationCase.Lower;

      if (!char.IsUpper(value[index]))
        return TransliterationCase.Lower;

      char left = '\0';
      char right = '\0';

      for (int i = index - 1; i >= 0; --i) {
        left = value[i];

        if (char.IsNumber(left) || char.IsPunctuation(left) || char.IsWhiteSpace(left))
          break;
        else if (char.IsLetter(left)) {
          if (char.IsLower(left))
            return TransliterationCase.Name;
          else
            break;
        }
      }

      for (int i = index + 1; i < value.Length; ++i) {
        right = value[i];

        if (char.IsNumber(right) || char.IsPunctuation(right) || char.IsWhiteSpace(right))
          break;
        else if (char.IsLetter(right)) {
          if (char.IsLower(right))
            return TransliterationCase.Name;
          else
            break;
        }
      }

      if (char.IsUpper(left) || char.IsUpper(right))
        return TransliterationCase.Upper;

      return TransliterationCase.Name;
    }

    protected virtual String CoreTransliterate(string value) {
      if (string.IsNullOrEmpty(value))
        return value;

      value = value.Normalize(NormalizationForm.FormC);

      StringBuilder sb = new StringBuilder(value.Length * 4);

      int maxLength = SubstitutionLength;

      for (int i = 0; i < value.Length;) {
        bool skip = true;

        for (int length = maxLength; length > 0; --length) {
          if (i + length > value.Length)
            continue;

          if (Substitutions.TryGetValue(value.Substring(i, length), out string chunk)) {
            skip = false;

            TransliterationCase cs = Case(value, i);

            if (cs == TransliterationCase.Lower)
              chunk = chunk.ToLowerInvariant();
            else if (cs == TransliterationCase.Upper)
              chunk = chunk.ToUpperInvariant();
            else
              chunk = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(chunk);

            sb.Append(chunk);

            i += length;
          }
        }

        if (skip) {
          sb.Append(value[i]);

          i += 1;
        }
      }

      return sb.ToString();
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="languageFrom"></param>
    /// <param name="languageTo"></param>
    protected BaseTransliterator(CultureInfo languageFrom, CultureInfo languageTo) {
      if (null == languageFrom)
        throw new ArgumentNullException(nameof(languageFrom));
      else if (null == languageTo)
        throw new ArgumentNullException(nameof(languageTo));

      LanguageFrom = languageFrom;
      LanguageTo = languageTo;
    }

    #endregion Create

    #region ITransliterator

    /// <summary>
    /// Language From
    /// </summary>
    public CultureInfo LanguageFrom { get; }

    /// <summary>
    /// Language From
    /// </summary>
    public CultureInfo LanguageTo { get; }

    /// <summary>
    /// Transliterate
    /// </summary>
    /// <param name="value">value to transliterate</param>
    /// <returns></returns>
    public string Transliterate(string value) => CoreTransliterate(value);

    #endregion ITransliterator
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Standard Transliterator
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class StandardTransliterator : BaseTransliterator {
    #region Private Data

    private readonly Dictionary<string, string> m_Correspondence = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    #endregion Private Data

    #region Algorithm

    protected override IReadOnlyDictionary<string, string> Substitutions => m_Correspondence;

    protected override int SubstitutionLength { get; }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StandardTransliterator(CultureInfo languageFrom,
                                  CultureInfo languageTo,
                                  IEnumerable<KeyValuePair<string, string>> pairs)
      : base(languageFrom, languageTo) {

      if (null == pairs)
        throw new ArgumentNullException(nameof(pairs));

      int max = 1;

      foreach (var pair in pairs) {
        m_Correspondence.Add(pair.Key.Normalize(NormalizationForm.FormC),
                             pair.Value.Normalize(NormalizationForm.FormC));

        if (pair.Key != null)
          max = Math.Max(max, pair.Key.Length);
      }

      if (languageFrom != languageTo && !m_Correspondence.Any())
        throw new ArgumentException("No data for translation", nameof(pairs));

      SubstitutionLength = max;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public StandardTransliterator(CultureInfo languageFrom,
                                  CultureInfo languageTo,
                                  IEnumerable<(string, string)> pairs)
      : this(languageFrom,
             languageTo,
             pairs != null
               ? pairs.Select(item => new KeyValuePair<string, string>(item.Item1, item.Item2))
               : Array.Empty<KeyValuePair<string, string>>()) { }

    #endregion Create
  }

}
