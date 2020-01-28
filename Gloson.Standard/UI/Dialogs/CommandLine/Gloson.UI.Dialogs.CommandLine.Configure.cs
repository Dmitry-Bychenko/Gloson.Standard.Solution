using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.UI.Dialogs.CommandLine {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Command Line Configure
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class CommandLineConfigure {
    #region Public

    /// <summary>
    /// Configure
    /// </summary>
    public static void Configure() {
      Dependencies.RegisterService(typeof(IAboutDialog), typeof(AboutDialog));
      Dependencies.RegisterService(typeof(IUnhandledExceptionDialog), typeof(UnhandledExceptionDialog));
      Dependencies.RegisterService(typeof(INetworkCredentialDialog), typeof(NetworkCredentialDialog));
    }

    #endregion Public
  }
}
