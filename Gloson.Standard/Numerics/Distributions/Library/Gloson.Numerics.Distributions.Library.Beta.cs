using Gloson.Numerics.SpecialFunctions;
using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Beta Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Beta_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class BetaProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="alpha">Alpha parameter</param>
    /// <param name="beta">Beta parameter</param>
    public BetaProbabilityDistribution(double alpha, double beta) {
      if (double.IsInfinity(alpha))
        throw new ArgumentOutOfRangeException(nameof(alpha), "value must be finite");
      else if (double.IsInfinity(beta))
        throw new ArgumentOutOfRangeException(nameof(beta), "value must be finite");
      else if (alpha <= 0)
        throw new ArgumentOutOfRangeException(nameof(alpha), "value must be positive");
      else if (beta <= 0)
        throw new ArgumentOutOfRangeException(nameof(beta), "value must be positive");

      Alpha = alpha;
      Beta = beta;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Alpha parameter
    /// </summary>
    public double Alpha { get; }

    /// <summary>
    /// Beta parameter
    /// </summary>
    public double Beta { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean {
      get {
        return Alpha / (Alpha + Beta);
      }
    }

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError {
      get {
        return Math.Sqrt(Alpha * Beta / (Alpha + Beta + 1.0)) / (Alpha + Beta);
      }
    }

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Beta Probability Distribution with aplha = {Alpha}; beta = {Beta}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    /// <seealso cref="https://en.wikipedia.org/wiki/Beta_function#Incomplete_beta_function"/>
    public override double Cdf(double x) {
      if (x < 0 || x > 1.0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be in [0..1] range");

      return GammaFunctions.BetaIncompleteRegular(x, Alpha, Beta);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x < 0 || x > 1.0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be in [0..1] range");

      if (Alpha == Beta && Alpha == 1)
        return 1.0;

      if (x == 0)
        return Alpha <= 1 ? double.PositiveInfinity : 0.0;
      else if (x == 1)
        return Beta <= 1 ? double.PositiveInfinity : 0.0;

      return Math.Pow(x, Alpha - 1.0) * Math.Pow(1.0 - x, Beta - 1.0) / GammaFunctions.BetaFunc(Alpha, Beta);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
