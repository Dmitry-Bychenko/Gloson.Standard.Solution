using Gloson.Numerics.SpecialFunctions;
using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Fisher Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/F-distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class FisherProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="degreeOfFreedom1">Degree Of Freedom 1</param>
    /// <param name="degreeOfFreedom2">Degree Of Freedom 2</param>
    public FisherProbabilityDistribution(double degreeOfFreedom1, double degreeOfFreedom2) {
      if (degreeOfFreedom1 <= 0)
        throw new ArgumentOutOfRangeException(nameof(degreeOfFreedom1), "value must be positive");
      else if (degreeOfFreedom2 <= 0)
        throw new ArgumentOutOfRangeException(nameof(degreeOfFreedom2), "value must be positive");

      DegreeOfFreedom1 = degreeOfFreedom1;
      DegreeOfFreedom2 = degreeOfFreedom2;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Degree Of Freedom 1
    /// </summary>
    public double DegreeOfFreedom1 { get; }

    /// <summary>
    /// Degree Of Freedom 2
    /// </summary>
    public double DegreeOfFreedom2 { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => DegreeOfFreedom2 > 2.0
      ? DegreeOfFreedom2 / (DegreeOfFreedom2 - 2.0)
      : double.PositiveInfinity;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => DegreeOfFreedom2 > 4.0
      ? Math.Sqrt(2 * (DegreeOfFreedom1 + DegreeOfFreedom2 - 2.0) / DegreeOfFreedom1 / (DegreeOfFreedom2 - 4)) * DegreeOfFreedom2 / (DegreeOfFreedom2 - 2)
      : double.PositiveInfinity;

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Fisher Distribution with df1 = {DegreeOfFreedom1}; df2 = {DegreeOfFreedom2}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x <= 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be positive");

      return GammaFunctions.BetaIncompleteRegular(DegreeOfFreedom1 * x / (DegreeOfFreedom1 * x + DegreeOfFreedom2),
                                                  DegreeOfFreedom1 / 2,
                                                  DegreeOfFreedom2 / 2);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x <= 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be positive");

      return Math.Sqrt(Math.Pow(DegreeOfFreedom1 * x, DegreeOfFreedom1) *
                       Math.Pow(DegreeOfFreedom2, DegreeOfFreedom2) /
                       Math.Pow(DegreeOfFreedom1 * x + DegreeOfFreedom2, DegreeOfFreedom1 + DegreeOfFreedom2)) /
             (x * GammaFunctions.BetaFunc(DegreeOfFreedom1 / 2, DegreeOfFreedom2 / 2));
    }

    #endregion IContinuousProbabilityDistribution
  }

}
