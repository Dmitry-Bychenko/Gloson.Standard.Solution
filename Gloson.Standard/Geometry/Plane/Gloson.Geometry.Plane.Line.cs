using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Geometry.Plane {
  
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public struct Line2D {
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
    /// To String
    /// </summary>
    public override string ToString() {
      StringBuilder sb = new StringBuilder();

      if (A == -1)
        sb.Append("-y");
      else if (A == 1)
        sb.Append("y");
      else if (A != 0){
        sb.Append(A);
        sb.Append(" * y");
      }

      if (B != 0) {
        sb.Append(" ");

        if (B == -1)
          sb.Append("- x");
        else if (B == 1)
          sb.Append("x");
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
  }

}
