using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Geometry.Norms.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Canberra Distance
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Canberra_distance"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CanberraDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double result = 0.0;

      foreach (var (x, y) in points)
        result += Math.Abs(x - y) / (Math.Abs(x) + Math.Abs(y));

      return result;
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Cosine Distance (based on Cosine similarity)
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Cosine_similarity"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CosineDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double ab = 0.0;
      double a = 0.0;
      double b = 0.0;

      foreach (var (x, y) in points) {
        ab += x * y;
        a += x * x;
        b += y * y;
      }

      return 1 - ab / Math.Sqrt(a) / Math.Sqrt(b);
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Bray-Curtis Distance (based on Bray-Curtis dissimilarity)
  /// </summary>
  /// <see cref="https://en.wikipedia.org/wiki/Bray%E2%80%93Curtis_dissimilarity"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BrayCurtisDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double a = 0.0;
      double b = 0.0;

      foreach (var (x, y) in points) {
        a += Math.Abs(x - y);
        b += Math.Abs(x + y);
      }

      return a / b;
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Correlation Distance 
  /// </summary>
  /// <see cref="https://reference.wolfram.com/language/ref/CorrelationDistance.html"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class CorrelationDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      var pts = points.ToList();

      if (pts.Count <= 0)
        return 0.0;

      double Mx = pts.Average(p => p.x);
      double My = pts.Average(p => p.y);

      double ab = 0.0;
      double a = 0.0;
      double b = 0.0;

      foreach (var (x, y) in pts) {
        ab += (x - Mx) * (y - My);
        a += (x - Mx) * (x - Mx);
        b += (y - My) * (y - My);
      }

      return 1 - ab / Math.Sqrt(a) / Math.Sqrt(b);
    }
  }

}
