using Gloson.UI.Dialogs;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace Gloson.Net {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Credential ID
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class NetworkCredentialRecord : IEquatable<NetworkCredentialRecord> {
    #region Create

    /// <summary>
    /// Credentails ID standard constructor 
    /// </summary>
    /// <param name="uri"></param>
    /// <param name="authType"></param>
    internal NetworkCredentialRecord(Uri uri, string authType) {
      Address = uri ?? new Uri("file://localhost/");

      AuthenicationType = string.IsNullOrWhiteSpace(authType)
        ? "Basic"
        : authType.Trim();
    }

    /// <summary>
    /// Credentials ID standard constructor
    /// </summary>
    internal NetworkCredentialRecord(Uri uri)
      : this(uri, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Address
    /// </summary>
    public Uri Address { get; }

    /// <summary>
    /// Authenication Type
    /// </summary>
    public string AuthenicationType { get; }

    /// <summary>
    /// To String 
    /// </summary>
    public override string ToString() => $"{Address} ({AuthenicationType})";

    #endregion Public

    #region IEquatable<CredentialsId>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(NetworkCredentialRecord other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (null == other)
        return false;

      return Address == other.Address &&
             string.Equals(AuthenicationType, other.AuthenicationType, StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => Equals(obj as NetworkCredentialRecord);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() =>
      Address.GetHashCode() ^ StringComparer.OrdinalIgnoreCase.GetHashCode(AuthenicationType);

    #endregion IEquatable<CredentialsId>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Credentials 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class NetworkCredentials : ICredentials {
    #region Private Data

    private readonly ConcurrentDictionary<NetworkCredentialRecord, NetworkCredential> m_Cache =
      new ConcurrentDictionary<NetworkCredentialRecord, NetworkCredential>();

    #endregion Private Data

    #region Algorithm

    private static NetworkCredential Clone(NetworkCredential value) {
      if (null == value)
        return value;

      return new NetworkCredential(value.UserName, value.Password, value.Domain);
    }

    #endregion Algorithm

    #region Create

    static NetworkCredentials() {
      Default = new NetworkCredentials();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Default
    /// </summary>
    public static NetworkCredentials Default { get; }

    /// <summary>
    /// Local Host (Workstation)
    /// </summary>
    public NetworkCredential WorkStation => GetCredential(null);

    /// <summary>
    /// Add Or Update
    /// </summary>
    public bool Add(Uri uri, string authType, NetworkCredential credential) {
      if (null == credential)
        return Remove(uri, authType);
      else {
        if (null == uri)
          return false;

        NetworkCredentialRecord id = new NetworkCredentialRecord(uri, authType);

        bool result = true;

        m_Cache.AddOrUpdate(
          id,
          Clone(credential),
          (key, oldValue) => {
            result = false;

            return Clone(credential);
          });

        return result;
      }
    }

    /// <summary>
    /// Add Or Update
    /// </summary>
    public bool Add(Uri uri, NetworkCredential credential) => Add(uri, null, credential);

    /// <summary>
    /// Add From UI
    /// </summary>
    public bool AddFromUI(string title, Uri uri, string authType) {
      NetworkCredential result = null;

      if (string.IsNullOrEmpty(title))
        title = uri == null ? "Local Host Credentials" : uri.ToString();

      var dialog = Dependencies.CreateService<INetworkCredentialDialog>();

      if (!dialog.ShowDialog(title, ref result))
        return false;

      return Add(uri, authType, result);
    }

    /// <summary>
    /// Add From UI
    /// </summary>
    public bool AddFromUI(string title, Uri uri) => AddFromUI(title, uri, null);

    /// <summary>
    /// Add From UI
    /// </summary>
    public bool AddFromUI(Uri uri) => AddFromUI(null, uri, null);

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(Uri uri, string authType) {
      NetworkCredentialRecord id = new NetworkCredentialRecord(uri, authType);

      return m_Cache.TryRemove(id, out var _);
    }

    /// <summary>
    /// Remove
    /// </summary>
    public bool Remove(Uri uri) => Remove(uri, null);

    /// <summary>
    /// All Records
    /// </summary>
    public KeyValuePair<NetworkCredentialRecord, NetworkCredential>[] AllRecords => m_Cache
      .Select(pair => new KeyValuePair<NetworkCredentialRecord, NetworkCredential>(pair.Key, Clone(pair.Value)))
      .ToArray();

    #endregion Public

    #region ICredentials

    /// <summary>
    /// Obtain Credentials 
    /// </summary>
    /// <param name="uri">Uri</param>
    /// <param name="authType">Authenication type</param>
    public NetworkCredential GetCredential(Uri uri, string authType) {
      NetworkCredentialRecord id = new NetworkCredentialRecord(uri, authType);

      if (m_Cache.TryGetValue(id, out NetworkCredential result))
        return Clone(result);

      return null;
    }

    /// <summary>
    /// Obtain Credentials 
    /// </summary>
    public NetworkCredential GetCredential(Uri uri) => GetCredential(uri, null);

    /// <summary>
    /// Get Or Read (UI) Credentials
    /// </summary>
    public NetworkCredential GetOrReadCredential(string title, Uri uri, string authType) {
      NetworkCredential result = GetCredential(uri, authType);

      if (null != result)
        return result;

      if (string.IsNullOrEmpty(title))
        title = uri == null ? "Local Host Credentials" : uri.ToString();

      var dialog = Dependencies.CreateService<INetworkCredentialDialog>();

      if (dialog.ShowDialog(title, ref result))
        return Clone(result);
      else
        return null;
    }

    /// <summary>
    /// Get Or Read (UI) Credentials
    /// </summary>
    public NetworkCredential GetOrReadCredential(string title, Uri uri) =>
      GetOrReadCredential(title, uri, null);

    /// <summary>
    /// Get Or Read (UI) Credentials
    /// </summary>
    public NetworkCredential GetOrReadCredential(string title) =>
      GetOrReadCredential(title, null, null);

    /// <summary>
    /// Get Or Read (UI) Credentials
    /// </summary>
    public NetworkCredential GetOrReadCredential(Uri uri) =>
      GetOrReadCredential(null, uri, null);

    /// <summary>
    /// Get Or Read (UI) Credentials
    /// </summary>
    public NetworkCredential GetOrReadCredential() =>
      GetOrReadCredential(null, null, null);

    #endregion ICredentials
  }
}
