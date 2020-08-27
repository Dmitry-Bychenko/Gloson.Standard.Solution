using Gloson.Collections.Generic;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Geometry.Clustering {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Clustering
  /// </summary>
  // 
  //-------------------------------------------------------------------------------------------------------------------

  public interface IClustering<T> {
    IEnumerable<(T item, int cluster)> Clusterize(IEnumerable<T> source);
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Base clustering
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public abstract class BaseClustering<T> : IClustering<T> {
    #region Algorithm

    protected static List<double[]> Sample(int count, IEnumerable<double[]> data, int dims, int? seed) {
      if (count <= 0)
        throw new ArgumentOutOfRangeException(nameof(count));

      var result = Enumerable
        .Range(0, count)
        .Select(ii => new double[dims])
        .ToList();

      List<(double min, double max)> ranges = Enumerable
        .Range(0, dims)
        .Select(ii => (double.PositiveInfinity, double.NegativeInfinity))
        .ToList();

      foreach (double[] record in data) {
        for (int i = 0; i < dims; ++i) {
          double v = record[i];

          if (double.IsNaN(v))
            continue;
          else if (double.IsInfinity(v))
            continue;

          if (ranges[i].min > v)
            ranges[i] = (v, ranges[i].max);

          if (ranges[i].max < v)
            ranges[i] = (ranges[i].min, v);
        }
      }

      Random random = seed.HasValue ? new Random(seed.Value) : null;

      for (int i = 0; i < dims; ++i) {
        double min = 0;
        double max = 1;

        if (!double.IsInfinity(ranges[i].min)) {
          min = ranges[i].min;
          max = ranges[i].max;
        }

        for (int j = 0; j < count; ++j) {
          double r = random?.NextDouble() ?? RandomThreadSafe.Default.NextDouble();

          r = min + (max - min) * r;

          result[j][i] = r;
        }
      }

      return result;
    }

    #endregion Algorithm

    #region IClustering<T>

    /// <summary>
    /// Clusterize
    /// </summary>
    public abstract IEnumerable<(T item, int cluster)> Clusterize(IEnumerable<T> source);

    #endregion IClustering<T>
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Clustering Extensions
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------
  public static class EnumerableExtensions {
    #region Algorithm

    private static IEnumerable<IGrouping<int, T>> ToGroupBy<T>(IClustering<T> engine, IEnumerable<T> source) {
      if (null == engine)
        throw new ArgumentException(nameof(engine));
      else if (null == source)
        throw new ArgumentNullException(nameof(source));

      Dictionary<int, List<T>> result = new Dictionary<int, List<T>>();

      foreach (var (item, cluster) in engine.Clusterize(source)) {
        if (result.TryGetValue(cluster, out var list))
          list.Add(item);
        else
          result.Add(cluster, new List<T>() { item });
      }

      int index = 0;

      foreach (var pair in result.OrderBy(p => p.Key))
        yield return new Grouping<int, T>(index++, pair.Value);
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Group By Clasters
    /// </summary>
    /// <param name="source">Items to clusterize</param>
    /// <param name="algorithm">Clustering Algorithm</param>
    /// <example>
    /// <code>
    /// var source = new double[][] {
    ///   new double[] {1, 2},
    ///   new double[] {3, 4},
    ///   ...
    /// };
    /// 
    /// var result = source.GroupByClusters(new KMeansClustering(p => p, 3));
    /// </code>
    /// </example>
    public static IEnumerable<IGrouping<int, T>> GroupByClusters<T>(
      this IEnumerable<T> source, IClustering<T> algorithm) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == algorithm)
        throw new ArgumentNullException(nameof(algorithm));

      return ToGroupBy(algorithm, source);
    }

    #endregion Public
  }

}
