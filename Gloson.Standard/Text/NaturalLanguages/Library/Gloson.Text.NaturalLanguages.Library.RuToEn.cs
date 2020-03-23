// https://www.translitteration.com/transliteration/en/russian/iso-9/

using System.Globalization;

namespace Gloson.Text.NaturalLanguages.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Russian / English (Library of Congress)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RussianToEnglishAlaAc : StandardTransliteration {
    #region Create

    static RussianToEnglishAlaAc() {
      Instance = new RussianToEnglishAlaAc();

      Transliterations.Register(Instance);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RussianToEnglishAlaAc() : base(
      "AlaAc",
       CultureInfo.GetCultureInfo("Ru"),
       CultureInfo.GetCultureInfo("En"),
       new (string, string)[] {
         ("а", "a"),
         ("б", "b"),
         ("в", "v"),
         ("г", "g"),
         ("д", "d"),
         ("е", "e"),
         ("ё", "ë"),
         ("ж", "zh"),
         ("з", "z"),
         ("и", "i"),
         ("й", "ĭ"),
         ("к", "k"),
         ("л", "l"),
         ("м", "m"),
         ("н", "n"),
         ("о", "o"),
         ("п", "p"),
         ("р", "r"),
         ("с", "s"),
         ("т", "t"),
         ("у", "u"),
         ("ф", "f"),
         ("х", "kh"),
         ("ц", "ts"),
         ("ч", "ch"),
         ("ш", "sh"),
         ("щ", "shch"),
         ("ъ", "\""),
         ("ы", "y"),
         ("ь", "'"),
         ("э", "ė"),
         ("ю", "iu"),
         ("я", "ia"),
       }) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Instance
    /// </summary>
    public static RussianToEnglishAlaAc Instance { get; }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Russian / English (GOST 1983 and UN 1987)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RussianToEnglishGost1983UN1987 : StandardTransliteration {
    #region Create

    static RussianToEnglishGost1983UN1987() {
      Instance = new RussianToEnglishGost1983UN1987();

      Transliterations.Register(Instance);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RussianToEnglishGost1983UN1987() : base(
      "Gost1983UN1987",
       CultureInfo.GetCultureInfo("Ru"),
       CultureInfo.GetCultureInfo("En"),
       new (string, string)[] {
         ("а", "a"),
         ("б", "b"),
         ("в", "v"),
         ("г", "g"),
         ("д", "d"),
         ("е", "e"),
         ("ё", "ë"),
         ("ж", "ž"),
         ("з", "z"),
         ("и", "i"),
         ("й", "j"),
         ("к", "k"),
         ("л", "l"),
         ("м", "m"),
         ("н", "n"),
         ("о", "o"),
         ("п", "p"),
         ("р", "r"),
         ("с", "s"),
         ("т", "t"),
         ("у", "u"),
         ("ф", "f"),
         ("х", "h"),
         ("ц", "c"),
         ("ч", "č"),
         ("ш", "š"),
         ("щ", "šč"),
         ("ъ", "\""),
         ("ы", "y"),
         ("ь", "'"),
         ("э", "è"),
         ("ю", "ju"),
         ("я", "ja"),
       }) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Instance
    /// </summary>
    public static RussianToEnglishGost1983UN1987 Instance { get; }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Russian / English ISO 9
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RussianToEnglishIso9 : StandardTransliteration {
    #region Create

    static RussianToEnglishIso9() {
      Instance = new RussianToEnglishIso9();

      Transliterations.Register(Instance);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RussianToEnglishIso9() : base(
      "Iso9",
       CultureInfo.GetCultureInfo("Ru"),
       CultureInfo.GetCultureInfo("En"),
       new (string, string)[] {
         ("а", "a"),
         ("б", "b"),
         ("в", "v"),
         ("г", "g"),
         ("д", "d"),
         ("е", "e"),
         ("ё", "ë"),
         ("ж", "ž"),
         ("з", "z"),
         ("и", "i"),
         ("й", "j"),
         ("к", "k"),
         ("л", "l"),
         ("м", "m"),
         ("н", "n"),
         ("о", "o"),
         ("п", "p"),
         ("р", "r"),
         ("с", "s"),
         ("т", "t"),
         ("у", "u"),
         ("ф", "f"),
         ("х", "h"),
         ("ц", "c"),
         ("ч", "č"),
         ("ш", "š"),
         ("щ", "ŝ"),
         ("ъ", "\""),
         ("ы", "y"),
         ("ь", "'"),
         ("э", "è"),
         ("ю", "û"),
         ("я", "â"),
       }) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Instance
    /// </summary>
    public static RussianToEnglishIso9 Instance { get; }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Russian / English Scholary
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RussianToEnglishScholary : StandardTransliteration {
    #region Create

    static RussianToEnglishScholary() {
      Instance = new RussianToEnglishScholary();

      Transliterations.Register(Instance);
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public RussianToEnglishScholary() : base(
      "Scholary",
       CultureInfo.GetCultureInfo("Ru"),
       CultureInfo.GetCultureInfo("En"),
       new (string, string)[] {
         ("а", "a"),
         ("б", "b"),
         ("в", "v"),
         ("г", "g"),
         ("д", "d"),
         ("е", "e"),
         ("ё", "ë"),
         ("ж", "ž"),
         ("з", "z"),
         ("и", "i"),
         ("й", "j"),
         ("к", "k"),
         ("л", "l"),
         ("м", "m"),
         ("н", "n"),
         ("о", "o"),
         ("п", "p"),
         ("р", "r"),
         ("с", "s"),
         ("т", "t"),
         ("у", "u"),
         ("ф", "f"),
         ("х", "x"),
         ("ц", "c"),
         ("ч", "č"),
         ("ш", "š"),
         ("щ", "šč"),
         ("ъ", "\""),
         ("ы", "y"),
         ("ь", "'"),
         ("э", "è"),
         ("ю", "ju"),
         ("я", "ja"),
       }) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Instance
    /// </summary>
    public static RussianToEnglishScholary Instance { get; }

    #endregion Public
  }

}
