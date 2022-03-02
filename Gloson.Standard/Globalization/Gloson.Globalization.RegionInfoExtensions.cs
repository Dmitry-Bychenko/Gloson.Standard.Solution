using System.Globalization;
using System.Linq;

namespace Gloson.Globalization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Region Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class RegionInfoExtensions {
    #region Public

    /// <summary>
    /// To Flag Emoji 
    /// </summary>
    public static string ToFlagEmoji(this RegionInfo region) {
      if (region is null)
        return "🏳";

      var countryCode = region.ThreeLetterISORegionName;

      return string.Concat(countryCode.ToUpper().Select(x => char.ConvertFromUtf32(x + 0x1F1A5)));
    }

    #endregion Public
  }


}
