using Gloson.Numerics.Calculus;
using Gloson.Numerics.SpecialFunctions;
using System;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Log-normal Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Log-normal_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LogNormalProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Private Data

    private static readonly double factor = Math.Sqrt(Math.PI * 2);

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="mu">Standard mu parameter</param>
    /// <param name="sigma">Standard sigma parameter</param>
    public LogNormalProbabilityDistribution(double mu, double sigma) {
      if (double.IsInfinity(mu))
        throw new ArgumentOutOfRangeException(nameof(mu), "value must be finite");
      else if (double.IsInfinity(sigma))
        throw new ArgumentOutOfRangeException(nameof(sigma), "value must be finite");
      else if (sigma <= 0)
        throw new ArgumentOutOfRangeException(nameof(sigma), "value must be positive");

      Mu = mu;
      Sigma = sigma;
    }

    /// <summary>
    /// Standard constructor (mu = 0.0, sigma = 1.0)
    /// </summary>
    public LogNormalProbabilityDistribution()
      : this(0.0, 1.0) {
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Standard Mu Parameter
    /// </summary>
    public double Mu { get; }

    /// <summary>
    /// Standard Sigma Parameter
    /// </summary>
    public double Sigma { get; }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean {
      get {
        return Math.Exp(Mu + Sigma * Sigma);
      }
    }

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError {
      get {
        return Math.Sqrt((Math.Exp(Sigma * Sigma) - 1.0) * Math.Sqrt(2 * Mu + Sigma * Sigma));
      }
    }

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Log-normal Probability Distribution with mu = {Mu}; sigma = {Sigma}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x < 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must not be negative");
      else if (x == 0)
        return 0.0;

      return 0.5 + ProbabilityIntegral.Erf((Math.Log(x) - Mu) / Sigma / Math.Sqrt(2)) / 2.0;
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x < 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must not be negative");
      else if (x == 0)
        return 0.0;

      double v = (Math.Log(x) - Mu) / Sigma;

      return Math.Exp(-v * v / 2.0) / (x * Sigma * factor);
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      if (x == 0)
        return double.NegativeInfinity;
      else if (x == 1)
        return double.PositiveInfinity;
      else if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));

      Func<double, double> erf = ProbabilityIntegral.Erf;

      return Math.Exp(Mu + Math.Sqrt(2) * Sigma * erf.InverseAt(2 * x - 1, Mu - 10.0 * Sigma, Mu + 10.0 * Sigma));
    }

    #endregion IContinuousProbabilityDistribution
  }
}
