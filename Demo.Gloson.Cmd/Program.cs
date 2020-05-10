using System;
using System.Data;

using Gloson;
using Gloson.Data;
using Gloson.Data.Oracle;

using Gloson.Games.TicTacToe;

using Gloson.Text;
using Gloson.Text.NaturalLanguages;
using Gloson.Text.NaturalLanguages.Library;
using Gloson.UI.Dialogs;
using Gloson.UI.Dialogs.CommandLine;
using Gloson.UI.CommandLine;

namespace Demo.Gloson.Cmd {

  public interface ITest {
    int MyInt { get; set; }
    int GetItNow() => MyInt * 2;
  }

  public class MyClass : ITest {
    public int MyInt { get; set; }

    public int GetItNow() => MyInt * 3;
  }

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
    static void Main() {
      ITest xxx = new MyClass() { MyInt = 1};

      Console.Write(xxx.GetItNow());

      //Configuration.Apply();

      //Console.WriteLine(TicTacToePosition.Empty.MoveNumber);

      //var data = Transliterations.GetTransliterations().Length;

      //int r = Transliterations.GetTransliterations().Length;

      //Console.WriteLine(data);

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
