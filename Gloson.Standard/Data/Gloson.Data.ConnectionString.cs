using System;
using System.Collections.Generic;
using System.Linq;
using System.Data;
using System.Text;

namespace Gloson.Data {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Connection String
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IConnectionStringBuilder {
    /// <summary>
    /// Login
    /// </summary>
    string Login { get; set; }

    /// <summary>
    /// Password
    /// </summary>
    string Password { get; set; }

    /// <summary>
    /// Server
    /// </summary>
    string Server { get; set; }

    /// <summary>
    /// Integrated Security
    /// </summary>
    bool IntegratedSecurity { get; set; }

    /// <summary>
    /// Created Connection String
    /// </summary>
    string ConnectionString { get; }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Connection String Builder
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ConnectionStringBuilder {
    #region Private

    // Items
    private readonly Dictionary<string, string> m_Items = 
      new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

    #endregion Private

    #region Algorithm

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ConnectionStringBuilder(IConnectionStringBuilder builder) {
      if (null == builder)
        builder = Dependencies.GetService<IConnectionStringBuilder>();

      Builder = builder ?? throw new ArgumentNullException(nameof(builder));
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public ConnectionStringBuilder() 
      : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Builder
    /// </summary>
    public IConnectionStringBuilder Builder { get; }

    /// <summary>
    /// Options
    /// </summary>
    public IReadOnlyDictionary<string, string> Options => m_Items;

    /// <summary>
    /// With Login
    /// </summary>
    public ConnectionStringBuilder WithLogin(string login) {
      Builder.Login = login;

      return this;
    }

    /// <summary>
    /// With Password
    /// </summary>
    public ConnectionStringBuilder WithPassword(string password) {
      Builder.Password = password;

      return this;
    }
    
    /// <summary>
    /// With Server
    /// </summary>
    public ConnectionStringBuilder WithServer(string server) {
      Builder.Server = server;

      return this;
    }

    /// <summary>
    /// With Integrated Securiry
    /// </summary>
    public ConnectionStringBuilder WithIntegratedSecurity(bool integratedSecurity) {
      Builder.IntegratedSecurity = integratedSecurity;

      return this;
    }

    /// <summary>
    /// With Integrated Securiry
    /// </summary>
    public ConnectionStringBuilder WithIntegratedSecurity() => WithIntegratedSecurity(true);

    /// <summary>
    /// With Option
    /// </summary>
    public ConnectionStringBuilder WithOption(string name, string value) {
      if (null == name)
        throw new ArgumentNullException(nameof(name));

      if (string.IsNullOrWhiteSpace(name))
        throw new ArgumentException("Empty (or white space only) name is not allowed.", nameof(name));

      if (null == value)
        m_Items.Remove(name);
      else if (m_Items.ContainsKey(name))
        m_Items[name] = value;
      else
        m_Items.Add(name, value);

      return this;
    }

    /// <summary>
    /// Connection String
    /// </summary>
    public String ConnectionString {
      get {
        string basic = Builder.ConnectionString;

        string advanced = string.Join(";", m_Items
          .Where(pair => !string.IsNullOrWhiteSpace(pair.Key))
          .Where(pair => !string.IsNullOrWhiteSpace(pair.Value))
          .Select(pair => $"{pair.Key}={pair.Value}")
        );

        return string.Join(";", new string[] { basic, advanced}
          .Where(item => !string.IsNullOrWhiteSpace(item)));
      }
    }

    /// <summary>
    /// Connection String
    /// </summary>
    public override string ToString() => ConnectionString;

    #endregion Public
  }
}
