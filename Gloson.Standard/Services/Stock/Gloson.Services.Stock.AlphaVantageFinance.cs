using Gloson.Text;
using System;
using System.Globalization;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Gloson.Services.Stock {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Alpha Vantage Finance
  /// </summary>
  /// <see cref="https://www.alphavantage.co/documentation/"/>
  // 9W0A3V9JHMAY5XWW
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class AlphaVantageFinance {
    #region Private Data

    private static string s_DefaultApiKey = "";

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public AlphaVantageFinance(string apiKey) {
      ApiKey = (apiKey ?? DefaultApiKey).Trim();
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    public AlphaVantageFinance() : this(null) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Api Key
    /// </summary>
    public string ApiKey { get; }

    /// <summary>
    /// Api Key
    /// </summary>
    public static string DefaultApiKey {
      get {
        string value = s_DefaultApiKey;

        return string.IsNullOrWhiteSpace(value) ? "demo" : value;
      }
      set {
        if (value is null)
          value = "";

        Interlocked.Exchange(ref s_DefaultApiKey, value);
      }
    }

    /// <summary>
    /// Daily Series
    /// </summary>
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="interval">Interval (in minutes)</param>
    public async Task<Ticket[]> DailyAsync(string symbol, int interval = 1) {
      if (symbol is null)
        throw new ArgumentNullException(nameof(symbol));

      string address = string.Join("&",
        $@"https://www.alphavantage.co/query?apikey={ApiKey}",
         $"symbol={symbol.Trim().ToUpperInvariant()}",
         $"function=TIME_SERIES_INTRADAY",
         $"outputsize=full",
         $"datatype=csv",
         $"interval={interval}min");

      HttpClient client = Dependencies.GetServiceRequired<HttpClient>();

      using var response = await client.GetAsync(address).ConfigureAwait(false);

      string data = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

      static DateTime ObtainDate(string value) {
        DateTime date = DateTime.ParseExact(
          value,
          "yyyy'-'MM'-'dd' 'HH':'mm':'ss",
          CultureInfo.InvariantCulture,
          DateTimeStyles.None);

        TimeZoneInfo tz = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");

        return TimeZoneInfo.ConvertTimeToUtc(date, tz);
      }

      return data
        .SplitToLines()
        .FromCsv()
        .Skip(1)
        .Select(items => new Ticket(
           symbol,
           ObtainDate(items[0]),
           Decimal.Parse(items[1], CultureInfo.InvariantCulture),
           Decimal.Parse(items[2], CultureInfo.InvariantCulture),
           Decimal.Parse(items[3], CultureInfo.InvariantCulture),
           Decimal.Parse(items[4], CultureInfo.InvariantCulture),
           Decimal.Parse(items[5], CultureInfo.InvariantCulture))
         )
        .ToArray();
    }

    /// <summary>
    /// Daily
    /// </summary>
    /// <param name="symbol">Symbol</param>
    /// <param name="interval">Interval (in minutes)</param>
    /// <returns></returns>
    public Ticket[] Daily(string symbol, int interval = 1) =>
      DailyAsync(symbol, interval).GetAwaiter().GetResult();

    #endregion Public
  }
}
