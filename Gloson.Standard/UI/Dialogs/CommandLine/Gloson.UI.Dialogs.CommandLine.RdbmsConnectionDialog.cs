using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
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

    /// <summary>
    /// Makes and Test connection string
    /// </summary>
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

        Console.WriteLine();

        if (string.IsNullOrWhiteSpace(serverName) &&
            string.IsNullOrWhiteSpace(login) &&
            string.IsNullOrWhiteSpace(password))
          return null;

        var cs = Dependencies.CreateService<IConnectionStringBuilder>();

        cs.Login = login;
        cs.Password = password;
        cs.Server = serverName;

        cs.IntegratedSecurity = string.IsNullOrEmpty(login) && string.IsNullOrEmpty(password);

        connection.ConnectionString = cs.ConnectionString;

        try {
          connection.Open();
          connection.Close();

          return cs.ConnectionString; 
        }
        catch (DbException) {
          ;
        }
      }
    }

    #endregion Public
  }
}
