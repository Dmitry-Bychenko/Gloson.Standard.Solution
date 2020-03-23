using System.Globalization;
using System.Linq;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// String Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class StringExtensions {
    #region Public

    /// <summary>
    /// Remove diacritics crème brûlée => creme brulee
    /// </summary>
    public static string RemoveDiacritics(string value) {
      if (null == value)
        return null;

      return string
        .Concat(value
           .Normalize(NormalizationForm.FormD)
           .Where(c => CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark))
        .Normalize(NormalizationForm.FormC);
    }

    #endregion Public
  }

}
