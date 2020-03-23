using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Bernoulli Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Bernoulli_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BernoulliProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="p">Probability (p)</param>
    public BernoulliProbabilityDistribution(double p) {
      if (p < 0 || p > 1)
        throw new ArgumentOutOfRangeException(nameof(p), "value must be in [0..1] range");

      P = p;
    }

    /// <summary>
    /// Standard constructor (p = 0.5)
    /// </summary>
    public BernoulliProbabilityDistribution() :
      this(0.5) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Probability (p)
    /// </summary>
    public double P { get; }

    /// <summary>
    /// Probability (q)
    /// </summary>
    public double Q => 1.0 - P;

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => P;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt(P * Q);

    /// <summary>
    /// To String (Debug)
    /// </summary>
    public override string ToString() => $"Bernoulli Distribution with p = {P}; q = {Q}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x < 0)
        return 0.0;
      else if (x > 1)
        return 1.0;

      return Q + P * x;
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x < 0.0 || x > 1.0)
        return 0;

      return x * P + (1.0 - x) * Q;
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));
      else if (x <= Q)
        return 0.0;
      else if (x == 1)
        return 1.0;

      return x / P + (1 - 1 / P);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
