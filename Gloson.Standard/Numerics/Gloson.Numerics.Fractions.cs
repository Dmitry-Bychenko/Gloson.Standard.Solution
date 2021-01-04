using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Fractions math
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Fractions {
    #region Public

    /// <summary>
    /// Farey sequence
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Farey_sequence"/>
    public static IEnumerable<(int numerator, int denominator)> Farey(int n) {
      if (n <= 0)
        throw new ArgumentOutOfRangeException(nameof(n), $"{nameof(n)} must be positive");

      int a = 0;
      int b = 1;
      int c = 1;
      int d = n;

      yield return (a, b);
      yield return (c, d);

      if (n == 1)
        yield break;

      while (true) {
        int z = (n + b) / d;

        int p = z * c - a;
        int q = z * d - b;

        yield return (p, q);

        if (p == 1 && q == 1)
          break;

        a = c;
        b = d;
        c = p;
        d = q;
      }
    }

    /// <summary>
    /// Dijkstra Fusc function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Calkin%E2%80%93Wilf_tree"/>
    public static int Fusc(int value) {
      int result = 0;

      for (int n = value, a = 1; n != 0; n /= 2)
        if (n % 2 == 0)
          a += result;
        else
          result += a;

      return result;
    }

    #endregion Public 
  }

}
