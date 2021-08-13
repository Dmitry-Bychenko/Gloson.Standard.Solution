using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Services.Atlassian {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Jira Connection
  /// </summary>
  /// <seealso cref="https://docs.atlassian.com/software/jira/docs/api/REST/8.7.0/"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class JiraConnection : IDisposable {
    #region Private Data

    private static readonly Dictionary<char, string> s_Escape = new() {
      { '\\', "\\\\" },
      { '"', "\\\"" },
      { '\n', "\\n" },
      { '\r', "\\r" },
      { '\t', "\\t" },
      { '\f', "\\f" },
      { '\b', "\\b" },
    };

    private CookieContainer m_CookieContainer;

    private HttpClient m_HttpClient;

    #endregion Private Data

    #region Algorithm

    private static string Escape(string value) {
      if (value is null)
        return "null";

      StringBuilder sb = new(value.Length);

      foreach (char c in value)
        sb.Append(s_Escape.TryGetValue(c, out var s) ? s : c);

      return sb.ToString();
    }

    private void CoreCreateClient() {
      try {
        ServicePointManager.SecurityProtocol =
          SecurityProtocolType.Tls |
          SecurityProtocolType.Tls11 |
          SecurityProtocolType.Tls12;
      }
      catch (NotSupportedException) {
        ;
      }

      m_CookieContainer = new CookieContainer();

      var handler = new HttpClientHandler() {
        CookieContainer = m_CookieContainer,
        Credentials = CredentialCache.DefaultCredentials,
      };

      m_HttpClient = new HttpClient(handler);
    }

    private async Task CoreConnectAsync(CancellationToken token) {
      string query =
         @$"{{
             ""username"": ""{Escape(Login)}"",
             ""password"": ""{Escape(Password)}""
           }}";

      using var req = new HttpRequestMessage {
        Method = HttpMethod.Post,
        RequestUri = new Uri(string.Join('/', Server, "rest/auth/1/session")),
        Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
        },
        Content = new StringContent(query, Encoding.UTF8, "application/json")
      };

      var response = await m_HttpClient.SendAsync(req, token).ConfigureAwait(false);

      if (!response.IsSuccessStatusCode)
        throw new DataException($"Failed to Connect to Jira: {response.ReasonPhrase}");
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="login">Login</param>
    /// <param name="password">Password</param>
    /// <param name="server">Server, e.g. https://jira-my-server.com</param>
    public JiraConnection(string login, string password, string server) {
      Login = login ?? throw new ArgumentNullException(nameof(login));
      Password = password ?? throw new ArgumentNullException(nameof(password));
      Server = server ?? throw new ArgumentNullException(nameof(server));

      CoreCreateClient();
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JiraConnection(string login, string password) : this(login, password, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Http Client
    /// </summary>
    public HttpClient Client => m_HttpClient;

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Server
    /// </summary>
    public string Server { get; }

    /// <summary>
    /// Session Id
    /// </summary>
    public string SessionId {
      get {
        if (!IsConnected || IsDisposed)
          return null;

        var cookies = m_CookieContainer.GetCookies(new Uri(Server));

        return cookies["JSessionID"]?.Value;
      }
    }

    /// <summary>
    /// Token
    /// </summary>
    public string Token {
      get {
        if (!IsConnected || IsDisposed)
          return null;

        var cookies = m_CookieContainer.GetCookies(new Uri(Server));

        return cookies["atlassian.xsrf.token"]?.Value;
      }
    }

    /// <summary>
    /// Is Connected
    /// </summary>
    public bool IsConnected { get; private set; }

    /// <summary>
    /// Connect Async
    /// </summary>
    public async Task ConnectAsync(CancellationToken token) {
      if (IsDisposed)
        throw new ObjectDisposedException("this");

      if (IsConnected)
        return;

      await CoreConnectAsync(token).ConfigureAwait(false);

      IsConnected = true;
    }

    /// <summary>
    /// Connect Async
    /// </summary>
    public async Task ConnectAsync() => await ConnectAsync(CancellationToken.None).ConfigureAwait(false);

    /// <summary>
    /// Create Query
    /// </summary>
    public JiraQuery CreateQuery() {
      if (IsDisposed)
        throw new ObjectDisposedException("this");

      return new JiraQuery(this);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"{Login}@{Server}{(IsConnected ? " (connected)" : "")}";

    #endregion Public

    #region IDisposable

    /// <summary>
    /// Is Disposed
    /// </summary>
    public bool IsDisposed { get; private set; }

    // Dispose
    private void Dispose(bool disposing) {
      if (IsDisposed)
        return;

      if (disposing) {
        m_HttpClient.Dispose();

        IsDisposed = true;
        IsConnected = false;
      }
    }

    /// <summary>
    /// Dispose
    /// </summary>
    public void Dispose() => Dispose(true);

    #endregion IDisposable
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Jira Query
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class JiraQuery {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public JiraQuery(JiraConnection connection) {
      Connection = connection ?? throw new ArgumentNullException(nameof(connection));

      if (Connection.IsDisposed)
        throw new ObjectDisposedException(nameof(connection));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Query
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string address, string query, HttpMethod method, CancellationToken token) {
      if (string.IsNullOrEmpty(address))
        throw new ArgumentNullException(nameof(address));

      if (!Connection.IsConnected)
        throw new DataException("Not connected");

      address = string.Join("/", Connection.Server, "rest/api/latest", address.TrimStart('/'));

      query ??= "{}";

      using var req = new HttpRequestMessage {
        Method = method,
        RequestUri = new Uri(address),
        Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
        },
        Content = new StringContent(query, Encoding.UTF8, "application/json")
      };

      var response = await Connection.Client.SendAsync(req, token).ConfigureAwait(false);

      if (!response.IsSuccessStatusCode)
        throw new DataException(response.ReasonPhrase);

      using Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

      return await JsonDocument.ParseAsync(stream, default, token).ConfigureAwait(false);
    }

    /// <summary>
    /// Query
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string address, string query, HttpMethod method) =>
      await QueryAsync(address, query, method, CancellationToken.None).ConfigureAwait(false);

    /// <summary>
    /// Query
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string address, string query, CancellationToken token) =>
      await QueryAsync(address, query, HttpMethod.Post, token).ConfigureAwait(false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string address, string query) =>
      await QueryAsync(address, query, HttpMethod.Post, CancellationToken.None).ConfigureAwait(false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string address, CancellationToken token) =>
      await QueryAsync(address, "", HttpMethod.Get, token).ConfigureAwait(false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async Task<JsonDocument> QueryAsync(string address) =>
      await QueryAsync(address, "", HttpMethod.Get, CancellationToken.None).ConfigureAwait(false);

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="method"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                            string query,
                                                            HttpMethod method,
                                                            int pageSize,
                                                            [EnumeratorCancellation]
                                                                CancellationToken token) {
      if (string.IsNullOrEmpty(address))
        throw new ArgumentNullException(nameof(address));

      if (!Connection.IsConnected)
        throw new DataException("Not connected");

      address = string.Join("/", Connection.Server, "rest/api/latest", address.TrimStart('/'));

      address += $"{(address.Contains('?') ? '&' : '?')}&maxResults={pageSize}";

      query ??= "{}";
      int startAt = 0;

      while (startAt >= 0) {
        using var req = new HttpRequestMessage {
          Method = method,
          RequestUri = new Uri(address + $"&startAt={startAt}"),
          Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
        },
          Content = new StringContent(query, Encoding.UTF8, "application/json")
        };

        var response = await Connection.Client.SendAsync(req, token).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
          throw new DataException(response.ReasonPhrase);

        using Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        var jsonDocument = await JsonDocument.ParseAsync(stream, default, token).ConfigureAwait(false);

        if (jsonDocument.RootElement.TryGetProperty("startAt", out var startAtItem)) {
          using var en = jsonDocument.RootElement.EnumerateObject();

          while (en.MoveNext()) {
            if (en.Current.Value.ValueKind == JsonValueKind.Array) {
              if (en.Current.Value.GetArrayLength() <= 0) {
                yield break;
              }
            }
          }

          yield return jsonDocument;

          startAt += pageSize;
        }
        else {
          yield return jsonDocument;

          startAt = 0;
        }
      }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="method"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                HttpMethod method,
                                                                int pageSize) {
      await foreach (var item in QueryPagedAsync(address, query, method, pageSize, CancellationToken.None))
        yield return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                int pageSize,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token) {
      await foreach (var item in QueryPagedAsync(address, query, HttpMethod.Post, pageSize, token))
        yield return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                int pageSize) {
      await foreach (var item in QueryPagedAsync(address, query, HttpMethod.Post, pageSize, CancellationToken.None))
        yield return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="pageSize"></param>
    /// <param name="token"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                int pageSize,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token) {
      await foreach (var item in QueryPagedAsync(address, "", HttpMethod.Get, pageSize, token))
        yield return item;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="address"></param>
    /// <param name="pageSize"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                int pageSize) {
      await foreach (var item in QueryPagedAsync(address, "", HttpMethod.Get, pageSize, CancellationToken.None))
        yield return item;
    }

    /// <summary>
    /// Connection
    /// </summary>
    public JiraConnection Connection { get; }

    #endregion Public
  }

}
