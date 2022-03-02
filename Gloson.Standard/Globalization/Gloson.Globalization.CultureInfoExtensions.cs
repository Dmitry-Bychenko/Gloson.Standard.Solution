using System.Globalization;

namespace Gloson.Globalization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Region Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class CultureInfoExtensions {
    #region Public

    /// <summary>
    /// To RegionInfo
    /// </summary>
    public static RegionInfo ToRegioninfo(this CultureInfo culture) =>
      culture is null ? null : new RegionInfo(culture.LCID);

    /// <summary>
    /// To Flag Emoji 
    /// </summary>
    public static string ToFlagEmoji(this CultureInfo culture) =>
      culture is null ? "🏳" : culture.ToRegioninfo().ToFlagEmoji();

    #endregion Public
  }

}
