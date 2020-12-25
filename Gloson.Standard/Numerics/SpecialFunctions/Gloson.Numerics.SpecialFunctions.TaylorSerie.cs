using System;
using System.Collections.Generic;

namespace Gloson.Numerics.SpecialFunctions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Taylor Serie
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class TaylorSerie {
    #region Public

    /// <summary>
    /// Tailor Serie
    /// </summary>
    /// <param name="x">Argument</param>
    /// <param name="next">Next term from index, argument, prior term</param>
    /// <param name="index">index</param>
    /// <param name="start">start term</param>
    /// <returns></returns>
    public static IEnumerable<double> Serie(double x,
                                            Func<int, double, double, double> next,
                                            int index = 0,
                                            double start = 0.0) {
      if (next is null)
        throw new ArgumentNullException(nameof(next));

      double result = start;

      for (int i = index; ; ++i) {
        result = next(i, x, result);

        yield return result;
      }
    }

    #endregion Public
  }
}
