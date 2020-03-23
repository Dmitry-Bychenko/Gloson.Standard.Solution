using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Triangular Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Triangular_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class TriangularProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="from">Leftmost point (a)</param>
    /// <param name="to">Rightmost point (b)</param>
    /// <param name="mode">Mode (c)</param>
    public TriangularProbabilityDistribution(double from, double to, double mode) {
      if (double.IsInfinity(from))
        throw new ArgumentOutOfRangeException(nameof(from), "value must be finite");
      else if (double.IsInfinity(to))
        throw new ArgumentOutOfRangeException(nameof(to), "value must be finite");
      else if (double.IsInfinity(mode))
        throw new ArgumentOutOfRangeException(nameof(mode), "value must be finite");

      if (from >= to)
        throw new ArgumentOutOfRangeException(nameof(to), "empty [from..to) interval");
      else if (from > mode)
        throw new ArgumentOutOfRangeException(nameof(mode), "wrong mode location (mode < from)");
      else if (mode > to)
        throw new ArgumentOutOfRangeException(nameof(mode), "wrong mode location (mode > to)");

      From = from;
      To = to;
      Mode = mode;
    }

    /// <summary>
    /// Standard constructor (isosceles triangle)
    /// </summary>
    /// <param name="from">Leftmost point (a)</param>
    /// <param name="to">Rightmost point (b)</param>
    public TriangularProbabilityDistribution(double from, double to)
      : this(from, to, (from + to) / 2.0) { }

    /// <summary>
    /// Standard constructor (isosceles triangle with 0..1 range)
    /// </summary>
    public TriangularProbabilityDistribution()
      : this(0.0, 1.0, 0.5) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Leftmost point
    /// </summary>
    public double From { get; }

    /// <summary>
    /// Rightmost point
    /// </summary>
    public double To { get; }

    /// <summary>
    /// Mode
    /// </summary>
    public double Mode { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean {
      get {
        return (From + To + Mode) / 3.0;
      }
    }

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError {
      get {
        return Math.Sqrt((From * From + To * To + Mode * Mode - From * To - From * Mode - To * Mode) / 18.0);
      }
    }

    /// <summary>
    /// To String (debug)
    /// </summary>
    public override string ToString() => $"Triangular Probability Distribution with left = {From}, right = {To}, mode {Mode}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      return
          x <= From ? 0.0
        : x < Mode ? (x - From) * (x - From) / (To - From) / (Mode - From)
        : x == Mode ? (Mode - From) / (To - From)
        : x < To ? 1.0 - (To - x) * (To - x) / (To - From) / (To - Mode)
        : 1.0;
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      return
          x < From ? 0.0
        : x < Mode ? 2.0 * (x - From) / (To - From) / (Mode - From)
        : x == Mode ? 2.0 / (To - From)
        : x < To ? 2 * (To - x) / (To - From) / (To - Mode)
        : 0.0;
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    /// <seealso cref="https://www.boost.org/doc/libs/1_68_0/libs/math/doc/html/math_toolkit/dist_ref/dists/triangular_dist.html"/>
    public override double Qdf(double x) {
      if (x == 0)
        return From;
      else if (x == 1)
        return To;
      else if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));

      double a = From;
      double b = To;
      double c = Mode;

      double p0 = (c - a) / (b - a);

      if (x < p0)
        return Math.Sqrt((b - a) * (c - a) * x) + a;
      else if (x == p0)
        return c;
      else
        return b - Math.Sqrt((b - a) * (b - c) * (1.0 - x));
    }

    #endregion IContinuousProbabilityDistribution
  }

}
