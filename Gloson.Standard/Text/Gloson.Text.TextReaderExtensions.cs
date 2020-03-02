using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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
      if (null == reader)
        throw new ArgumentNullException(nameof(reader));

      while (true) {
        string line = reader.ReadLine();

        if (null == line)
          break;

        yield return line;
      }
    }

    #endregion Public
  }
}
