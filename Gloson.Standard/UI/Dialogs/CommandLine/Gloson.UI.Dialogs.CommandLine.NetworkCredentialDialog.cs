using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Net;
using System.Text;

using Gloson.Consoles;

namespace Gloson.UI.Dialogs.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Network Credentials Dialog
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public class NetworkCredentialDialog : INetworkCredentialDialog {
    /// <summary>
    /// Add / Edit Credentilas
    /// </summary>
    public bool ShowDialog(string title, ref NetworkCredential credential) {
      if (null == credential)
        credential = new NetworkCredential(Environment.UserName, "");

      if (string.IsNullOrWhiteSpace(title))
        Console.WriteLine(title);

      Console.Write("Login:       ");
      string login = Console.ReadLine();

      Console.Write("Password:    ");
      string password = ConsoleReader.ReadPasswordLine('*');

      credential.UserName = login;
      credential.Password = password;

      if (string.IsNullOrWhiteSpace(login) && string.IsNullOrWhiteSpace(password))
        return false;
      else
        return true;
    }
  }
}
