using System;
using System.Diagnostics.CodeAnalysis;

namespace Gloson {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Elementary Math functions (addition to System.Math)
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class ElementaryMath {
    #region Constants

    /// <summary>
    /// Euler constant (E)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public const double E = 2.7182818284590452356028747;

    /// <summary>
    /// Euler constant (C)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public const double C = 0.577215664901532860606512090082;

    /// <summary>
    /// Euler constant (Gamma)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public const double Gamma = 1.7810724179901979852365041031065;

    /// <summary>
    /// Gold ratio
    /// (sqrt(5) + 1) / 2
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public const double FI = 1.6180339887498948482045868343656;

    /// <summary>
    /// Katalan number (G)
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1704")]
    public const double G = 0.915965594177219015054603514932384110774;

    #endregion Constants

    #region Public

    /// <summary>
    /// Area Sine (Hyperbolic)
    /// </summary>
    public static double Asinh(double value) => Math.Log(value + Math.Sqrt(value * value + 1.0));

    /// <summary>
    /// Area Cosine (Hyperbolic)
    /// </summary>
    public static double Acosh(double value) => Math.Log(value + Math.Sqrt(value * value - 1.0));

    /// <summary>
    /// Area Tangent (Hyperbolic)
    /// </summary>
    public static double Atanh(double value) => Math.Log((1.0 + value) / (1.0 - value)) / 2.0;

    #endregion Public
  }
}
