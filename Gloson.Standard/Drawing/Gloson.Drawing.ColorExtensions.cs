using System.Drawing;

namespace Gloson.Drawing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Color Extensions 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class ColorExtensions {
    #region Public

    /// <summary>
    /// To Grayscale
    /// </summary>
    public static Color ToGrayscale(this Color value) {
      int gs = (value.R * 299 + value.G * 587 + value.B * 114 + 499) / 1000;

      return Color.FromArgb(value.A, gs, gs, gs);
    }

    /// <summary>
    /// Gray scale value
    /// </summary>
    public static int Grayscale(this Color value) => (value.R * 299 + value.G * 587 + value.B * 114 + 499) / 1000;

    #endregion Public
  }

}
