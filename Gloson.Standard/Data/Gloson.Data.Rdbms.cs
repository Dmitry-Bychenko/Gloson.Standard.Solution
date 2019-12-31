using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace Gloson.Data {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// DB Connection Dialog
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IRdbmsConnectionDialog {
    /// <summary>
    /// Connection string or null if not connected
    /// </summary>
    string ConnectionString(IDbConnection connection);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Connection
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Rdbms {
    #region Private Data

    private static String s_ConnectionString = "";

    private static object s_SyncObj = new Object();

    #endregion Private Data

    #region Algorithm

    private static void CoreClearAll() {
      Dependencies.Services.RemoveAll(typeof(IDbConnection));
      Dependencies.Services.RemoveAll(typeof(IDbTransaction));
      Dependencies.Services.RemoveAll(typeof(IDbCommand));
      Dependencies.Services.RemoveAll(typeof(IDbDataParameter));

      Dependencies.Services.RemoveAll(typeof(DbProviderFactory));
      Dependencies.Services.RemoveAll(typeof(IDbDataAdapter));
    }

    private static void CoreRegister(Type connectionType) {
      Type transactionType = connectionType
          .GetMethods(BindingFlags.Public | BindingFlags.Instance)
          .FirstOrDefault(mi => "BeginTransaction".Equals(mi.Name))
         ?.ReturnType;

      Type commandType = connectionType
          .GetMethods(BindingFlags.Public | BindingFlags.Instance)
          .FirstOrDefault(mi => "CreateCommand".Equals(mi.Name))
         ?.ReturnType;

      Type parameterType = commandType
        ?.GetMethods(BindingFlags.Public | BindingFlags.Instance)
        ?.FirstOrDefault(mi => "CreateParameter".Equals(mi.Name))
        ?.ReturnType;

      Type providerFactoryType = connectionType
        .Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract && t.IsPublic)
        .Where(t => t.IsSubclassOf(typeof(DbProviderFactory)))
        .FirstOrDefault();

      Type adapterType = connectionType
        .Assembly
        .GetTypes()
        .Where(t => !t.IsAbstract && t.IsPublic)
        .Where(t => t.GetInterfaces().Any(itf => itf == typeof(IDbDataAdapter)))
        .FirstOrDefault();
      
      if (transactionType != null)
        Dependencies.Services.Add(new ServiceDescriptor(
          typeof(IDbTransaction),
          transactionType,
          ServiceLifetime.Transient));

      if (commandType != null)
        Dependencies.Services.Add(new ServiceDescriptor(
          typeof(IDbCommand),
          commandType,
          ServiceLifetime.Transient));

      if (parameterType != null)
        Dependencies.Services.Add(new ServiceDescriptor(
          typeof(IDbDataParameter),
          parameterType,
          ServiceLifetime.Transient));

      if (providerFactoryType != null)
        Dependencies.Services.Add(new ServiceDescriptor(
          typeof(DbProviderFactory),
          providerFactoryType,
          ServiceLifetime.Transient));

      if (adapterType != null)
        Dependencies.Services.Add(new ServiceDescriptor(
          typeof(IDbDataAdapter),
          adapterType,
          ServiceLifetime.Transient));
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Register Connection
    /// </summary>
    /// <param name="connectionType">Connection Type</param>
    /// <param name="connectionString">Connection String</param>
    public static void Register(Type connectionType, string connectionString) {
      if (null == connectionType)
        throw new ArgumentNullException(nameof(connectionType));
      else if (connectionType.IsAbstract || connectionType.IsInterface)
        throw new ArgumentException($"Class {connectionType.Name} must not be abstract", nameof(connectionType));
      else if (!connectionType.GetInterfaces().Any(itf => typeof(IDbConnection) == itf))
        throw new ArgumentException($"Class {connectionType.Name} must implement {typeof(IDbConnection).Name}", nameof(connectionType));

      if (string.IsNullOrWhiteSpace(connectionString)) {
        Register(connectionType);

        return;
      }

      lock (s_SyncObj) {
        CoreClearAll();

        ServiceDescriptor descriptor = new ServiceDescriptor(
          typeof(IDbConnection),
          (provider) => {
            IDbConnection result = Activator.CreateInstance(connectionType, connectionString) as IDbConnection;

          //result.Open();

          return result;
          },
          ServiceLifetime.Transient);

        Dependencies.Services.Add(descriptor);

        CoreRegister(connectionType);

        Interlocked.Exchange(ref s_ConnectionString, connectionString);
      }
    }

    /// <summary>
    /// Register
    /// </summary>
    /// <param name="connectionType">Connection Type</param>
    public static void Register(Type connectionType) {
      if (null == connectionType)
        throw new ArgumentNullException(nameof(connectionType));
      else if (connectionType.IsAbstract || connectionType.IsInterface)
        throw new ArgumentException($"Class {connectionType.Name} must not be abstract", nameof(connectionType));
      else if (!connectionType.GetInterfaces().Any(itf => typeof(IDbConnection) == itf))
        throw new ArgumentException($"Class {connectionType.Name} must implement {typeof(IDbConnection).Name}", nameof(connectionType));

      lock (s_SyncObj) {
        CoreClearAll();

        ServiceDescriptor descriptor = new ServiceDescriptor(
          typeof(IDbConnection),
          (provider) => {
            IDbConnection result = Activator.CreateInstance(connectionType) as IDbConnection;

            var dialog = Dependencies.Provider.GetService<IRdbmsConnectionDialog>();

            if (dialog != null) {
              string connectionString = dialog.ConnectionString(result);

              if (!string.IsNullOrWhiteSpace(connectionString)) {
                Register(connectionType, connectionString);

                result.ConnectionString = connectionString;
              }
            }

            return result;
          },
          ServiceLifetime.Transient);

        Dependencies.Services.Add(descriptor);

        CoreRegister(connectionType);

        Interlocked.Exchange(ref s_ConnectionString, "");
      }
    }

    /// <summary>
    /// Create Connection
    /// </summary>
    public static IDbConnection CreateConnection() {
      return Dependencies.Provider.GetRequiredService<IDbConnection>();
    }

    /// <summary>
    /// Connect (Create Opened Connection)
    /// </summary>
    public static IDbConnection Connect(string connectionString) {
      IDbConnection result = Dependencies.Provider.GetRequiredService<IDbConnection>();

      if (!string.IsNullOrWhiteSpace(connectionString))
        result.ConnectionString = connectionString;

      if (!string.IsNullOrWhiteSpace(result.ConnectionString))
        result.Open();

      return result;
    }

    /// <summary>
    /// Connect (Create Opened Connection)
    /// </summary>
    public static IDbConnection Connect() => Connect(null);

    /// <summary>
    /// Provider Factory
    /// </summary>
    public static DbProviderFactory ProviderFactory() {
      return Dependencies.Provider.GetRequiredService<DbProviderFactory>();
    }

    /// <summary>
    /// Adapter
    /// </summary>
    public static IDbDataAdapter Adapter() {
      return Dependencies.Provider.GetRequiredService<IDbDataAdapter>();
    }

    /// <summary>
    /// Connection string
    /// </summary>
    public static string ConnectionString => s_ConnectionString;

    /// <summary>
    /// Execute Non Query
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">Parameters</param>
    public static int ExecuteNonQuery(string sql, params (string, object)[] parameters) {
      if (null == sql)
        throw new ArgumentNullException(nameof(sql));
      else if (null == parameters)
        throw new ArgumentNullException(nameof(parameters));

      using (IDbConnection conn = Connect()) {
        using (IDbCommand q = conn.CreateCommand()) {
          q.CommandText = sql;

          foreach (var item in parameters) {
            IDbDataParameter prm = q.CreateParameter();

            prm.ParameterName = item.Item1;
            prm.Value = item.Item2;
          }

          return q.ExecuteNonQuery();
        }
      }
    }

    /// <summary>
    /// Execute Scalar
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">Parameters</param>
    public static Object ExecuteScalar(string sql, params (string, object)[] parameters) {
      if (null == sql)
        throw new ArgumentNullException(nameof(sql));
      else if (null == parameters)
        throw new ArgumentNullException(nameof(parameters));

      using (IDbConnection conn = Connect()) {
        using (IDbCommand q = conn.CreateCommand()) {
          q.CommandText = sql;

          foreach (var item in parameters) {
            IDbDataParameter prm = q.CreateParameter();

            prm.ParameterName = item.Item1;
            prm.Value = item.Item2;
          }

          return q.ExecuteScalar();
        }
      }
    }

    /// <summary>
    /// Execute Scalar
    /// </summary>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">Parameters</param>
    public static IEnumerable<IDataRecord> ExecuteEnumerable(string sql, params (string, object)[] parameters) {
      if (null == sql)
        throw new ArgumentNullException(nameof(sql));
      else if (null == parameters)
        throw new ArgumentNullException(nameof(parameters));

      using (IDbConnection conn = Connect()) {
        using (IDbCommand q = conn.CreateCommand()) {
          q.CommandText = sql;

          foreach (var item in parameters) {
            IDbDataParameter prm = q.CreateParameter();

            prm.ParameterName = item.Item1;
            prm.Value = item.Item2;
          }

          using (var reader = q.ExecuteReader()) {
            while (reader.Read()) {
              yield return reader;
            }
          }
        }
      }
    }

    #endregion Public
  }

}
