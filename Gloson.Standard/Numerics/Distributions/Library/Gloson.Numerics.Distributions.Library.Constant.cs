using System;
using System.Collections.Generic;
using System.Text;

using Gloson.Numerics.Distributions;

namespace Gloson.Numerics.Distributions.Library {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Constant Probability Distribution. 
  /// Random value can have a single constant value only 
  /// PDF is Dirac Delta Function
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/Dirac_delta_function"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ConstantProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="mean">Mean</param>
    public ConstantProbabilityDistribution(double mean) {
      if (double.IsInfinity(mean))
        throw new ArgumentOutOfRangeException(nameof(mean), "value must be finite");
    }

    /// <summary>
    /// Standard constructor (Mean = 0.0)
    /// </summary>
    public ConstantProbabilityDistribution()
      : this(0) { }

    #endregion Create

    #region Public

    /// <summary>
    /// Mean 
    /// </summary>
    public override double Mean { get; }

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => 0.0;

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Constant Probability Distribution mean = {Mean}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) =>
        x < Mean ? 0.0
      : x == Mean ? 0.5
      : 1.0;

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) => x == Mean ? double.PositiveInfinity : 0.0;

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) => x >= 0 && x <= 1.0 ? Mean : throw new ArgumentOutOfRangeException(nameof(x));

    #endregion IContinuousProbabilityDistribution
  }

}
