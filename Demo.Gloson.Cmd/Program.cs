using System;

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

  internal class Program {
    #region Entry Point

    /// <summary>
    /// Entry Point
    /// </summary>
    private static void Main() {
      ITest xxx = new MyClass() { MyInt = 1 };

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
