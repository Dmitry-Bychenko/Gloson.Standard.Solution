using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Consoles {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Console Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ConsoleHelper {
    #region Public

    /// <summary>
    /// Read Lines
    /// </summary>
    public static IEnumerable<string> ReadLines() {
      while (Console.KeyAvailable) 
        yield return Console.ReadLine();
    }

    #endregion Public
  }
}
