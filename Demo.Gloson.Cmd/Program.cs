using System;
using System.Data;

using Gloson;
using Gloson.Data;
using Gloson.Data.Oracle;
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
      RdbmsOracle.Register();

      using (var conn = Rdbms.CreateConnection()) {
        var result = conn.ExecuteScalar(
          @"select Sysdate 
              from Dual");

        Console.WriteLine(result);
      }



        Console.ReadKey();
    }

    #endregion Entry Point
  }
}
