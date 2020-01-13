using System;

using Gloson;
using Gloson.Text;

namespace Demo.Gloson.Cmd {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Entry Point
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  class Program {
    #region Entry Point

    /// <summary>
    /// Entry Point
    /// </summary>
    static void Main(string[] args) {
      Console.WriteLine(Radix.ToDecimal("123(16)"));

      Console.ReadKey();
    }

    #endregion Entry Point
  }
}
