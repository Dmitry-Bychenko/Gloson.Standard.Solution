using System;

namespace Gloson.Numerics.Distributions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Trimmed (Original) distribution which now defined in [LeftBorder..RightBorder]
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ElevatedTrimContinuousDistribution : ContinuousProbabilityDistribution {
    #region Private Data

    private readonly double m_Shift;

    private readonly double m_CdfLeft;

    private readonly double m_CdfRight;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard 
    /// </summary>
    /// <param name="origin">Original Distribution</param>
    /// <param name="leftBorder">Left Border</param>
    /// <param name="rightBorder">Right Border</param>
    public ElevatedTrimContinuousDistribution(IContinuousProbabilityDistribution origin,
                                              double leftBorder,
                                              double rightBorder) {
      Origin = origin ?? throw new ArgumentNullException(nameof(origin));

      if (leftBorder <= rightBorder) {
        LeftBorder = leftBorder;
        RightBorder = rightBorder;
      }
      else
        throw new ArgumentOutOfRangeException(nameof(leftBorder), "Empty region");

      m_CdfRight = Origin.Cdf(RightBorder);
      m_CdfLeft = Origin.Cdf(LeftBorder);

      if (m_CdfRight <= 0)
        throw new ArgumentOutOfRangeException(nameof(rightBorder), "Zero density region");

      if (leftBorder != rightBorder && (m_CdfRight - m_CdfLeft <= 0))
        throw new ArgumentOutOfRangeException(nameof(rightBorder), "Zero density region");

      if (LeftBorder != RightBorder)
        m_Shift = (1 - (m_CdfRight - m_CdfLeft)) / (RightBorder - LeftBorder);
      else
        m_Shift = double.NaN;
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Original Distribution
    /// </summary>
    public IContinuousProbabilityDistribution Origin { get; }

    /// <summary>
    /// Left Border
    /// </summary>
    public double LeftBorder { get; }

    /// <summary>
    /// Right Border
    /// </summary>
    public double RightBorder { get; }

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    public override double Cdf(double x) {
      if (x < LeftBorder)
        return 0;
      else if (x >= RightBorder)
        return 1;

      return (Origin.Cdf(x) - m_CdfLeft) +
             (x - LeftBorder) / (RightBorder - LeftBorder) * (m_CdfLeft + 1 - m_CdfRight);
    }

    /// <summary>
    /// Probability Density Function
    /// </summary>
    public override double Pdf(double x) {
      if (LeftBorder == RightBorder)
        return x == LeftBorder ? double.PositiveInfinity : 0;

      if (x < LeftBorder)
        return 0.0;
      else if (x > RightBorder)
        return 0.0;
      else
        return Origin.Pdf(x) + m_Shift;
    }

    /// <summary>
    /// Quantile Function
    /// </summary>
    public override double Qdf(double x) {
      if (x == 0)
        return LeftBorder;
      else if (x == 1)
        return RightBorder;

      return base.Qdf(x);
    }

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"{Origin} within [{LeftBorder}..{RightBorder}] range (elevated)";

    #endregion Public
  }
}
