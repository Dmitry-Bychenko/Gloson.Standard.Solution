using System;
using System.Data;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Gloson.Data.Oracle {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// RDBMS Oracle Entry Point
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class RdbmsOracle {
    #region Private Data
    #endregion Private Data

    #region Algorithm

    private static string ComputeAssemblyName() {
      return "Oracle.ManagedDataAccess";

      /*
      Assembly assembly = Assembly.GetEntryAssembly();

      if (null == assembly)
        assembly = Assembly.GetExecutingAssembly();

      var framework = assembly.GetCustomAttribute<TargetFrameworkAttribute>();

      bool isCore = (null != framework) &&
                    (framework.FrameworkName.IndexOf("Core", StringComparison.OrdinalIgnoreCase) >= 0);

      return isCore
        ? "Oracle.ManagedDataAccess.Core"
        : "Oracle.ManagedDataAccess";
      */
    }

    private static Type ConnectionType() {
      if (null == AccessAssembly)
        return null;

      return AccessAssembly
        .GetTypes()
        .Where(t => !t.IsAbstract && t.IsPublic)
        .Where(t => t.GetInterfaces().Any(itf => itf == typeof(IDbConnection)))
        .FirstOrDefault();
    }

    #endregion Algorithm

    #region Create

    static RdbmsOracle() {
      AccessAssemblyName = ComputeAssemblyName();

      try {
        string savedDir = Environment.CurrentDirectory;

        try {
          Environment.CurrentDirectory = Path.GetDirectoryName(Assembly.GetEntryAssembly().Location);

          string path = Path.Combine(
            Path.GetDirectoryName(Assembly.GetEntryAssembly().Location),
            AccessAssemblyName + ".dll");

          Assembly asm = Assembly.LoadFile(path);
        }
        finally {
          Environment.CurrentDirectory = savedDir;
        }
      }
      catch (FileNotFoundException) {; }
      catch (IOException) {; }
      catch (BadImageFormatException) {; }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Register
    /// </summary>
    public static void Register(string connectionString) {
      Dependencies.RegisterService(
        typeof(IConnectionStringBuilder),
        typeof(OracleConnectionStringBuilder));

      Dependencies.TryRegisterService(
        typeof(IRdbmsConnectionDialog),
        typeof(Gloson.UI.Dialogs.CommandLine.RdbmsConnectionDialog));

      Rdbms.Register(ConnectionType(), connectionString);
    }

    /// <summary>
    /// Register
    /// </summary>
    public static void Register() => Register(null);

    /// <summary>
    /// Access Assembly Name
    /// </summary>
    public static string AccessAssemblyName { get; }

    /// <summary>
    /// Access Assembly
    /// </summary>
    public static Assembly AccessAssembly =>
      AppDomain
        .CurrentDomain
        .GetAssemblies()
        .FirstOrDefault(asm => string.Equals(asm.GetName().Name,
                                             AccessAssemblyName,
                                             StringComparison.OrdinalIgnoreCase));

    #endregion Public
  }

}
