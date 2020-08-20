using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Numerics.Distributions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Probability Distribution based on Sample
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class SampleBasedDistribution : ContinuousProbabilityDistribution {
    #region Private Data

    private double m_Sx = 0;
    private double m_Sx2 = 0;
    private double m_Sx3 = 0;
    private double m_Sx4 = 0;

    private readonly List<double> m_RawSample = new List<double>();

    private List<int> m_Histogram;

    #endregion Private Data

    #region Algorithm
    private void CoreProcess(IEnumerable<double> sample) {
      foreach (double x in sample) {
        if (double.IsNaN(x))
          continue;

        m_RawSample.Add(x);

        m_Sx += x;
        m_Sx2 += x * x;
        m_Sx3 += x * x * x;
        m_Sx4 += x * x * x * x;
      }

      m_RawSample.Sort();

      CoreBuildHistogram();
    }

    private void CoreBuildHistogram() {
      int n = (int)(Math.Sqrt(m_RawSample.Count) + 0.5);

      m_Histogram = Enumerable.Repeat(0, n).ToList();

      foreach (var item in m_RawSample) {
        int index = GetBucket(item);

        m_Histogram[index] += 1;
      }
    }

    private double IndexOf(double x) {
      if (m_RawSample.Count <= 0)
        return double.NaN;

      int index = m_RawSample.BinarySearch(x);

      if (index == -1)
        return -1;
      else if (index == -m_RawSample.Count - 1)
        return m_RawSample.Count;

      double left;
      double right;

      if (index >= 0) {
        left = index;
        right = index;

        for (int i = index; i >= 0; --i)
          if (m_RawSample[i] == x)
            left = i;

        for (int i = index; i < m_RawSample.Count; ++i)
          if (m_RawSample[i] == x)
            right = i;

        return (left + right) / 2.0;
      }

      left = -index - 2;
      right = -index - 1;

      double delta = m_RawSample[-index - 1] - m_RawSample[-index - 2];

      return (x - m_RawSample[-index - 2]) / delta * right +
             (m_RawSample[-index - 1] - x) / delta * left;
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="sample">Sample to create distribution from</param>
    public SampleBasedDistribution(IEnumerable<double> sample) {
      if (null == sample)
        throw new ArgumentNullException(nameof(sample));

      CoreProcess(sample);
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Sample  
    /// </summary>
    public IReadOnlyList<double> RawSample => m_RawSample;

    /// <summary>
    /// Histogram
    /// </summary>
    public IReadOnlyList<int> Histogram => m_Histogram;

    /// <summary>
    /// Histogram Bucket Width 
    /// </summary>
    public int BucketWidth { get; private set; }

    /// <summary>
    /// Get Bucket
    /// </summary>
    public int GetBucket(double x) {
      if (m_RawSample.Count <= 0)
        return -1;
      else if (BucketWidth <= 0)
        return 0;

      int result = (int)((x - m_RawSample[0]) / BucketWidth);

      return result < 0 ? 0
        : result >= m_Histogram.Count ? m_Histogram.Count - 1
        : result;
    }

    /// <summary>
    /// Mean
    /// </summary>
    public override double Mean => m_Sx / m_RawSample.Count;

    /// <summary>
    /// Variance
    /// </summary>
    public override double Variance => m_Sx2 / m_RawSample.Count - m_Sx * m_Sx / m_RawSample.Count / m_RawSample.Count;

    /// <summary>
    /// Standard Error
    /// </summary>
    public override double StandardError => Math.Sqrt(Variance);

    /// <summary>
    /// Skew
    /// </summary>
    public Double Skew {
      get {
        if (m_RawSample.Count <= 0)
          return double.NaN;

        Double m1 = m_Sx / m_RawSample.Count;
        Double m2 = m_Sx2 / m_RawSample.Count;
        Double m3 = m_Sx3 / m_RawSample.Count;

        Double s = m3 - 3 * m2 * m1 + 2 * m1 * m1 * m1;
        Double d = m2 - m1 * m1;

        if (d == 0)
          if (s < 0)
            return Double.NegativeInfinity;
          else
            return Double.PositiveInfinity;
        else
          return s / Math.Sqrt(d * d * d);
      }
    }

    /// <summary>
    /// Kurtosis
    /// </summary>
    public Double Kurtosis {
      get {
        if (m_RawSample.Count <= 0)
          return double.NaN;

        Double m1 = m_Sx / m_RawSample.Count;
        Double m2 = m_Sx2 / m_RawSample.Count;
        Double m3 = m_Sx3 / m_RawSample.Count;
        Double m4 = m_Sx4 / m_RawSample.Count;

        Double s = m4 - 4 * m3 * m1 + 6 * m2 * m1 * m1 - 3 * m1 * m1 * m1 * m1;
        Double d = m2 - m1 * m1;

        if (d == 0)
          if (s < 0)
            return Double.NegativeInfinity;
          else
            return Double.PositiveInfinity;
        else
          return s / d * d - 3.0;
      }
    }

    /// <summary>
    /// Probability Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Probability_density_function"/>
    public override double Pdf(double x) {
      int index = GetBucket(x);

      if (index < 0 || index >= m_Histogram.Count)
        return 0;

      return m_Histogram[index] / (BucketWidth * m_RawSample.Count);
    }

    /// <summary>
    /// Quantile Distribution Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Quantile_function"/>
    public override double Qdf(double x) {
      if (x < 0 || x > 1)
        throw new ArgumentOutOfRangeException(nameof(x));

      if (m_RawSample.Count <= 0)
        return double.NaN;

      if (0 == x)
        return m_RawSample[0];
      else if (1 == x)
        return m_RawSample[m_RawSample.Count - 1];

      double index = x * (m_RawSample.Count - 1);
      double frac = index % 1;

      return (1 - frac) * m_RawSample[(int)index] + frac * m_RawSample[(int)index + 1];
    }

    /// <summary>
    /// Cumulative Density Function
    /// </summary>
    /// <see cref="https://en.wikipedia.org/wiki/Cumulative_distribution_function"/>
    public override double Cdf(double x) {
      double index = IndexOf(x);

      if (index < 0)
        return 0.0;
      else if (index >= m_RawSample.Count)
        return 1.0;
      else if (m_RawSample.Count == 1)
        return 0.5;

      return index / (m_RawSample.Count - 1);
    }

    /// <summary>
    /// To String (debug only)
    /// </summary>
    public override string ToString() => $"Sample-based distribution";

    #endregion Public
  }

}
