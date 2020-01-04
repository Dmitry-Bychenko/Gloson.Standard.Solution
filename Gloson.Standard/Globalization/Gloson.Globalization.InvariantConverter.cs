using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Gloson.Globalization {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Invariant Culture Standard Converter (Parser)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class InvariantConverter {
    #region Public

    public static bool TryParse(string value, out byte result) {
      if (null == value) {
        result = byte.MinValue;
        return false;
      }

      return byte.TryParse(value, NumberStyles.Any, CultureInfo.InvariantCulture, out result) ||
             byte.TryParse(value, NumberStyles.AllowHexSpecifier, CultureInfo.InvariantCulture, out result);
    }

    #endregion Public
  }
}
