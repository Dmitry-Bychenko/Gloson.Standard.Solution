using Gloson.Numerics.SpecialFunctions;
using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Poisson Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Poisson_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class PoissonProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="rate">Rate (lambda, a)</param>
    public PoissonProbabilityDistribution(double rate) {
      if (rate <= 0)
        throw new ArgumentOutOfRangeException(nameof(rate), "value must be positive");

      Rate = rate;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Rate (lambda, a)
    /// </summary>
    public double Rate { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => Rate;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt(Rate);

    /// <summary>
    /// To String (Debug Only)
    /// </summary>
    public override string ToString() => $"Poisson Distribution with lambda = {Rate}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x < 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be positive");
      else if (x == 0)
        return 0.0;
      else if (double.IsPositiveInfinity(x))
        return 1.0;

      return GammaFunctions.GammaHigh(x + 1, Rate) / GammaFunctions.Factorial(x);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x < 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be non negative");
      else if (x == 0)
        return 0.0;

      return Math.Pow(Rate, x) * Math.Exp(-Rate) / GammaFunctions.Factorial(x);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
