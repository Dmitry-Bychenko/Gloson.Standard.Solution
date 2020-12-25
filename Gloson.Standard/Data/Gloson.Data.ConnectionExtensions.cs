using System;
using System.Collections.Generic;
using System.Data;

namespace Gloson.Data {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Connection Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DbConnectionExtensions {
    #region Public

    /// <summary>
    /// Execute Non Query
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">Parameters</param>
    /// <returns>Rows Affected</returns>
    public static int ExecuteNonQuery(this IDbConnection connection,
                                           string sql,
                                   params (string, object)[] parameters) {
      if (connection is null)
        throw new ArgumentNullException(nameof(connection));
      else if (sql is null)
        throw new ArgumentNullException(nameof(sql));
      else if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      using IDbCommand q = connection.CreateCommand();

      q.CommandText = sql;

      foreach (var item in parameters) {
        IDbDataParameter prm = q.CreateParameter();

        prm.ParameterName = item.Item1;
        prm.Value = item.Item2;
      }

      return q.ExecuteNonQuery();
    }

    /// <summary>
    /// Execute Scalar
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">Parameters</param>
    public static object ExecuteScalar(this IDbConnection connection,
                                            string sql,
                                    params (string, object)[] parameters) {
      if (connection is null)
        throw new ArgumentNullException(nameof(connection));
      else if (sql is null)
        throw new ArgumentNullException(nameof(sql));
      else if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      using IDbCommand q = connection.CreateCommand();

      q.CommandText = sql;

      foreach (var item in parameters) {
        IDbDataParameter prm = q.CreateParameter();

        prm.ParameterName = item.Item1;
        prm.Value = item.Item2;
      }

      return q.ExecuteScalar();
    }

    /// <summary>
    /// Execute Scalar
    /// </summary>
    /// <param name="connection">Connection</param>
    /// <param name="sql">SQL</param>
    /// <param name="parameters">Parameters</param>
    public static IEnumerable<IDataRecord> ExecuteEnumerable(this IDbConnection connection,
                                                             string sql,
                                                     params (string, object)[] parameters) {
      if (connection is null)
        throw new ArgumentNullException(nameof(connection));
      else if (sql is null)
        throw new ArgumentNullException(nameof(sql));
      else if (parameters is null)
        throw new ArgumentNullException(nameof(parameters));

      using IDbCommand q = connection.CreateCommand();

      q.CommandText = sql;

      foreach (var item in parameters) {
        IDbDataParameter prm = q.CreateParameter();

        prm.ParameterName = item.Item1;
        prm.Value = item.Item2;
      }

      using var reader = q.ExecuteReader();

      while (reader.Read())
        yield return reader;
    }

    #endregion Public
  }
}
