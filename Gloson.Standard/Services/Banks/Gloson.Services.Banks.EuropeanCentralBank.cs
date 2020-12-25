using Gloson.Globalization;
using Gloson.Json;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Json;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Services.Banks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// European Central Bank
  /// </summary>
  // https://github.com/exchangeratesapi/exchangeratesapi 
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EuropeanCentralBank : IExchangeOffice {
    #region Algortithm

    private static async Task<IDictionary<CurrencyInfo, decimal>> CoreExchangeRatesAsync(string address, CancellationToken token) {
      HttpClient httpClient = Dependencies.GetServiceRequired<HttpClient>();

      using var response = await httpClient.GetAsync(address, token).ConfigureAwait(false);
      CultureInfo ru = CultureInfo.GetCultureInfo("ru-Ru");

      string data = await response.Content.ReadAsStringAsync(CancellationToken.None).ConfigureAwait(false);

      JsonValue json = JsonValue.Parse(data);

      if (json is JsonObject obj) {
        var raw = (obj.Value("rates") as JsonObject);

        var top = raw.Values.FirstOrDefault();

        JsonObject array = null;

        if (top is null)
          return new Dictionary<CurrencyInfo, decimal>();
        else if (top is JsonObject)
          array = top as JsonObject;
        else
          array = raw;

        return array.ToDictionary(
          item => CurrencyInfo.Parse(item.Key),
          item => (decimal)(item.Value));
      }
      else
        return new Dictionary<CurrencyInfo, decimal>();
    }

    #endregion Algortithm

    #region IExchangeOffice

    /// <summary>
    /// Exchange Rates for given date
    /// </summary>
    public async Task<IDictionary<CurrencyInfo, decimal>> ExchangeRatesAsync(DateTime at, CancellationToken token) {
      string dateAt = at.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);

      return await CoreExchangeRatesAsync(
        $"https://api.exchangeratesapi.io/history?start_at={dateAt}&end_at={dateAt}",
          token).ConfigureAwait(false);
    }

    /// <summary>
    /// Exchange Rates for the latest date
    /// </summary>
    public static async Task<IDictionary<CurrencyInfo, decimal>> ExchangeRatesLatestAsync(CancellationToken token) {
      return await CoreExchangeRatesAsync(
        $"https://api.exchangeratesapi.io/latest",
          token).ConfigureAwait(false);
    }

    #endregion IExchangeOffice
  }
}
