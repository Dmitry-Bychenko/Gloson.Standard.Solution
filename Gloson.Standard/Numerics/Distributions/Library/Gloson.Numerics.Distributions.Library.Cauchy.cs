using System;

namespace Gloson.Numerics.Distributions.Library {
  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Normal Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Cauchy_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------
  public sealed class CauchyProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="x0">Offset</param>
    /// <param name="Gamma">Gamma parameter</param>
    public CauchyProbabilityDistribution(double x0, double gamma) {
      if (gamma <= 0)
        throw new ArgumentOutOfRangeException(nameof(gamma), "gamma parameter must be positive");

      X0 = x0;
      Gamma = gamma;
    }

    /// <summary>
    /// Standard constructor for Mean (mu) = 0.0, Standard Error (sigma) = 1.0
    /// </summary>
    public CauchyProbabilityDistribution()
      : this(0.0, 1.0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Mean (mu)
    /// </summary>
    public double X0 { get; }

    /// <summary>
    /// Standard Error (sigma)
    /// </summary>
    public double Gamma { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => double.PositiveInfinity;

    /// <summary>
    /// Variance
    /// </summary>
    public override double Variance => double.PositiveInfinity;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => double.PositiveInfinity;

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() =>
      $"Normal Probability Distribution with offset = {X0}; gamma = {Gamma}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      return 0.5 + Math.Atan((x - X0) / Gamma) / Math.PI;
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      return 1.0 / (Math.PI * Gamma * (1 + (x - X0) * (x - X0) / Gamma / Gamma));
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      return X0 + Gamma * Math.Tan(Math.PI * (x - 0.5));
    }

    #endregion IContinuousProbabilityDistribution
  }

}
