using Gloson.Geometry.Norms;
using Gloson.Geometry.Norms.Library;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Geometry.Clustering.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// K-Means
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class KMeansClustering<T> : BaseClustering<T> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="point">Point map function</param>
    /// <param name="centroinds">Centroids to create</param>
    /// <param name="geometry">Geometry to use</param>
    /// <param name="seed">Seed</param>
    public KMeansClustering(Func<T, IEnumerable<double>> point,
                            int centroinds,
                            IDistance geometry = null,
                            int? seed = null) {

      Point = point ?? throw new ArgumentNullException(nameof(point));

      Centroids = centroinds > 0
        ? centroinds
        : throw new ArgumentOutOfRangeException(nameof(centroinds));

      Geometry = geometry ?? new EuclidianDistance();
      Seed = seed;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Clusterize
    /// </summary>
    /// <param name="source">Items to clusterize</param>
    /// <returns></returns>
    public override IEnumerable<(T item, int cluster)> Clusterize(IEnumerable<T> source) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      List<(T item, double[] point, int cluster)> data = source
        .Select(item => (item, Point(item)?.ToArray() ?? new double[0], 0))
        .ToList();

      if (data.Count <= 0)
        yield break;

      int dim = data[0].point.Length;

      for (int i = 0; i < data.Count; ++i) {
        if (data[i].point.Length != dim)
          throw new ArgumentException($"Inconsistent dimension: point #{i}", nameof(source));

        if (data[i].point.Any(d => double.IsNaN(d)))
          throw new ArgumentException($"NaN values: point #{i}", nameof(source));
      }

      // Centroids Sampling 

      List<double[]> centroids = Sample(Centroids, data.Select(r => r.point), dim, Seed);

      // Centroids processing
      bool hasAgenda = true;

      while (hasAgenda) {
        hasAgenda = false;

        // Assign to centroids
        for (int i = 0; i < data.Count; ++i) {
          var (item, point, cluster) = data[i];

          int best = -1;
          double bestDistance = double.PositiveInfinity;

          for (int j = 0; j < centroids.Count; ++j) {
            double d = Geometry.Distance(centroids[j], point);

            if (best < 0 || d < bestDistance) {
              best = j;
              bestDistance = d;
            }
          }

          if (best != cluster) {
            hasAgenda = true;
            data[i] = (item, point, best);
          }
        }

        // Move centroids
        if (!hasAgenda)
          break;

        List<(int count, double[] point)> sums = Enumerable
          .Range(0, Centroids)
          .Select(ii => (0, new double[dim]))
          .ToList();

        foreach (var (item, point, cluster) in data) {
          double[] before = sums[cluster].point;
          double[] after = point;
          double[] next = new double[point.Length];

          for (int i = 0; i < after.Length; ++i)
            next[i] = after[i] + before[i];

          sums[cluster] = (sums[cluster].count + 1, next);
        }

        for (int i = 0; i < sums.Count; ++i)
          if (sums[i].count > 0)
            for (int j = 0; j < dim; ++j)
              centroids[i][j] = sums[i].point[j] / sums[i].count;
      }

      // Final output
      foreach (var item in data)
        yield return ((item.item, item.cluster));
    }

    /// <summary>
    /// Seed to Random
    /// </summary>
    public int? Seed { get; }

    /// <summary>
    /// Obtain point from T item
    /// </summary>
    public Func<T, IEnumerable<double>> Point { get; }

    /// <summary>
    /// Centroids
    /// </summary>
    public int Centroids { get; }

    /// <summary>
    /// Geometry
    /// </summary>
    public IDistance Geometry { get; }

    #endregion Public
  }

}
