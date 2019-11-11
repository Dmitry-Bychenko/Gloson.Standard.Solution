using System;
using System.Collections.Generic;
using System.Json;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
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
      if (null == client)
        throw new ArgumentNullException(nameof(client));

      var respond = await client.GetAsync(address);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync();

      return JsonValue.Parse(text);
    }

    /// <summary>
    /// Read as XML
    /// </summary>
    public static async Task<XDocument> XmlAsync(this HttpClient client, string address) {
      if (null == client)
        throw new ArgumentNullException(nameof(client));

      var respond = await client.GetAsync(address);

      respond.EnsureSuccessStatusCode();

      var text = await respond.Content.ReadAsStringAsync();

      return XDocument.Parse(text);
    }

    #endregion Public
  }
}
