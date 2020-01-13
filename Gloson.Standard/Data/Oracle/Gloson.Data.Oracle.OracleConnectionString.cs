using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Data.Oracle {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// OracleConnectionStringBuilder 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class OracleConnectionStringBuilder : IConnectionStringBuilder {
    #region Algorithm

    private IEnumerable<(string, string)> Parts() {
      if (!string.IsNullOrEmpty(Login))
        yield return ("User Id", Login);

      if (!string.IsNullOrEmpty(Password))
        yield return ("Password", Password);

      if (!string.IsNullOrEmpty(Server))
        yield return ("Data Source", Server);

      if (IntegratedSecurity)
        yield return ("Integrated Security", "SSPI");
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public OracleConnectionStringBuilder() { }

    #endregion Create

    #region IConnectionStringBuilder

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; set; }
    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; set; }
    /// <summary>
    /// Server
    /// </summary>
    public string Server { get; set; }
    /// <summary>
    /// Integrated Security
    /// </summary>
    public bool IntegratedSecurity { get; set; }

    /// <summary>
    /// Final Connection string
    /// </summary>
    public string ConnectionString => string.Join(";", Parts()
      .Select(tuple => $"{tuple.Item1}={tuple.Item2}"));

    /// <summary>
    /// Final Connection string
    /// </summary>
    public override string ToString() => ConnectionString;

    #endregion IConnectionStringBuilder
  }
}
