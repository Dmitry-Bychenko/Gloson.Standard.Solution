using System.Collections.Generic;
using System.Numerics;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Number Sequences
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class Sequences {
    #region Public

    /// <summary>
    /// Fibonacci Sequence
    /// </summary>
    public static IEnumerable<BigInteger> Fibonacci() {
      yield return 0;

      BigInteger prior = 0;
      BigInteger current = 1;

      while (true) {
        yield return current;

        BigInteger next = prior + current;
        prior = current;
        current = next;
      }
    }

    /// <summary>
    /// Catalan numbers
    /// </summary>
    public static IEnumerable<BigInteger> Catalan() {
      BigInteger C = 1;

      for (int n = 1; ; ++n) {
        yield return C;

        C = 2 * (2 * n - 1) * C / (n + 1);
      }
    }

    #endregion Public
  }
}
