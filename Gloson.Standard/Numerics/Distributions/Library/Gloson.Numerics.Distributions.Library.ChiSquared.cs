using Gloson.Numerics.SpecialFunctions;
using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Chi-Squared Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Chi-squared_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ChiSquaredProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="degreeOfFreedom">Degree Of Freedom (n - 1)</param>
    public ChiSquaredProbabilityDistribution(double degreeOfFreedom) {
      if (degreeOfFreedom <= 0)
        throw new ArgumentOutOfRangeException(nameof(degreeOfFreedom), "value must be positive");

      DegreeOfFreedom = degreeOfFreedom;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Degree Of Freedom
    /// </summary>
    public double DegreeOfFreedom { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => DegreeOfFreedom;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt(2 * DegreeOfFreedom);

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Chi-Squared Distribution with df = {DegreeOfFreedom}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x <= 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be positive");

      return GammaFunctions.GammaLowRegular(DegreeOfFreedom / 2, x / 2);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x <= 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be positive");

      return Math.Pow(x, DegreeOfFreedom / 2 - 1) * Math.Exp(-x / 2) / (Math.Pow(2, DegreeOfFreedom / 2) * GammaFunctions.Gamma(DegreeOfFreedom / 2));
    }

    #endregion IContinuousProbabilityDistribution
  }

}
