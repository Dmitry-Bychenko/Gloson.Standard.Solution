using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace Gloson.UI.Dialogs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// About Dialog Interface
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IAboutDialog {
    /// <summary>
    /// Show
    /// </summary>
    public void Show(int level);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// About Dialog Interface Extensions 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class AboutDialogExtensions {
    #region Public

    /// <summary>
    /// Show (with standard level == 0)
    /// </summary>
    public static void Show(this IAboutDialog dialog) {
      if (null == dialog)
        throw new ArgumentNullException(nameof(dialog));

      dialog.Show(0);
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Unhandled Exception Dialog
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IUnhandledExceptionDialog {
    void Show(Exception error);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ask For Network Credential
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface INetworkCredentialDialog {
    bool ShowDialog(string title, ref NetworkCredential credential);
  }
}
