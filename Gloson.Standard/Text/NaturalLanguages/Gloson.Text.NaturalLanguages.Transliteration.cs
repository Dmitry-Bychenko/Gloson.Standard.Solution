using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Transliteration
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface ITransliteration : IEquatable<ITransliteration> {
    /// <summary>
    /// Translitarator name
    /// </summary>
    string Name { get; }

    /// <summary>
    /// Language From
    /// </summary>
    CultureInfo LanguageFrom { get; }

    /// <summary>
    /// Language From
    /// </summary>
    CultureInfo LanguageTo { get; }

    /// <summary>
    /// Direct Transliterator
    /// </summary>
    ITransliterator Direct { get; }

    /// <summary>
    /// Reverse Transliterator
    /// </summary>
    ITransliterator Reverse { get; }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Abstract Transliteration
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseTransliteration : ITransliteration {
    #region Create

    protected BaseTransliteration() { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    protected BaseTransliteration(string name,
                                  ITransliterator direct,
                                  ITransliterator reverse)
      : this() {

      if (name is null)
        throw new ArgumentNullException(nameof(name));
      else if (direct is null)
        throw new ArgumentNullException(nameof(direct));
      else if (reverse is null)
        throw new ArgumentNullException(nameof(reverse));

      Name = name;
      Direct = direct;
      Reverse = reverse;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => Name;

    /// <summary>
    /// Is Empty
    /// </summary>
    public bool IsEmpty => (LanguageFrom == LanguageTo);

    #endregion Public

    #region Operators

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(BaseTransliteration left, BaseTransliteration right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (right is null || left is null)
        return false;

      return left.Equals(right);
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(BaseTransliteration left, BaseTransliteration right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (right is null || left is null)
        return true;

      return !left.Equals(right);
    }

    #endregion Operators

    #region ITransliteration

    /// <summary>
    /// Translitarator name
    /// </summary>
    public virtual string Name { get; }

    /// <summary>
    /// Language From
    /// </summary>
    public virtual CultureInfo LanguageFrom => Direct.LanguageFrom;

    /// <summary>
    /// Language From
    /// </summary>
    public virtual CultureInfo LanguageTo => Direct.LanguageTo;

    /// <summary>
    /// Direct Transliterator
    /// </summary>
    public virtual ITransliterator Direct { get; }

    /// <summary>
    /// Reverse Transliterator
    /// </summary>
    public virtual ITransliterator Reverse { get; }

    #endregion ITransliteration

    #region IEquatable<ITransliteration>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ITransliteration other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (other is null)
        return false;

      return LanguageFrom == other.LanguageFrom &&
             LanguageTo == other.LanguageTo &&
             string.Equals(Name, other.Name);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as ITransliteration);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      return (LanguageFrom is null ? 0 : LanguageFrom.GetHashCode()) ^
             (LanguageTo is null ? 0 : LanguageTo.GetHashCode()) ^
             (Name is null ? 0 : Name.GetHashCode());
    }

    #endregion IEquatable<ITransliteration>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Empty Transliteration
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EmptyTransliteration : BaseTransliteration {
    #region Create

    public EmptyTransliteration(CultureInfo culture)
      : base("[Empty]", new EmptyTransliterator(culture), new EmptyTransliterator(culture)) {
    }

    #endregion Create
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Standard Transliteration
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class StandardTransliteration : BaseTransliteration {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public StandardTransliteration(string name,
                                   CultureInfo languageFrom,
                                   CultureInfo languageTo,
                                   IEnumerable<KeyValuePair<string, string>> pairs)
      : base(name,
             new StandardTransliterator(languageFrom, languageTo, pairs),
             new StandardTransliterator(languageTo, languageFrom, pairs.Select(p => new KeyValuePair<string, string>(p.Value, p.Key)))) { }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public StandardTransliteration(string name,
                                   CultureInfo languageFrom,
                                   CultureInfo languageTo,
                                   IEnumerable<(string, string)> pairs)
      : base(name,
             new StandardTransliterator(languageFrom, languageTo, pairs),
             new StandardTransliterator(languageTo, languageFrom, pairs.Select(p => (p.Item2, p.Item1)))) { }

    #endregion Create
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Transliteration Extension
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TransliterationExtensions {
    #region Inner Classes

    private sealed class TransliterationReversed : BaseTransliteration {
      private readonly ITransliteration m_Source;

      public TransliterationReversed(ITransliteration source) {
        m_Source = source ?? throw new ArgumentNullException(nameof(source));
      }

      /// <summary>
      /// Translitarator name
      /// </summary>
      public override string Name => m_Source.Name;

      /// <summary>
      /// Language From
      /// </summary>
      public override CultureInfo LanguageFrom => m_Source.LanguageTo;

      /// <summary>
      /// Language From
      /// </summary>
      public override CultureInfo LanguageTo => m_Source.LanguageFrom;

      /// <summary>
      /// Direct Transliterator
      /// </summary>
      public override ITransliterator Direct => m_Source.Reverse;

      /// <summary>
      /// Reverse Transliterator
      /// </summary>
      public override ITransliterator Reverse => m_Source.Direct;
    }

    #endregion Inner Classes

    #region Public

    /// <summary>
    /// Reversed
    /// </summary>
    /// <param name="source"></param>
    /// <returns></returns>
    public static ITransliteration Reversed(this ITransliteration source) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (source.LanguageFrom == source.LanguageTo || source.Direct == source.Reverse)
        return source;
      else
        return new TransliterationReversed(source);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Transliterations
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Transliterations {
    #region Private Data

    private static readonly ConcurrentDictionary<ITransliteration, bool> s_Items;

    #endregion Private Data

    #region Create

    static Transliterations() {
      s_Items = new ConcurrentDictionary<ITransliteration, bool>();

      Register(Library.RussianToEnglishAlaAc.Instance);
      Register(Library.RussianToEnglishGost1983UN1987.Instance);
      Register(Library.RussianToEnglishIso9.Instance);
      Register(Library.RussianToEnglishScholary.Instance);

      Register(Library.TajikToEnglishAlaAc.Instance);
      Register(Library.TajikToEnglishIso9.Instance);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Register new transliteration
    /// </summary>
    public static bool Register(ITransliteration value) {
      if (value is null)
        return false;

      return s_Items.TryAdd(value, true);
    }

    /// <summary>
    /// Transliterations 
    /// </summary>
    public static ITransliteration[] GetTransliterations() {
      return s_Items.Keys.ToArray();
    }

    #endregion Public
  }

}
