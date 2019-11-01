using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Logistic Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Logistic_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class LogisticProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="mean">Mean (mu)</param>
    /// <param name="scale">Scale (s)</param>
    public LogisticProbabilityDistribution(double mean, double scale) {
      if (double.IsInfinity(mean))
        throw new ArgumentOutOfRangeException(nameof(mean), "value must be finite");
      else if (double.IsInfinity(scale))
        throw new ArgumentOutOfRangeException(nameof(scale), "value must be finite");

      if (scale <= 0)
        throw new ArgumentOutOfRangeException(nameof(scale), "value must be positive");

      Mean = mean;
      Scale = scale;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean { get; }

    /// <summary>
    /// Scale
    /// </summary>
    public double Scale { get; }

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Scale * Math.PI / Math.Sqrt(3);

    /// <summary>
    /// To String (Debug)
    /// </summary>
    public override string ToString() => $"Logistic Distribution with mean = {Mean}; scale = {Scale}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x < 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be positive");
      else if (double.IsNegativeInfinity(x))
        return 0.0;
      else if (double.IsPositiveInfinity(x))
        return 1.0;

      return 1.0 / (1 + Math.Exp((Mean - x) / Scale));
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (double.IsInfinity(x))
        return 0.0;

      double v = Math.Exp((Mean - x) / Scale);

      return v / Scale / (1 + v) / (1 + v);
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));
      else if (x == 0)
        return double.NegativeInfinity;
      else if (x == 1)
        return double.PositiveInfinity;

      return Mean + Scale * Math.Log(x / (1.0 - x));
    }

    #endregion IContinuousProbabilityDistribution
  }
}
