using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

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
         ("ъ", "”"),
         ("ы", "y"),
         ("ь", "ʹ"),
         ("э", "ė"),
         ("ю", "iu"),
         ("я", "ia"),
       }) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Instance
    /// </summary>
    public static RussianToEnglishAlaAc Instance { get; } = new RussianToEnglishAlaAc();

    #endregion Public
  }

}
