using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Gloson.Ini;
using Gloson.Text;

namespace Gloson.Data.Oracle.Client {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Oracle Home
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class OracleHome {
    #region Private Data

    private Lazy<TnsNames> m_TnsNames;

    private Lazy<IReadOnlyDictionary<string, string>> m_Ldap;

    private Lazy<IReadOnlyDictionary<string, string>> m_SqlNet;

    #endregion Private Data

    #region Algotithm

    private static IReadOnlyDictionary<string, string> ReadAsDictionary(string fileName) {
      IniDocument doc = IniDocument.Load(File.ReadLines(fileName));

      Dictionary<string, string> result = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);

      foreach (var section in doc.Sections) {
        foreach (var record in section.Records) {
          if (result.ContainsKey(record.Name))
            result[record.Name] = record.Value;
          else
            result.Add(record.Name, record.Value);
        }
      }

      return result;
    }

    #endregion Algotithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="directoryName"></param>
    public OracleHome(string directoryName) {
      if (null == directoryName)
        throw new ArgumentNullException(nameof(directoryName));

      ClientPath = directoryName;

      string netWork = Path.Combine(ClientPath, "network", "admin");

      m_TnsNames = new Lazy<TnsNames>(() => TnsNames.Load(Path.Combine(netWork, "tnsnames.ora")));

      m_Ldap = new Lazy<IReadOnlyDictionary<string, string>>(() => ReadAsDictionary(Path.Combine(netWork, "ldap.ora")));
      m_SqlNet = new Lazy<IReadOnlyDictionary<string, string>>(() => ReadAsDictionary(Path.Combine(netWork, "sqlnet.ora")));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Client Path
    /// </summary>
    public string ClientPath { get; }

    /// <summary>
    /// Tns Names
    /// </summary>
    public TnsNames TnsNames => m_TnsNames.Value;

    /// <summary>
    /// Ldap
    /// </summary>
    public IReadOnlyDictionary<string, string> Ldap => m_Ldap.Value;

    /// <summary>
    /// Ldap
    /// </summary>
    public IReadOnlyDictionary<string, string> SqlNet => m_SqlNet.Value;

    #endregion Public
  }
}
