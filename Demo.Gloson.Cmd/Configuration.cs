using System;
using System.Collections.Generic;
using System.Text;

using Gloson;
using Gloson.Data;
using Gloson.Data.Oracle;
using Gloson.Text;
using Gloson.UI.Dialogs;
using Gloson.UI.Dialogs.CommandLine;
using Gloson.UI.CommandLine;

namespace Demo.Gloson.Cmd {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Configuration
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Configuration {
    #region Algorithm

    private static void RegisterDependencies() {
      CommandLineConfigure.Configure();
      RdbmsOracle.Register();
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Apply
    /// </summary>
    public static void Apply() {
      RegisterDependencies();
    }

    #endregion Public
  }
}
