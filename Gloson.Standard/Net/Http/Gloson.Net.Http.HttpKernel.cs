using Gloson.Text;
using System;
using System.Collections.Generic;
using System.IO;
using System.Json;
using System.Net.Http;
using System.Text;
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
      if (null == address)
        throw new ArgumentNullException(nameof(address));

      using var reader = null == encoding
        ? new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      for (string line = reader.ReadLine(); line != null; line = reader.ReadLine())
        yield return line;
    }

    /// <summary>
    /// Read Csv
    /// </summary>
    public static IEnumerable<string[]> ReadCsv(string address,
                                                Encoding encoding = null,
                                                char delimiter = ',',
                                                char quotation = '"') {
      if (null == address)
        throw new ArgumentNullException(nameof(address));

      return ReadLines(address, encoding)
        .FromCsv(delimiter, quotation);
    }

    /// <summary>
    /// Read Text
    /// </summary>
    public static String ReadText(string address, Encoding encoding = null) {
      using var reader = null == encoding
        ? new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      return reader.ReadToEnd();
    }

    /// <summary>
    /// Read Json
    /// </summary>
    public static JsonValue ReadJson(string address, Encoding encoding = null) {
      using var reader = null == encoding
        ? new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      var text = reader.ReadToEnd();

      return JsonValue.Parse(text);
    }

    /// <summary>
    /// Read XML
    /// </summary>
    public static XDocument ReadXml(string address, Encoding encoding = null) {
      using var reader = null == encoding
        ? new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(Client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      var text = reader.ReadToEnd();

      return XDocument.Parse(text);
    }

    #endregion Sync

    #region Async

    /// <summary>
    /// Read as Text (async)
    /// </summary>
    public static async Task<string> ReadTextAsync(string address) {
      if (null == address)
        throw new ArgumentNullException(nameof(address));

      var respond = await Client.GetAsync(address).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      return await respond.Content.ReadAsStringAsync().ConfigureAwait(false);
    }

    /// <summary>
    /// Read as Json (async)
    /// </summary>
    public static async Task<JsonValue> JsonAsync(string address) {
      if (null == address)
        throw new ArgumentNullException(nameof(address));

      var respond = await Client.GetAsync(address).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync().ConfigureAwait(false);

      return JsonValue.Parse(text);
    }

    /// <summary>
    /// Read as XML (async)
    /// </summary>
    public static async Task<XDocument> XmlAsync(string address) {
      if (null == address)
        throw new ArgumentNullException(nameof(address));

      var respond = await Client.GetAsync(address).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync().ConfigureAwait(false);

      return XDocument.Parse(text);
    }

    #endregion Async

    #endregion Public
  }

}
