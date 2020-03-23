﻿using System;
using System.Collections.Generic;
using System.Text;

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

    #endregion Public
  }
}
