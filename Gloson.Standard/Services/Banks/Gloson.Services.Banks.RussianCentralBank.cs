using Gloson.Globalization;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using System.Xml.Linq;

namespace Gloson.Services.Banks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Russian Central Bank
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class RussianCentralBank : IExchangeOffice {
    #region IExchangeOffice

    /// <summary>
    /// Exchange Rates Async
    /// </summary>
    /// <param name="at">Date to get exchange rates</param>
    /// <param name="token">Cancellation, if any</param>
    /// <returns>Exchange rates</returns>
    public async Task<IDictionary<CurrencyInfo, decimal>> ExchangeRatesAsync(DateTime at, CancellationToken token) {
      string address = $"http://www.cbr.ru/scripts/XML_daily.asp?date_req={at.Day:00}/{at.Month:00}/{at.Year}";

      HttpClient httpClient = Dependencies.GetServiceRequired<HttpClient>();

      using var response = await httpClient.GetAsync(address, token).ConfigureAwait(false);
      CultureInfo ru = CultureInfo.GetCultureInfo("ru-Ru");

      string data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

      XDocument doc = XDocument.Parse(data);

      return doc
        .Root
        .Elements()
        .Select(node => new {
          name = node.Element("CharCode").Value,
          rate = decimal.Parse(node.Element("Value").Value, ru),
          nominal = int.Parse(node.Element("Nominal").Value, ru)
        })
        .ToDictionary(item => CurrencyInfo.Parse(item.name), item => item.rate / item.nominal);
    }

    #endregion IExchangeOffice
  }
}
