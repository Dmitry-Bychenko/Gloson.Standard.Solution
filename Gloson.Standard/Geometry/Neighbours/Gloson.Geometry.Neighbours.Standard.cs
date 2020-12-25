using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Geometry.Neighbours {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Neighbours
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Neighbours {
    #region Public

    /// <summary>
    /// Manhattan (L1) metric
    /// {0, 0, 0, 0, 1}, {0, 0, 0, 0, -1}, {0, 0, 0, 1, 0}, {0, 0, 0, -1, 0}, {0, 0, 1, 0, 0}, ..., {-1, 0, 0, 0, 0}
    /// </summary>
    public static IEnumerable<int[]> Manhatten(int dimensions) {
      if (dimensions < 0)
        throw new ArgumentOutOfRangeException(nameof(dimensions));
      else if (dimensions == 0)
        yield break;

      int[] result;

      for (int i = 0; i < dimensions; ++i) {
        result = new int[dimensions];
        result[result.Length - 1 - i] = 1;

        yield return result;

        result = new int[dimensions];
        result[result.Length - 1 - i] = -1;

        yield return result;
      }
    }

    /// <summary>
    /// Manhattan (L1) metric
    /// </summary>
    /// <param name="from">From point</param>
    public static IEnumerable<int[]> Manhatten(IEnumerable<int> from) {
      if (from is null)
        throw new ArgumentNullException(nameof(from));

      int[] data = from.ToArray();

      foreach (var delta in Manhatten(data.Length)) {
        int[] result = new int[data.Length];

        for (int i = 0; i < data.Length; ++i)
          result[i] = data[i] + delta[i];

        yield return result;
      }
    }

    /// <summary>
    /// Manhattan (L2) metric
    /// {1, 0, 0}, {-1, 0, 0}, {0, 1, 0}, {1, 1, 0}, {-1, 1, 0}, {-1, -1, 0}, ... {-1, -1, -1}
    /// </summary>
    public static IEnumerable<int[]> Euclid(int dimensions) {
      if (dimensions < 0)
        throw new ArgumentOutOfRangeException(nameof(dimensions));
      else if (dimensions == 0)
        yield break;

      int[] result = new int[dimensions];

      do {
        if (!result.All(item => item == 0))
          yield return result.ToArray();

        for (int i = 0; i < dimensions; ++i) {
          if (result[i] == -1)
            result[i] = 0;
          else {
            if (result[i] == 0)
              result[i] = 1;
            else
              result[i] = -1;

            break;
          }
        }
      }
      while (!result.All(item => item == 0));
    }

    /// <summary>
    /// Manhattan (L1) metric
    /// </summary>
    /// <param name="from">From point</param>
    public static IEnumerable<int[]> Euclid(IEnumerable<int> from) {
      if (from is null)
        throw new ArgumentNullException(nameof(from));

      int[] data = from.ToArray();

      foreach (var delta in Euclid(data.Length)) {
        int[] result = new int[data.Length];

        for (int i = 0; i < data.Length; ++i)
          result[i] = data[i] + delta[i];

        yield return result;
      }
    }

    #endregion Public
  }

}
