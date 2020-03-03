using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using Gloson;
using Gloson.Text;

namespace Gloson.Services.Quandl {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Quandl Query
  /// </summary>
  /// https://help.quandl.com/article/320-where-can-i-find-my-api-key
  //
  // Demo: QuandlDataSet.Query("WIKI/AAPL")
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class QuandlDataSet {
    #region Public

    /// <summary>
    /// Api Key
    /// </summary>
    // https://help.quandl.com/article/320-where-can-i-find-my-api-key  
    public static string ApiKey {
      get;
      set;
    }

    /// <summary>
    /// Query
    /// </summary>
    public static IEnumerable<string[]> Query(string dataset, string apiKey, int version) {
      if (null == dataset)
        throw new ArgumentNullException(nameof(dataset));
      else if (version <= 0)
        version = 3;

      string address = $"https://www.quandl.com/api/v{version}/datasets/{dataset}.csv";

      if (string.IsNullOrEmpty(apiKey))
        apiKey = ApiKey;

      if (!string.IsNullOrEmpty(apiKey))
        address += $"?api_key={apiKey}";

      HttpClient httpClient = Dependencies.GetServiceRequired<HttpClient>();

      using var response = httpClient.GetAsync(address).Result;
      using Stream stream = response.Content.ReadAsStreamAsync().Result;

      foreach (var record in CommaSeparatedValues.ParseCsv(stream, ',', '"', Encoding.UTF8))
        yield return record;
    }

    /// <summary>
    /// Query
    /// </summary>
    public static IEnumerable<string[]> Query(string dataset, string apiKey) =>
      Query(dataset, apiKey, 3);

    /// <summary>
    /// Query
    /// </summary>
    public static IEnumerable<string[]> Query(string dataset) =>
      Query(dataset, null, 3);

    /// <summary>
    /// Query
    /// </summary>
    public static async Task<string[][]> QueryAsync(string dataset, string apiKey, int version) {
      if (null == dataset)
        throw new ArgumentNullException(nameof(dataset));
      else if (version <= 0)
        version = 3;

      string address = $"https://www.quandl.com/api/v{version}/datasets/{dataset}.csv";

      if (string.IsNullOrEmpty(apiKey))
        apiKey = ApiKey;

      if (!string.IsNullOrEmpty(apiKey))
        address += $"?api_key={apiKey}";

      HttpClient httpClient = Dependencies.GetServiceRequired<HttpClient>();

      using var response = await httpClient.GetAsync(address).ConfigureAwait(false);
      using Stream stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false);

      return await Task<string[][]>.Run(() => CommaSeparatedValues
        .ParseCsv(stream, ',', '"', Encoding.UTF8)
        .ToArray()).ConfigureAwait(false);

      //return CommaSeparatedValues.ParseCsv(stream, ',', '"', Encoding.UTF8).ToArray();
    }

    /// <summary>
    /// Query
    /// </summary>
    public static async Task<string[][]> QueryAsync(string dataset, string apiKey)
      => await QueryAsync(dataset, apiKey, 3).ConfigureAwait(false);

    /// <summary>
    /// Query
    /// </summary>
    public static async Task<string[][]> QueryAsync(string dataset)
      => await QueryAsync(dataset, null, 3).ConfigureAwait(false);

    #endregion Public
  }

}
