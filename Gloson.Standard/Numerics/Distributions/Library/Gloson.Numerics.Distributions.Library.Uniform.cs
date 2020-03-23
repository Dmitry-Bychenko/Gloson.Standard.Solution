using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Uniform Probability Distribution for [From..To) range
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Uniform_distribution_(continuous)"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class UniformProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="from">Range: from (inclusive)</param>
    /// <param name="to">Range: to (exclusive)</param>
    public UniformProbabilityDistribution(double from, double to) {
      if (double.IsInfinity(from))
        throw new ArgumentOutOfRangeException(nameof(from), "value must be finite");
      else if (double.IsInfinity(to))
        throw new ArgumentOutOfRangeException(nameof(to), "value must be finite");
      else if (from >= to)
        throw new ArgumentOutOfRangeException(nameof(to), "to must be less than from");

      From = from;
      To = to;
    }

    /// <summary>
    /// Standard constructor for [0..1) range
    /// </summary>
    public UniformProbabilityDistribution()
      : this(0.0, 1.0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// From (inclusive)
    /// </summary>
    public double From { get; }

    /// <summary>
    /// To (exclusive)
    /// </summary>
    public double To { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean {
      get {
        return (From + To) / 2.0;
      }
    }

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => (To - From) / 2 / Math.Sqrt(3);

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Uniform Probability Distribution for [{From}..{To}) range";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) =>
        x < From ? 0.0
      : x >= To ? 1.0
      : (x - From) / (To - From);

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) => x < From || x >= To ? 0.0 : 1.0 / (To - From);

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) =>
        x == 0 ? From
      : x == 1 ? To
      : x < 0 || x > 1 ? throw new ArgumentOutOfRangeException(nameof(x))
      : From + x * (To - From);

    #endregion IContinuousProbabilityDistribution
  }

}
