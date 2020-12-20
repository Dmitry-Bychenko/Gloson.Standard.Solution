using System;
using System.Text;

namespace Gloson.Geometry.Plane {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Line2D
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct Line2D : IEquatable<Line2D> {
    #region Create

    /// <summary>
    /// Standard constructor for A * y + B * x + C = 0
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <param name="c"></param>
    public Line2D(double a, double b, double c) {
      A = a;
      B = b;
      C = c;
    }

    /// <summary>
    /// Standard constructor for y = slope * x + intersept
    /// </summary>
    /// <param name="slope"></param>
    /// <param name="interpect"></param>
    public Line2D(double slope, double interpect) {
      A = 1;
      B = -slope;
      C = -interpect;
    }

    /// <summary>
    /// Standard constructor for y - y0 = k * (x - x0)
    /// </summary>
    /// <param name="slope"></param>
    /// <param name="point"></param>
    public Line2D(double slope, (double x, double y) point) {
      A = 1;
      B = -slope;
      C = slope * point.x - point.y;
    }

    /// <summary>
    /// Line from 2 points
    /// </summary>
    /// <param name="point1">point #1</param>
    /// <param name="point2">point #2</param>
    public Line2D((double x, double y) point1, (double x, double y) point2) {
      if (point1.x == point2.x) {
        A = 0;
        B = 1;
        C = -point1.x;
      }
      else {
        A = point2.x - point1.x;
        B = point1.y - point2.y;
        C = point1.y * (point1.x - point2.x) + point1.x * (point2.y - point1.y);
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// A in A * y + B * x + C = 0
    /// </summary>
    public double A { get; }

    /// <summary>
    /// B in A * y + B * x + C = 0
    /// </summary>
    public double B { get; }

    /// <summary>
    /// C in A * y + B * x + C = 0
    /// </summary>
    public double C { get; }

    /// <summary>
    /// Slope
    /// </summary>
    public double Slope => -B / A;

    /// <summary>
    /// Intercept
    /// </summary>
    public double Intercept => -C / A;

    /// <summary>
    /// Angle
    /// </summary>
    public double Angle {
      get {
        if (0 == B)
          return 0.0;
        else if (0 == A)
          return Math.PI / 2;

        double result = Math.Atan2(C / A, C / B);

        if (result < 0)
          result += Math.PI;
        else if (result > Math.PI)
          result -= Math.PI;

        return result;
      }
    }

    /// <summary>
    /// Two line Intersection  
    /// </summary>
    public (double x, double y) Intersect(Line2D other) =>
      ((A * other.C - other.A * C) / (A * other.B - other.A * B),
       (B * other.C - other.B * C) / (A * other.B - other.A * B));

    /// <summary>
    /// Is other line Parallel
    /// </summary>
    public bool IsParallel(Line2D other) => A * other.B == other.A * B;

    /// <summary>
    /// At
    /// </summary>
    public double At(double x) => (-B * x - C) / A;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();

      if (A == -1)
        sb.Append("-y");
      else if (A == 1)
        sb.Append('y');
      else if (A != 0) {
        sb.Append(A);
        sb.Append(" * y");
      }

      if (B != 0) {
        sb.Append(' ');

        if (B == -1)
          sb.Append("- x");
        else if (B == 1)
          sb.Append('x');
        else {
          sb.Append(B);
          sb.Append(" * x");
        }
      }

      if (C != 0) {
        sb.Append(' ');
        sb.Append(C);
      }

      if (sb.Length == 0)
        return "0 = 0";

      sb.Append(" = 0");

      return sb.ToString();
    }

    #endregion Public

    #region Operators

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(Line2D left, Line2D right) => left.Equals(right);

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(Line2D left, Line2D right) => !left.Equals(right);

    #endregion Operators

    #region IEquatable<Line2D>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(Line2D other) =>
      A * other.B == other.A * B &&
      A * other.C == other.A * C &&
      B * other.C == other.B * C;

    /// <summary>
    /// 
    /// </summary>
    public override bool Equals(object obj) =>
      (obj is Line2D other) && Equals(other);

    /// <summary>
    /// HasCode 
    /// </summary>
    public override int GetHashCode() => ((A == 0) ? B : (B / A)).GetHashCode();

    #endregion IEquatable<Line2D>
  }

}
