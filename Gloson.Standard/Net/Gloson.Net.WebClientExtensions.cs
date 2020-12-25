using System;
using System.Json;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Gloson.Net {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Web Client extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class WebClientExtensions {
    #region Public

    /// <summary>
    /// With Default credentials
    /// </summary>
    public static WebClient WithDefaultCredentials(this WebClient client) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));

      client.Credentials = CredentialCache.DefaultCredentials;

      return client;
    }

    /// <summary>
    /// Read String with encoding
    /// </summary>
    public static string ReadString(this WebClient client, string address, Encoding encoding) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));

      byte[] data = client.DownloadData(address);

      return encoding.GetString(data);
    }

    /// <summary>
    /// Read Json
    /// </summary>
    public static JsonValue ReadJson(this WebClient client, string address, Encoding encoding) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));

      return JsonValue.Parse(ReadString(client, address, encoding));
    }

    /// <summary>
    /// Read XML
    /// </summary>
    public static XDocument ReadXml(this WebClient client, string address, Encoding encoding) {
      if (client is null)
        throw new ArgumentNullException(nameof(client));

      return XDocument.Parse(ReadString(client, address, encoding));
    }

    #endregion Public
  }

}
