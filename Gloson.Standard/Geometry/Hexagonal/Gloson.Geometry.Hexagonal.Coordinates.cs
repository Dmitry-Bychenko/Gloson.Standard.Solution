using System;
using System.Drawing;

namespace Gloson.Geometry.Hexagonal {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Hexagonal Point
  /// </summary>
  /// <see cref="https://www.redblobgames.com/grids/hexagons/"/>
  /// <see cref="http://devmag.org.za/2013/08/31/geometry-with-hex-coordinates/"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct HexagonalPoint : IEquatable<HexagonalPoint> {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public HexagonalPoint(int x, int y) {
      X = x;
      Y = y;
    }

    /// <summary>
    /// Standard Constructor
    /// </summary>
    public HexagonalPoint(int x, int y, int z) {
      if (x + y + z == 0) {
        X = x;
        Y = y;
      }
      else {
        X = x - z;
        Y = y + z;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// X
    /// </summary>
    public int X { get; }

    /// <summary>
    /// Y
    /// </summary>
    public int Y { get; }

    /// <summary>
    /// Z
    /// </summary>
    public int Z => -X - Y;

    /// <summary>
    /// Distance to other point
    /// </summary>
    public int DistanceTo(HexagonalPoint other) =>
      (Math.Abs(X - other.X) + Math.Abs(Y - other.Y) + Math.Abs(Z - other.Z)) / 2;

    /// <summary>
    /// Cartesian X
    /// </summary>
    public double CartesianX => X + Y / 2.0;

    /// <summary>
    /// Cartesian Y
    /// </summary>
    public double CartesianY => Math.Sqrt(3) * Y / 2.0;

    /// <summary>
    /// Shape (Hexagon)
    /// </summary>
    public PointF[] Shape() {
      double x = CartesianX;
      double y = CartesianY;

      double d = Math.Sqrt(3) / 4;

      return new PointF[] {
        new PointF((float) x, (float) (y + 1)),
        new PointF((float) (x + d), (float) (y + 0.25)),
        new PointF((float) (x + d), (float) (y - 0.25)),
        new PointF((float) x, (float) (y - 1)),
        new PointF((float) (x - d), (float) (y - 0.25)),
        new PointF((float) (x - d), (float) (y + 0.25)),
      };
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => $"({X}, {Y}, {Z})";

    #endregion Public

    #region Operators

    /// <summary>
    /// To PointF 
    /// </summary>
    public PointF ToPointF() => new((float)CartesianX, (float)CartesianY);

    /// <summary>
    /// To PointF
    /// </summary>
    public static implicit operator PointF(HexagonalPoint value) => value.ToPointF();

    /// <summary>
    /// Equals
    /// </summary>
    public static bool operator ==(HexagonalPoint left, HexagonalPoint right) => left.Equals(right);

    /// <summary>
    /// Not Equals
    /// </summary>
    public static bool operator !=(HexagonalPoint left, HexagonalPoint right) => !left.Equals(right);

    #endregion Operators

    #region IEquatable<HexagonalPoint>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(HexagonalPoint other) => X == other.X && Y == other.Y;

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(Object obj) => obj is HexagonalPoint other && Equals(other);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => unchecked((X << 16) | Y);

    #endregion IEquatable<HexagonalPoint>
  }

}
