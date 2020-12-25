using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Algorithms.Graphs {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Shortest Paths (Bellman-Ford)
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Bellman%E2%80%93Ford_algorithm"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ShortestPaths {
    #region Algorithm

    /// <summary>
    /// All Shortest Paths between all nodes
    /// </summary>
    public static (double[][] distances, N[] nodes) AllShortestPaths<T, N>(
      this IEnumerable<T> source,
           Func<T, (N from, N to, double length)> edgeMap,
           IEqualityComparer<N> comparer) {
      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (edgeMap is null)
        throw new ArgumentNullException(nameof(edgeMap));

      if (comparer is null)
        comparer = EqualityComparer<N>.Default;

      List<(N from, N to, double length)> list = new List<(N from, N to, double length)>();
      Dictionary<N, int> dict = new Dictionary<N, int>(comparer);

      foreach (T record in source) {
        var edge = edgeMap(record);

        list.Add(edge);

        if (!dict.ContainsKey(edge.from))
          dict.Add(edge.from, dict.Count);

        if (!dict.ContainsKey(edge.to))
          dict.Add(edge.to, dict.Count);
      }

      int n = dict.Count;

      double[][] W = Enumerable
        .Range(0, n)
        .Select(_i => Enumerable
           .Repeat(double.PositiveInfinity, n)
           .ToArray())
        .ToArray();

      for (int i = 0; i < n; ++i)
        W[i][i] = 0;

      // --- Solution ---

      foreach (var (from, to, length) in list)
        W[dict[from]][dict[to]] = length;

      for (int k = 0; k < n; ++k)
        for (int i = 0; i < n; ++i)
          for (int j = 0; j < n; ++j)
            W[i][j] = Math.Min(W[i][j], W[i][k] + W[k][j]);

      bool negativeLoop = Enumerable
        .Range(0, n)
        .Any(i => W[i][i] < 0);

      if (negativeLoop)
        return (Array.Empty<double[]>(), Array.Empty<N>());

      return (W, dict.OrderBy(i => i).Select(p => p.Key).ToArray());
    }

    /// <summary>
    /// All Shortest Paths between all nodes
    /// </summary>
    public static (double[][] distances, N[] nodes) AllShortestPaths<T, N>(
      this IEnumerable<T> source,
           Func<T, (N from, N to, double length)> edgeMap) => AllShortestPaths(source, edgeMap, null);

    #endregion Algorithm
  }

}
