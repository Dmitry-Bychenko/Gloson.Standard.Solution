using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Geometry.Norms.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Euclidian Distance (L2 Norm)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class EuclidianDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double result = 0.0;

      foreach (var (x, y) in points) 
        result += (x - y) * (x - y);

      return Math.Sqrt(result);
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Squared Euclidian Distance (L2 Norm squared)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SquaredEuclidianDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double result = 0.0;

      foreach (var (x, y) in points)
        result += (x - y) * (x - y);

      return result;
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Manhattan Distance (L1 Norm)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ManhattanDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double result = 0.0;

      foreach (var (x, y) in points)
        result += Math.Abs(x - y);

      return result;
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Chessboard Distance (L∞ Norm)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ChessboardDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double result = 0.0;

      foreach (var (x, y) in points)
        result = Math.Max(result, Math.Abs(x - y));

      return result;
    }
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// L Norm distance 0 .. ∞ Norms
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LNormDistance : DistanceAbstract {
    /// <summary>
    /// Distance Computation
    /// </summary>
    protected override double CoreDistance(IEnumerable<(double x, double y)> points) {
      double result = 0.0;

      if (double.IsPositiveInfinity(Norm)) {
        foreach (var (x, y) in points)
          result = Math.Max(result, Math.Abs(x - y));
      }
      else if (Norm == 0) {
        foreach (var (x, y) in points)
          result = x == y ? 0 : 1;
      }
      else {
        foreach (var (x, y) in points)
          result += Math.Pow(Math.Abs(x - y), Norm);

        result = Math.Pow(result, 1.0 / Norm);
      }

      return result;
    }

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="norm">Norm in (-∞..+∞] range</param>
    public LNormDistance(double norm) {
      if (double.IsNaN(norm))
        throw new ArgumentException("norm must not be NaN", nameof(norm));
      else if (double.IsNegativeInfinity(norm))
        throw new ArgumentOutOfRangeException(nameof(norm));

      Norm = norm;
    }

    /// <summary>
    /// Norm
    /// </summary>
    public double Norm { get; }
  }

}
