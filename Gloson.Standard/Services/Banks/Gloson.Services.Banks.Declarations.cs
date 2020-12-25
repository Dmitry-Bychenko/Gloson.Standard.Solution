using Gloson.Globalization;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Services.Banks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Exchange Office
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public interface IExchangeOffice {
    /// <summary>
    /// Exchange Rates at 
    /// </summary>
    /// <param name="at">DateTime of Exchange Rates</param>
    /// <returns>Exchange Rates</returns>
    Task<IDictionary<CurrencyInfo, decimal>> ExchangeRatesAsync(DateTime at, CancellationToken token);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Exchange Office Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ExchangeOfficeExtensions {
    #region Public

    /// <summary>
    /// Exchange Rates at 
    /// </summary>
    /// <param name="at">DateTime of Exchange Rates</param>
    /// <returns>Exchange Rates</returns>
    public static IDictionary<CurrencyInfo, decimal> ExchangeRates(this IExchangeOffice office, DateTime at) {
      if (office is null)
        throw new ArgumentNullException(nameof(office));

      return office.ExchangeRatesAsync(at, CancellationToken.None).GetAwaiter().GetResult();
    }

    /// <summary>
    /// Exchange Rates at 
    /// </summary>
    /// <param name="at">DateTime of Exchange Rates</param>
    /// <returns>Exchange Rates</returns>
    public static Task<IDictionary<CurrencyInfo, decimal>> ExchangeRatesAsync(this IExchangeOffice office, DateTime at) {
      if (office is null)
        throw new ArgumentNullException(nameof(office));

      return office.ExchangeRatesAsync(at, CancellationToken.None);
    }

    #endregion Public
  }

}
