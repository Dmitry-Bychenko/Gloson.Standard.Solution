using System;
using System.Text;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// StringBuilder Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class StringBuilderExtensions {
    #region Public

    /// <summary>
    /// Reverse (at place)
    /// </summary>
    public static void Reverse(this StringBuilder value) {
      if (value is null)
        throw new ArgumentNullException(nameof(value));

      for (int i = 0; i < value.Length / 2; ++i)
        (value[value.Length - 1 - i], value[i]) = (value[i], value[value.Length - 1 - i]);
    }

    #endregion Public
  }
}
