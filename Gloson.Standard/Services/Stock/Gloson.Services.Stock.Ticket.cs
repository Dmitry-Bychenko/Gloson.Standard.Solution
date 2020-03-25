using System;

namespace Gloson.Services.Stock {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Ticket
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class Ticket {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="symbol"></param>
    /// <param name="at"></param>
    public Ticket(string symbol,
                  DateTime at,
                  Decimal open,
                  Decimal high,
                  Decimal low,
                  Decimal close,
                  Decimal volume) {
      Symbol = symbol?.Trim().ToUpperInvariant() ?? throw new ArgumentNullException(nameof(symbol));
      At = at;

      Open = open;
      High = high;
      Low = low;
      Close = close;
      Volume = volume;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Symbol
    /// </summary>
    public string Symbol { get; }

    /// <summary>
    /// At
    /// </summary>
    public DateTime At { get; }

    /// <summary>
    /// Open
    /// </summary>
    public Decimal Open { get; }

    /// <summary>
    /// Close
    /// </summary>
    public Decimal Close { get; }

    /// <summary>
    /// High
    /// </summary>
    public Decimal High { get; }

    /// <summary>
    /// Low
    /// </summary>
    public Decimal Low { get; }

    /// <summary>
    /// Volume
    /// </summary>
    public Decimal Volume { get; }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"{Symbol} at {At:yyyy-MM-dd HH:mm:ss} {High:f2}..{Low:f2} ({Volume})";

    #endregion Public
  }
}
