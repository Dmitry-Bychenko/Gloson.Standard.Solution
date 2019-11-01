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
  /// Pert Probability Distribution
  /// </summary>
  /// <seealso cref="https://en.wikipedia.org/wiki/PERT_distribution"/>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class PertProbabilityDistribution : ContinuousProbabilityDistribution {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="count">Count (n)</param>
    /// <param name="p">Probability (p)</param>
    public PertProbabilityDistribution(double a, double b, double c) {
      if (b <= a)
        throw new ArgumentOutOfRangeException(nameof(b), "b must be bigger than a");
      else if (c <= b)
        throw new ArgumentOutOfRangeException(nameof(c), "c must be bigger than b");

      A = a;
      B = b;
      C = c;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// A
    /// </summary>
    public double A { get; }

    /// <summary>
    /// B
    /// </summary>
    public double B { get; }

    /// <summary>
    /// C
    /// </summary>
    public double C { get; }

    /// <summary>
    /// Alpha
    /// </summary>
    public double Alpha => 1.0 + 4.0 * (B - A) / (C - A);

    /// <summary>
    /// Beta
    /// </summary>
    public double Beta => 1.0 + 4.0 * (C - B) / (C - A);

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => (A + 4.0 * B + C) / 6.0;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt((Mean - A) * (C - Mean) / 7.0);

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Pert Distribution with A = {A}; B = {B}; C = {C}";

    #endregion Public

    #region IContinuousProbabilityDistribution

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      if (x <= A)
        return 0.0;
      else if (x >= C)
        return 1.0;

      return GammaFunctions.BetaIncomplete((x - A) / (C - A), Alpha, Beta);
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    /// <seealso cref="https://proofwiki.org/wiki/Binomial_Coefficient_expressed_using_Beta_Function"/>
    public override double Pdf(double x) {
      if (x <= A)
        return 0.0;
      else if (x >= C)
        return 0.0;

      return Math.Pow(x - A, Alpha - 1) *
             Math.Pow(C - x, Beta - 1) /
             GammaFunctions.BetaFunc(Alpha, Beta) /
             Math.Pow(C - A, Alpha + Beta - 1);
    }

    #endregion IContinuousProbabilityDistribution
  }

}
