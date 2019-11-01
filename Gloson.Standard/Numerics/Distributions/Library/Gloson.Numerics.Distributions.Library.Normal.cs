using System;
using System.Collections.Generic;
using System.Text;

using Gloson.Numerics.Calculus;
using Gloson.Numerics.Distributions;
using Gloson.Numerics.SpecialFunctions;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Normal Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Normal_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class NormalProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Private Data

    private static readonly double factor = Math.Sqrt(Math.PI * 2);

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="mean">Mean (mu)</param>
    /// <param name="sigma">Standard Error (sigma)</param>
    public NormalProbabilityDistribution(double mean, double sigma) {
      if (double.IsInfinity(mean))
        throw new ArgumentOutOfRangeException(nameof(mean), "value must be finite");
      else if (double.IsInfinity(sigma))
        throw new ArgumentOutOfRangeException(nameof(sigma), "value must be finite");
      else if (sigma <= 0)
        throw new ArgumentOutOfRangeException(nameof(sigma), "value must be positive");

      Mean = mean;
      StandardError = sigma;
    }

    /// <summary>
    /// Standard constructor for Mean (mu) = 0.0, Standard Error (sigma) = 1.0
    /// </summary>
    public NormalProbabilityDistribution()
      : this(0.0, 1.0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Mean (mu)
    /// </summary>
    public override double Mean { get; }

    /// <summary>
    /// Standard Error (sigma)
    /// </summary>
    public override double StandardError { get; }

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Normal Probability Distribution with mean = {Mean}; sd = {StandardError}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      return 0.5 + ProbabilityIntegral.Erf((x - Mean) / (Math.Sqrt(2) * StandardError)) / 2.0;
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      Double v = (x - Mean) / StandardError;

      return Math.Exp(-v * v / 2.0) / (StandardError * factor);
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

      return Mean + StandardError * Math.Sqrt(2) * erf.InverseAt(2.0 * x - 1, Mean - 10.0 * StandardError, Mean + 10.0 * StandardError);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
