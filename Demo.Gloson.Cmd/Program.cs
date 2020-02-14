using System;
using System.Data;

using Gloson;
using Gloson.Data;
using Gloson.Data.Oracle;
using Gloson.Text;
using Gloson.Text.NaturalLanguages;
using Gloson.Text.NaturalLanguages.Library;
using Gloson.UI.Dialogs;
using Gloson.UI.Dialogs.CommandLine;
using Gloson.UI.CommandLine;

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
      Configuration.Apply();

      var data = Transliterations.GetTransliterations().Length;

      //int r = Transliterations.GetTransliterations().Length;

      Console.WriteLine(data);

      //Gloson.Text.NaturalLanguages.

      //Dependencies.CreateService<IAboutDialog>().Show();


      // IAboutDialog

      /*
      RdbmsOracle.Register();

      using (var conn = Rdbms.CreateConnection()) {
        var result = conn.ExecuteScalar(
          @"select Sysdate 
              from Dual");

        Console.WriteLine(result);
      }
      */

      Console.ReadKey();
    }

    #endregion Entry Point
  }
}
