using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

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
    /// To Greyscale
    /// </summary>
    public static Color ToGreyscale(this Color value) {
      int gs = (value.R * 299 + value.G * 587 + value.B * 114 + 499) / 1000;

      return Color.FromArgb(value.A, gs, gs, gs);
    }

    #endregion Public
  }
}
