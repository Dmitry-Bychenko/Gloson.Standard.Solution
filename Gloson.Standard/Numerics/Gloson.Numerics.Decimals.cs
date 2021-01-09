using System;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Decimal Helper
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class DecimalHelper {
    #region Public

    /// <summary>
    /// Generate 0m, 0.0m, 0.00m, ... ,0.00000000000000000m, ....
    /// </summary>
    /// <param name="zeroesAfterDecimalPoint"></param>
    /// <returns></returns>
    public static decimal Zero(int zeroesAfterDecimalPoint) {
      if (zeroesAfterDecimalPoint < 0 || zeroesAfterDecimalPoint > 29)
        throw new ArgumentOutOfRangeException(nameof(zeroesAfterDecimalPoint));

      int[] bits = new int[4];

      bits[3] = zeroesAfterDecimalPoint << 16;

      return new decimal(bits);
    }

    /// <summary>
    /// Round 
    /// </summary>
    public static decimal Round(this decimal value, int decimals, MidpointRounding mode) =>
      Math.Round(value, decimals, mode) + Zero(decimals);
    

    /// <summary>
    /// Round 
    /// </summary>
    public static decimal Round(this decimal value, int decimals) =>
      Math.Round(value, decimals) + Zero(decimals);
    
    #endregion Public
  }
}
