using System;

namespace Gloson.Geometry.Plane {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Point (double x, double y)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class PointExtensions {
    #region Public

    /// <summary>
    /// Shift
    /// </summary>
    /// <param name="source">Initial Point</param>
    /// <param name="delta">Delta (shift)</param>
    /// <returns>New point</returns>
    public static (double x, double y) Shift(this (double x, double y) source, (double x, double y) delta) =>
      (source.x + delta.x, source.y + delta.y);

    /// <summary>
    /// Scale (stretch)
    /// </summary>
    /// <param name="source">Source point</param>
    /// <param name="factor">Factor</param>
    /// <returns>New point</returns>
    public static (double x, double y) Scale(this (double x, double y) source, (double x, double y) factor) =>
      (source.x * factor.x, source.y * factor.y);

    /// <summary>
    /// Mirror (reflection), Line Symmetry
    /// </summary>
    /// <param name="source">Initial Point</param>
    /// <param name="mirror">Line a * y + b * x + c = 0 to reflect in</param>
    /// <returns>New point</returns>
    /// <see cref="http://nasweb2.dscloud.me/wordpress/2017/05/06/how-to-find-the-coordinates-of-a-reflected-point-in-a-general-line/"/>
    public static (double x, double y) Mirror(this (double x, double y) source, (double a, double b, double c) mirror) {
      if (mirror.a == 0 && mirror.b == 0)
        throw new ArgumentException($"Both a and b coefficients of {nameof(mirror)} 0.", nameof(mirror));

      var (a, b, c) = mirror;
      var (p, q) = source;

      double d = a * a + b * b;

      return ((p * (a * a - b * b) - 2 * b * (a * q + c)) / d, (q * (b * b - a * a) - 2 * a * (b * p + c)) / d);
    }

    /// <summary>
    /// Center Symmetry 
    /// </summary>
    /// <param name="source">Initial Point</param>
    /// <param name="center">Center of the Symmetry</param>
    /// <returns>New point</returns>
    public static (double x, double y) Mirror(this (double x, double y) source, (double x, double y) center) =>
      (2 * center.x - source.x, 2 * center.y - source.y);
    
    /// <summary>
    /// Rotate 
    /// </summary>
    /// <param name="source">Initial Point</param>
    /// <param name="angle">Angle to rotate around</param>
    /// <param name="center">Center to rotate around</param>
    /// <returns>New point</returns>
    public static (double x, double y) Rotate(this (double x, double y) source, double angle, (double x, double y) center) {
      var (x, y) = source;
      var (cx, cy) = center;

      double fi = Math.Atan2(y - cy, x - cx) + angle;
      double r = Math.Sqrt((y - cy) * (y - cy) + (x - cx) * (x - cx));

      return (r * Math.Cos(fi), r * Math.Sin(fi));
    }

    /// <summary>
    /// Distance (Euclidian) between two points 
    /// </summary>
    /// <param name="source">Source point</param>
    /// <param name="target">Target point</param>
    /// <returns>Euclidian distance</returns>
    public static double DistanceTo(this (double x, double y) source, (double x, double y) target) =>
      Math.Sqrt((source.x - target.x) * (source.x - target.x) + (source.y - target.y) * (source.y - target.y));

    /// <summary>
    /// Angle between (source - target) and (0 - x) vectors
    /// </summary>
    /// <param name="source">Source point</param>
    /// <param name="target">Target point</param>
    /// <returns>Angle</returns>
    public static double AngleTo(this (double x, double y) source, (double x, double y) target) =>
      source.x - target.x == 0 && source.y - target.y == 0 // zero tolerance
        ? 0.0
        : Math.Atan2(target.y - source.y, target.x - source.x);

    #endregion Public
  }
}
