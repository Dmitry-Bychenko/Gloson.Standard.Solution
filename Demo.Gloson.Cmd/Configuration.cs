using Gloson.Data.Oracle;
using Gloson.UI.Dialogs.CommandLine;

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
