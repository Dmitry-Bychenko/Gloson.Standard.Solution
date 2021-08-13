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
  /// BitBacket Connection
  /// </summary>
  /// <seealso cref="https://developer.atlassian.com/server/bitbucket/reference/rest-api/"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BitBucketConnection {
    #region Private Data

    private static readonly CookieContainer s_CookieContainer;

    private static readonly HttpClient s_HttpClient;

    #endregion Private Data

    #region Algorithm
    #endregion Algorithm

    #region Create

    static BitBucketConnection() {
      try {
        ServicePointManager.SecurityProtocol =
          SecurityProtocolType.Tls |
          SecurityProtocolType.Tls11 |
          SecurityProtocolType.Tls12;
      }
      catch (NotSupportedException) {
        ;
      }

      s_CookieContainer = new CookieContainer();

      var handler = new HttpClientHandler() {
        CookieContainer = s_CookieContainer,
        Credentials = CredentialCache.DefaultCredentials,
      };

      // We create HTTP client with special cookie container to hold Atlassian cookie authentication
      s_HttpClient = new HttpClient(handler);
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public BitBucketConnection(string login, string password, string server) {
      Login = login ?? throw new ArgumentNullException(nameof(login));
      Password = password ?? throw new ArgumentNullException(nameof(password));
      Server = server?.Trim().TrimEnd('/') ?? throw new ArgumentNullException(nameof(server));

      Auth = $"Basic {Convert.ToBase64String(Encoding.UTF8.GetBytes($"{Login}:{Password}"))}";
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public BitBucketConnection(string login, string password) : this(login, password, null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Http Client
    /// </summary>
    public static HttpClient Client => s_HttpClient;

    /// <summary>
    /// Create Query
    /// </summary>
    public BitBucketQuery CreateQuery() => new(this);

    /// <summary>
    /// Login
    /// </summary>
    public string Login { get; }

    /// <summary>
    /// Password
    /// </summary>
    public string Password { get; }

    /// <summary>
    /// Authentification
    /// </summary>
    public string Auth { get; }

    /// <summary>
    /// Server
    /// </summary>
    public string Server { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"{Login}@{Server}";

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// BitBucket Query
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BitBucketQuery {
    #region Create

    public BitBucketQuery(BitBucketConnection connection) {
      Connection = connection ?? throw new ArgumentNullException(nameof(connection));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Connection
    /// </summary>
    public BitBucketConnection Connection { get; }

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

      address = string.Join("/", Connection.Server, "rest", address.TrimStart('/'));

      query ??= "{}";

      using var req = new HttpRequestMessage {
        Method = method,
        RequestUri = new Uri(address),
        Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
          { HttpRequestHeader.Authorization.ToString(), Connection.Auth},
        },
        Content = new StringContent(query, Encoding.UTF8, "application/json")
      };

      var response = await BitBucketConnection.Client.SendAsync(req, token).ConfigureAwait(false);

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
    /// Query
    /// </summary>
    /// <param name="address"></param>
    /// <param name="query"></param>
    /// <param name="method"></param>
    /// <returns></returns>
    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                HttpMethod method,
                                                                int pageSize,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token) {
      if (string.IsNullOrEmpty(address))
        throw new ArgumentNullException(nameof(address));

      if (pageSize <= 0)
        throw new ArgumentOutOfRangeException(nameof(pageSize));

      address = string.Join("/", Connection.Server, "rest", address.TrimStart('/'));

      if (address.Contains('?'))
        address += $"&limit={pageSize}";
      else
        address += $"?limit={pageSize}";

      query ??= "{}";

      int start = 0;

      while (start >= 0) {
        using var req = new HttpRequestMessage {
          Method = method,
          RequestUri = new Uri(address + (start == 0 ? "" : $"&start={start}")),
          Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
          { HttpRequestHeader.Authorization.ToString(), Connection.Auth},
        },
          Content = new StringContent(query, Encoding.UTF8, "application/json")
        };

        var response = await BitBucketConnection.Client.SendAsync(req, token).ConfigureAwait(false);

        if (!response.IsSuccessStatusCode)
          throw new DataException(string.IsNullOrEmpty(response.ReasonPhrase)
            ? $"Query failed with {response.StatusCode} ({(int)response.StatusCode}) code"
            : response.ReasonPhrase);

        using Stream stream = await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false);

        var jsonDocument = await JsonDocument.ParseAsync(stream, default, token).ConfigureAwait(false);

        if (jsonDocument.RootElement.TryGetProperty("nextPageStart", out var prop))
          start = prop.GetInt32();
        else
          start = -1;

        yield return jsonDocument;
      }
    }

    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                HttpMethod method,
                                                                int pageSize) {
      await foreach (var item in QueryPagedAsync(address, query, method, pageSize, CancellationToken.None))
        yield return item;
    }

    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                int pageSize,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token) {
      await foreach (var item in QueryPagedAsync(address, query, HttpMethod.Post, pageSize, token))
        yield return item;
    }

    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                string query,
                                                                int pageSize) {
      await foreach (var item in QueryPagedAsync(address, query, HttpMethod.Post, pageSize, CancellationToken.None))
        yield return item;
    }

    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                int pageSize,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token) {
      await foreach (var item in QueryPagedAsync(address, "", HttpMethod.Get, pageSize, token))
        yield return item;
    }

    public async IAsyncEnumerable<JsonDocument> QueryPagedAsync(string address,
                                                                int pageSize) {
      await foreach (var item in QueryPagedAsync(address, "", HttpMethod.Get, pageSize, CancellationToken.None))
        yield return item;
    }

    #endregion Public
  }

}
