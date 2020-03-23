namespace Gloson.Services.Banks {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Exchange Offices
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ExchangeOffices {
    #region Public

    /// <summary>
    /// Russian Central Bank
    /// </summary>
    public static IExchangeOffice RussianCentralBank { get; } = new RussianCentralBank();

    /// <summary>
    /// Russian Central Bank
    /// </summary>
    public static IExchangeOffice EuropeanCentralBank { get; } = new EuropeanCentralBank();

    #endregion Public
  }
}
