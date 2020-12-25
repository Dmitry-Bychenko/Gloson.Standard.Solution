using System;
using System.Collections.Generic;
using System.IO;

namespace Gloson.Text {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Text Reader Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class TextReaderExtensions {
    #region Public

    /// <summary>
    /// As Enumerable
    /// </summary>
    public static IEnumerable<string> AsEnumerable(this TextReader reader) {
      if (reader is null)
        throw new ArgumentNullException(nameof(reader));

      while (true) {
        string line = reader.ReadLine();

        if (line is null)
          break;

        yield return line;
      }
    }

    #endregion Public
  }
}
