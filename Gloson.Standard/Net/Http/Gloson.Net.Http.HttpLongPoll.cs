using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Net.Http {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Http Long Poll
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class HttpLongPoll {
    #region Public

    /// <summary>
    /// Long Poll Read Lines
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(string address, 
                                                                string query, 
                                                                HttpMethod method,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token = default) {
      using var http = new HttpClient();

      http.Timeout = Timeout.InfiniteTimeSpan;

      using var request = new HttpRequestMessage {
        Method = method,
        RequestUri = new Uri(address),
        Headers = {
          { HttpRequestHeader.Accept.ToString(), "application/json" },
        },
        Content = new StringContent(query, Encoding.UTF8, "application/json")
      };

      using var response = await http
        .SendAsync(request, HttpCompletionOption.ResponseHeadersRead, token)
        .ConfigureAwait(false);

      using var reader = new StreamReader(await response.Content.ReadAsStreamAsync(token).ConfigureAwait(false));

      while (!reader.EndOfStream) {
        token.ThrowIfCancellationRequested();

        yield return await reader.ReadLineAsync().ConfigureAwait(false);
      }
    }

    /// <summary>
    /// Long Poll Read Lines
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(string address, 
                                                                string query, 
                                                                [EnumeratorCancellation]
                                                                CancellationToken token = default) {
      await foreach (var item in ReadLinesAsync(address, query, HttpMethod.Post, token).ConfigureAwait(false))
        yield return item;
    }

    /// <summary>
    /// Long Poll Read Lines
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(string address,
                                                                [EnumeratorCancellation]
                                                                CancellationToken token = default) {
      await foreach (var item in ReadLinesAsync(address, "", HttpMethod.Get).ConfigureAwait(false))
        yield return item;
    }

    #endregion Public
  }

}
