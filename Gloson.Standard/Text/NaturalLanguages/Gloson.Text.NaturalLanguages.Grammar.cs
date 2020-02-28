using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gloson.Text.NaturalLanguages {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Gender
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  [Flags]
  public enum GrammaticalGender {
    None = 0b0000,
    Masculine = 0b0001,
    Feminine = 0b0010,
    Nueter = 0b0100,
    Common = Masculine | Feminine,
    Animate = 0b1000
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Number
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public enum GrammaticalNumber {
    Singular = 0,
    Plural = 1,
    Dual = 2,
    Trial = 3,
    Quadral = 4,
    Paucal = 5,
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// IGrammaticalNumberDetector
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IGrammaticalNumberDetector {
    /// <summary>
    /// Detect Grammatic Number for a given number
    /// </summary>
    GrammaticalNumber Detect(int value);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Grammatical Number Detector
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class GrammaticalNumberDetector {
    #region Internal Class

    private sealed class DefaultGrammaticalNumberDetector : IGrammaticalNumberDetector {
      public GrammaticalNumber Detect(int value) {
        return (value == 1 || value == -1)
          ? GrammaticalNumber.Singular
          : GrammaticalNumber.Plural;
      }
    }

    private sealed class RussianGrammaticalNumberDetector : IGrammaticalNumberDetector {
      public GrammaticalNumber Detect(int value) {
        value = Math.Abs(value % 100);

        if (value >= 20)
          value %= 10;

        return
            value == 1 ? GrammaticalNumber.Singular
          : value == 2 || value == 3 || value == 4 ? GrammaticalNumber.Dual
          : GrammaticalNumber.Plural;
      }
    }

    #endregion Internal Class

    #region Private Data

    private static readonly ConcurrentDictionary<CultureInfo, IGrammaticalNumberDetector> s_Detectors;

    #endregion Private Data

    #region Create

    static GrammaticalNumberDetector() {
      s_Detectors = new ConcurrentDictionary<CultureInfo, IGrammaticalNumberDetector>();

      Register(CultureInfo.GetCultureInfo("ru"), new RussianGrammaticalNumberDetector());
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static IGrammaticalNumberDetector Default { get; } = new DefaultGrammaticalNumberDetector();

    /// <summary>
    /// Register
    /// </summary>
    public static void Register(CultureInfo culture, IGrammaticalNumberDetector detector) {
      if (null == culture)
        culture = CultureInfo.CurrentCulture;

      if (null == detector)
        s_Detectors.TryRemove(culture, out detector);
      else
        s_Detectors.AddOrUpdate(culture, detector, (key, existing) => detector);
    }

    /// <summary>
    /// Find
    /// </summary>
    public static IGrammaticalNumberDetector Find(CultureInfo culture) {
      if (null == culture)
        culture = CultureInfo.CurrentCulture;

      while (culture != null) {
        if (s_Detectors.TryGetValue(culture, out var result))
          return result;

        int p = culture.Name.LastIndexOf('-');

        if (p < 0)
          return Default;

        culture = CultureInfo.GetCultureInfo(culture.Name.Substring(0, p));
      }

      return Default;
    }

    /// <summary>
    /// Detect Grammatical Number 
    /// </summary>
    public static GrammaticalNumber DetectNumber(int value, CultureInfo culture) =>
      Find(culture).Detect(value);

    /// <summary>
    /// Detect Grammatical Number 
    /// </summary>
    public static GrammaticalNumber DetectNumber(int value) => Find(null).Detect(value);

    #endregion Public
  }

}
