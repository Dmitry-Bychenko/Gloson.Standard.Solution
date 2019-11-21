using System;
using System.Collections.Generic;
using System.Drawing;
using System.Text;

namespace Gloson.Drawing {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Console Color Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class ConsoleColorExtensions {
    #region Private Data

    private static Color[] s_Colors = new Color[] {
      Color.FromArgb(0x000000), // Black
      Color.FromArgb(0x000080), // Dark Blue
      Color.FromArgb(0x008000), // Dark Green
      Color.FromArgb(0x008080), // Dark Cyan
      Color.FromArgb(0x800000), // Dark Red
      Color.FromArgb(0x800080), // Dark Magenta
      Color.FromArgb(0x808000), // Dark Yellow
      Color.FromArgb(0xC0C0C0), // Gray

      Color.FromArgb(0x808080), // DarkGray
      Color.FromArgb(0x0000FF), // Blue
      Color.FromArgb(0x00FF00), // Green
      Color.FromArgb(0x00FFFF), // Cyan
      Color.FromArgb(0xFF0000), // Red
      Color.FromArgb(0xFF00FF), // Magenta
      Color.FromArgb(0xFFFF00), // Yellow
      Color.FromArgb(0xFFFFFF), // White
    };

    #endregion Private Data

    #region Public

    /// <summary>
    /// To Color
    /// </summary>
    public static Color ToColor(this ConsoleColor value) {
      return s_Colors[(int)value];
    }

    #endregion Public
  }
}
