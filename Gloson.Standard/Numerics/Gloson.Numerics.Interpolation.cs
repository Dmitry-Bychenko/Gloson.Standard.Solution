using Gloson.Numerics.Matrices;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Numerics {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Interpolation and Extrapolation
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------
  public static class Interpolation {
    #region Public

    /// <summary>
    /// Lagrange Interpolation
    /// </summary>
    public static double InterpolationLagrange<T>(this IEnumerable<T> source, Func<T, (double x, double y)> map, double at) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == map)
        throw new ArgumentNullException(nameof(map));

      List<(double x, double y)> data = source
        .Select(value => map(value))
        .ToList();

      if (data.Count <= 0)
        throw new ArgumentException("source must not be empty.", nameof(source));

      double result = 0;

      for (int i = 0; i < data.Count; ++i) {
        double term = data[i].y;

        for (int j = 0; j < data.Count; ++j)
          if (j != i)
            term *= (at - data[j].x) / (data[i].x - data[j].x);

        result += term;
      }

      return result;
    }

    /// <summary>
    /// Interpolation polynom
    /// </summary>
    public static Polynom InterpolatonPolynom<T>(this IEnumerable<T> source, Func<T, (double x, double y)> map) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == map)
        throw new ArgumentNullException(nameof(map));

      List<(double x, double y)> list = source
        .Select(value => map(value))
        .ToList();

      if (list.Count <= 0)
        throw new ArgumentException("source must not be empty.", nameof(source));

      double[][] data = Enumerable
        .Range(0, list.Count)
        .Select(index => new double[list.Count + 1])
        .ToArray();

      for (int r = 0; r < data.Length; ++r) {
        data[r][list.Count] = list[r].y;

        double x = list[r].x;
        double p = 1.0;

        for (int c = 0; c < list.Count; ++c) {
          data[r][c] = p;
          p *= x;
        }
      }

      return new Polynom(MatrixLowLevel.Solve(data));
    }

    /// <summary>
    /// Linear Interpolation
    /// </summary>
    public static double InterpolationLinear<T>(this IEnumerable<T> source, Func<T, (double x, double y)> map, double at) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == map)
        throw new ArgumentNullException(nameof(map));

      (double x, double y) first = (double.NaN, double.NaN);
      (double x, double y) second = (double.NaN, double.NaN);

      foreach (var (x, y) in source.Select(point => map(point))) {
        if (double.IsNaN(first.x) || Math.Abs(first.x - at) > Math.Abs(x - at)) {
          second = first;
          first = (x, y);
        }
        else if (double.IsNaN(second.x) || Math.Abs(second.x - at) > Math.Abs(x - at))
          if (x != first.x)
            second = (x, y);
      }

      if (double.IsNaN(first.x))
        throw new ArgumentException("source must not be empty", nameof(source));
      else if (double.IsNaN(second.x))
        throw new ArgumentException("source must have at least 2 different points", nameof(source));

      double k = (second.y - first.y) / (second.x - first.x);
      double b = (first.y * second.x - second.y * first.x) / (second.x - first.x);

      return k * at + b;
    }

    #endregion Public
  }

}
