using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Pareto Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Pareto_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ParetoProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="mean">Mean (mu)</param>
    /// <param name="sigma">Standard Error (sigma)</param>
    public ParetoProbabilityDistribution(double shape, double scale) {
      Shape = shape > 0 ? shape : throw new ArgumentOutOfRangeException(nameof(shape));
      Scale = scale > 0 ? scale : throw new ArgumentOutOfRangeException(nameof(scale));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Shape (alpha)
    /// </summary>
    public double Shape { get; }

    /// <summary>
    /// Scale (Xm)
    /// </summary>
    public double Scale { get; }

    /// <summary>
    /// Mean (mu)
    /// </summary>
    public override double Mean => Shape <= 1
      ? double.PositiveInfinity
      : Shape * Scale / (Shape - 1);

    /// <summary>
    /// Standard Error (sigma)
    /// </summary>
    public override double StandardError => Math.Sqrt(Variance);

    /// <summary>
    /// Variance (sigma ** 2)
    /// </summary>
    public override double Variance => Shape <= 2
      ? double.PositiveInfinity
      : Scale * Scale * Shape / (Scale - 1) / (Scale - 1) / (Scale - 2);

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Pareto Probability Distribution with shape = {Shape}; scale = {Scale}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) =>
      x < 1 ? 0 : 1 - Math.Pow(Scale / x, Shape);

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) =>
      x < 1 ? 0 : Shape * Math.Pow(Scale, Shape) / Math.Pow(x, Shape + 1);

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      if (x == 0)
        return 1;
      else if (x == 1)
        return double.PositiveInfinity;
      else if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));

      return Scale / Math.Pow(1 - x, 1 / Shape);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
