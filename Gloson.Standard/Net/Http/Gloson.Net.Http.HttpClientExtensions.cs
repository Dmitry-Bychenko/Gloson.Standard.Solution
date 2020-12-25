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
  /// Http Client Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class HttpClientExtensions {
    #region Public

    /// <summary>
    /// Read as Json
    /// </summary>
    public static async Task<JsonValue> JsonAsync(this HttpClient client, string address) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));

      var respond = await client.GetAsync(address).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync().ConfigureAwait(false);

      return JsonValue.Parse(text);
    }

    /// <summary>
    /// Read as XML
    /// </summary>
    public static async Task<XDocument> XmlAsync(this HttpClient client, string address) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));

      var respond = await client.GetAsync(address).ConfigureAwait(false);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync().ConfigureAwait(false);

      return XDocument.Parse(text);
    }

    /// <summary>
    /// Read Lines
    /// </summary>
    public static IEnumerable<string> ReadLines(this HttpClient client, string address, Encoding encoding = null) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));
      else if (address is null)
        throw new ArgumentNullException(nameof(address));

      using var reader = encoding is null
        ? new StreamReader(client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult())
        : new StreamReader(client.GetStreamAsync(address).ConfigureAwait(false).GetAwaiter().GetResult(), encoding);

      for (string line = reader.ReadLine(); line is not null; line = reader.ReadLine())
        yield return line;
    }

    #endregion Public
  }
}
