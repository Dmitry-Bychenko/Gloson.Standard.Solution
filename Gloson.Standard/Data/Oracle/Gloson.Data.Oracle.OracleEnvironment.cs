using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Reflection;
using System.Text;

namespace Gloson.Data.Oracle {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Oracle Environment
  /// </summary>
  /// <seealso cref="https://docs.oracle.com/cd/B19306_01/server.102/b14200/functions165.htm#i1038176"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class OracleEnvironment {
    #region Private Data

    private readonly Dictionary<string, string> m_Cached = 
      new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    private Version m_Version = null;

    #endregion Private Data

    #region Algorithm

    private static Boolean IsBooleanTrue(String value) =>
      String.Equals(value, "y", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "t", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "1", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "+", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "+1", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "enable", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "enabled", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "yes", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "on", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "ok", StringComparison.OrdinalIgnoreCase) ||
      String.Equals(value, "true", StringComparison.OrdinalIgnoreCase);

    private string Query(string name) {
      string result;

      if (string.IsNullOrWhiteSpace(name))
        return "";

      name = name?.Trim();

      if (m_Cached.TryGetValue(name, out result))
        return result;

      using (IDbCommand q = Connection.CreateCommand()) {
        q.CommandText =
          @"select SYS_CONTEXT('USERENV', :prm_Name)
              from Dual";

        var prm = q.CreateParameter();

        prm.Direction = ParameterDirection.Input;
        prm.Value = name;
        prm.DbType = DbType.AnsiString;

        q.Parameters.Add(prm);

        result = Convert.ToString(q.ExecuteScalar());

        m_Cached.Add(name, result);

        return result;
      }
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="connection"></param>
    public OracleEnvironment(IDbConnection connection) {
      if (null == connection)
        throw new ArgumentNullException(nameof(connection));

      if (!(connection.State == ConnectionState.Open ||
            connection.State == ConnectionState.Connecting ||
            connection.State == ConnectionState.Executing ||
            connection.State == ConnectionState.Fetching))
        throw new ArgumentException("Connection is not opened", nameof(connection));

      Connection = connection;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Connection
    /// </summary>
    public IDbConnection Connection { get; }

    /// <summary>
    /// SID (Session Id)
    /// </summary>
    public long SID => long.Parse(Query("SID"));

    /// <summary>
    /// Audit Entry Id
    /// </summary>
    public long AuditEntryId => long.Parse(Query("ENTRYID"));

    /// <summary>
    /// Terminal Name
    /// </summary>
    public string TerminalName => Query("TERMINAL");

    /// <summary>
    /// Is DBA
    /// </summary>
    public bool IsDba => IsBooleanTrue(Query("ISDBA"));

    /// <summary>
    /// Region
    /// </summary>
    public RegionInfo Region => new RegionInfo(Query("LANG"));

    /// <summary>
    /// Version
    /// </summary>
    public Version Version {
      get {
        if (null != m_Version)
          return m_Version;

        using (IDbCommand q = Connection.CreateCommand()) {
          q.CommandText =
            @"SELECT Banner
                FROM v$version
               WHERE Banner LIKE 'Oracle%'";

          if (!Version.TryParse(q.ExecuteScalar()?.ToString(), out m_Version))
            m_Version = new Version(0, 0);

          return m_Version;
        }
      }
    }

    #endregion Public
  }

}
