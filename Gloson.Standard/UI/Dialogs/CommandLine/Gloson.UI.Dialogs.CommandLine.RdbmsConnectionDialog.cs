using System;
using System.Collections.Generic;
using System.Data;
using System.Text;

using Gloson.Consoles;
using Gloson.Data;

namespace Gloson.UI.Dialogs.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// RdbmsConnectionDialog
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class RdbmsConnectionDialog : IRdbmsConnectionDialog {
    #region Public

    //-------------------------------------------------------------------------------------------------------------------
    //
    /// <summary>
    /// Makes and Tests connection string
    /// </summary>
    // 
    //-------------------------------------------------------------------------------------------------------------------

    public string ConnectionString(IDbConnection connection) {
      if (null == connection)
        return null;
      else if (connection.State == ConnectionState.Open ||
               connection.State == ConnectionState.Connecting ||
               connection.State == ConnectionState.Executing ||
               connection.State == ConnectionState.Fetching)
        return connection.ConnectionString;

      while (true) {
        Console.Write("Server Name: ");

        string serverName = Console.ReadLine();

        Console.Write("Login:       ");

        string login = Console.ReadLine();

        Console.Write("Password:    ");

        string password = ConsoleReader.ReadPasswordLine('*');

        if (string.IsNullOrWhiteSpace(serverName) &&
            string.IsNullOrWhiteSpace(login) &&
            string.IsNullOrWhiteSpace(password))
          return null;

        //TODO: Implement Me!
      }
    }

    #endregion Public
  }
}
