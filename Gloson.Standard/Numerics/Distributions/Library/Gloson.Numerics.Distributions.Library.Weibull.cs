using System;
using System.Collections.Generic;
using System.Text;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Normal Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Weibull_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------
  public sealed class WeibullProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    public WeibullProbabilityDistribution(double scale, double shape) {
      Scale = scale > 0 ? scale : throw new ArgumentOutOfRangeException(nameof(scale));
      Shape = shape > 0 ? shape : throw new ArgumentOutOfRangeException(nameof(shape));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Scale
    /// </summary>
    public double Scale { get; }

    /// <summary>
    /// Scale
    /// </summary>
    public double Shape { get; }

    /// <summary>
    /// Mean (mu)
    /// </summary>
    public override double Mean => Scale * SpecialFunctions.GammaFunctions.Gamma(1 + 1 / Shape);

    /// <summary>
    /// Standard Error (sigma)
    /// </summary>
    public override double StandardError =>
      Scale * Math.Sqrt(SpecialFunctions.GammaFunctions.Gamma(1 + 2 / Shape) -
               Math.Pow(SpecialFunctions.GammaFunctions.Gamma(1 + 1 / Shape), 2));

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Weibull Probability Distribution with shape = {Shape}; scale = {Scale}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      return x < 0
        ? 0.0
        : 1 - Math.Exp(-Math.Pow(x / Scale, Shape));
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      return x < 0
        ? 0.0
        : Shape / Scale * Math.Pow(x / Scale, Shape - 1) * Math.Exp(-Math.Pow(-x / Scale, Shape));
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      if (x == 0)
        return 0;
      else if (x == 1)
        return double.PositiveInfinity;
      else if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));

      return Scale * Math.Pow(Math.Abs(1 - x), 1 / Shape);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
