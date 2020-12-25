using Gloson.Consoles;
using System;
using System.Net;

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
      if (credential is null)
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
