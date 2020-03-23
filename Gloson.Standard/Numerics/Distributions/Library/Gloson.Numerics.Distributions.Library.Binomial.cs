using Gloson.Numerics.SpecialFunctions;
using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Binomial Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Binomial_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BinomialProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="count">Count (n)</param>
    /// <param name="p">Probability (p)</param>
    public BinomialProbabilityDistribution(double count, double p) {
      if (p < 0 || p > 1)
        throw new ArgumentOutOfRangeException(nameof(p), "value must be in [0..1] range");
      else if (count < 0)
        throw new ArgumentOutOfRangeException(nameof(count), "value must not be negative");
      else if (double.IsInfinity(count))
        throw new ArgumentOutOfRangeException(nameof(count), "value must be finite");

      P = p;
      Count = count;
    }

    /// <summary>
    /// Standard constructor (p = 0.5)
    /// </summary>
    /// <param name="count">Count (n)</param>
    public BinomialProbabilityDistribution(double count)
      : this(count, 0.5) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Count (N)
    /// </summary>
    public double Count { get; }

    /// <summary>
    /// Probability (P)
    /// </summary>
    public double P { get; }

    /// <summary>
    /// Probability (Q)
    /// </summary>
    public double Q => 1.0 - P;

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => Count * P;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt(Count * P * Q);

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Binomial Distribution with p = {P}; N = {Count}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x <= 0)
        return 0.0;
      else if (x >= Count)
        return 1.0;

      return GammaFunctions.BetaIncomplete(Q, Count - x, x + 1);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    /// <seealso cref="https://proofwiki.org/wiki/Binomial_Coefficient_expressed_using_Beta_Function"/>
    public override double Pdf(double x) {
      if (x < 0.0 || x > Count)
        return 0;

      double coef = 1.0 / (x + 1) / GammaFunctions.BetaFunc(x + 1, Count - x + 1);

      return coef * Math.Pow(P, x) * Math.Pow(Q, Count - x);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
