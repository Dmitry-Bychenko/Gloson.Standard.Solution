using Gloson.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace Gloson.Net.Http {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Http Data
  /// </summary>
  //
  // https://github.com/astronexus/HYG-Database
  // https://raw.githubusercontent.com/astronexus/HYG-Database/master/hygdata_v3.csv
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class HttpData {
    #region Public

    /// <summary>
    /// Client
    /// </summary>
    public static HttpClient Client => Dependencies.GetServiceRequired<HttpClient>();

    #region Sync

    /// <summary>
    /// Read Lines
    /// </summary>
    public static IEnumerable<string> ReadLines(string address, Encoding encoding = null) {
      if (address is null)
        throw new ArgumentNullException(nameof(address));

      using var reader = encoding is null
        ? new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      for (string line = reader.ReadLine(); line is not null; line = reader.ReadLine())
        yield return line;
    }

    /// <summary>
    /// Read Csv
    /// </summary>
    public static IEnumerable<string[]> ReadCsv(string address,
                                                Encoding encoding = null,
                                                char delimiter = ',',
                                                char quotation = '"') {
      if (address is null)
        throw new ArgumentNullException(nameof(address));

      return ReadLines(address, encoding)
        .FromCsv(delimiter, quotation);
    }

    /// <summary>
    /// Read Text
    /// </summary>
    public static String ReadText(string address, Encoding encoding = null) {
      using var reader = encoding is null
        ? new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read Json
    /// </summary>
    public static JsonDocument ReadJson(string address, Encoding encoding = null) =>
      JsonDocument.Parse(ReadText(address, encoding));

    /// <summary>
    /// Read XML
    /// </summary>
    public static XDocument ReadXml(string address, Encoding encoding = null) =>
      XDocument.Parse(ReadText(address, encoding));

    #endregion Sync

    #region Async

    /// <summary>
    /// Read as Text (async)
    /// </summary>
    public static async Task<string> ReadTextAsync(string address, CancellationToken token) {
      if (address is null)
        throw new ArgumentNullException(nameof(address));

      var respond = await Client.GetAsync(address, token).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      return await respond.Content.ReadAsStringAsync(token).ConfigureAwait(false);
    }

    /// <summary>
    /// Read as Text (async)
    /// </summary>
    public static async Task<string> ReadTextAsync(string address) =>
      await ReadTextAsync(address, CancellationToken.None);

    /// <summary>
    /// Read as XML (async)
    /// </summary>
    public static async Task<XDocument> XmlAsync(string address, CancellationToken token) {
      if (address is null)
        throw new ArgumentNullException(nameof(address));

      var respond = await Client.GetAsync(address, token).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync(token).ConfigureAwait(false);

      return XDocument.Parse(text);
    }

    /// <summary>
    /// Read as XML (async)
    /// </summary>
    public static async Task<XDocument> XmlAsync(string address) =>
      await XmlAsync(address, CancellationToken.None);

    /// <summary>
    /// Read Json Async 
    /// </summary>
    public static async Task<JsonDocument> JsonAsync(string address, CancellationToken token) {
      if (address is null)
        throw new ArgumentNullException(nameof(address));

      var respond = await Client.GetAsync(address, token).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      using Stream stream = await Client.GetStreamAsync(address, token).ConfigureAwait(false);

      JsonDocumentOptions options = new() {
        CommentHandling = JsonCommentHandling.Skip,
        AllowTrailingCommas = true
      };

      return await JsonDocument.ParseAsync(stream, options, token);
    }

    /// <summary>
    /// Read Json Async
    /// </summary>
    public static async Task<JsonDocument> JsonAsync(string address) =>
      await JsonAsync(address, CancellationToken.None);

    /// <summary>
    /// Read Lines Async
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(
      string address, Encoding encoding, [EnumeratorCancellation] CancellationToken token) {

      if (address is null)
        throw new ArgumentNullException(nameof(address));

      encoding ??= Encoding.Default;

      using Stream stream = await Client.GetStreamAsync(address, token).ConfigureAwait(false);

      using StreamReader reader = new(stream, encoding, true, -1, true);

      for (string line = reader.ReadLine(); line is not null; line = reader.ReadLine()) {
        token.ThrowIfCancellationRequested();

        yield return line;
      }
    }

    /// <summary>
    /// Read Lines Async
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(string address) {
      await foreach (string line in ReadLinesAsync(address, null, CancellationToken.None))
        yield return line;
    }

    /// <summary>
    /// Read Lines Async
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(
      string address, [EnumeratorCancellation] CancellationToken token) {

      await foreach (string line in ReadLinesAsync(address, null, token))
        yield return line;
    }

    /// <summary>
    /// Read Lines Async
    /// </summary>
    public static async IAsyncEnumerable<string> ReadLinesAsync(string address, Encoding encoding) {
      await foreach (string line in ReadLinesAsync(address, encoding, CancellationToken.None))
        yield return line;
    }

    /// <summary>
    /// Read Csv
    /// </summary>
    public static async IAsyncEnumerable<string[]> ReadCsvAsync(string address,
                                                                Encoding encoding = null,
                                                                char delimiter = ',',
                                                                char quotation = '"',
                                       [EnumeratorCancellation] CancellationToken token = default) {
      if (address is null)
        throw new ArgumentNullException(nameof(address));

      encoding ??= Encoding.Default;

      using Stream stream = await Client.GetStreamAsync(address, token).ConfigureAwait(false);

      using StreamReader reader = new(stream, encoding, true, -1, true);

      IEnumerable<string> Lines() {
        for (string line = reader.ReadLine(); line is not null; line = reader.ReadLine()) {
          token.ThrowIfCancellationRequested();

          yield return line;
        }
      }

      foreach (string[] record in CommaSeparatedValues.ParseCsv(Lines(), delimiter, quotation)) {
        token.ThrowIfCancellationRequested();

        yield return record;
      }
    }

    #endregion Async

    #endregion Public
  }

}
