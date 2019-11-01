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
  /// Gamma Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Gamma_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class GammaProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="shape">Shape</param>
    /// <param name="scale">Scale (Rate = 1 / Scale)</param>
    public GammaProbabilityDistribution(double shape, double scale) {
      if (double.IsInfinity(shape))
        throw new ArgumentOutOfRangeException(nameof(shape), "value must be finite");
      else if (double.IsInfinity(scale))
        throw new ArgumentOutOfRangeException(nameof(scale), "value must be positive");

      if (shape <= 0)
        throw new ArgumentOutOfRangeException(nameof(shape), "value must be positive");
      else if (scale <= 0)
        throw new ArgumentOutOfRangeException(nameof(scale), "value must be positive");

      Shape = shape;
      Scale = scale;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Shape (k, alpha)
    /// </summary>
    public double Shape { get; }

    /// <summary>
    /// Scale (theta)
    /// </summary>
    public double Scale { get; }

    /// <summary>
    /// Rate (beta)
    /// </summary>
    public double Rate => 1.0 / Scale;

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => Shape * Scale;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt(Shape) * Scale;

    /// <summary>
    /// To String (Debug only)
    /// </summary>
    public override string ToString() =>
      $"Gamma Probability Distribution with k = {Shape}; theta = {Scale}; alpha = {Shape}; beta = {Rate}";

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

      return GammaFunctions.GammaLow(Shape, x / Scale) / GammaFunctions.Gamma(Shape);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      if (x < 0)
        throw new ArgumentOutOfRangeException(nameof(x), "value must be non negative");
      else if (x == 0)
        return Shape <= 1 ? double.PositiveInfinity : 0.0;

      return Math.Pow(x, Shape - 1) * 
             Math.Exp(-x / Scale) /
            (GammaFunctions.Gamma(Shape) * Math.Pow(Scale, Shape));
    }

    #endregion IContinuousProbabilityDistribution
  }

}
