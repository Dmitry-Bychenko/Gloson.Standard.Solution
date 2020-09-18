using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Numerics.Distributions {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Shuffle (dependent) sequence to ensure correlation 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ShuffledCorrelation : IReadOnlyList<(double x, double y)> {
    #region Private Data

    private readonly List<double> m_Items;

    private double m_XY;

    #endregion Private Data

    #region Algorithm

    private double ComputeR() {
      ActualR = (m_XY / m_Items.Count - MeanX * MeanY) / Math.Sqrt(VarianceX) / Math.Sqrt(VarianceY);

      return ActualR;
    }

    private double TryToSwap(int i, int j) {
      double s = m_XY - X[i] * Y[i] - X[j] * Y[j] + X[i] * Y[j] + X[j] * Y[i];

      return (s / m_Items.Count - MeanX * MeanY) / Math.Sqrt(VarianceX) / Math.Sqrt(VarianceY);
    }

    private double Swap(int i, int j) {
      m_XY = m_XY - X[i] * Y[i] - X[j] * Y[j] + X[i] * Y[j] + X[j] * Y[i];

      double h = m_Items[i];
      m_Items[i] = m_Items[j];
      m_Items[j] = h;

      return ComputeR();
    }

    private void AdjustR() {
      if (DesiredR >= ActualR)
        return;

      int top = 0;
      int bottom = m_Items.Count - 1;

      while (top < bottom) {
        double nextR = TryToSwap(top, bottom);

        if (DesiredR <= nextR) {
          Swap(top, bottom);

          top += 1;
          bottom -= 1;

          continue;
        }

        double nextR1 = TryToSwap(top + 1, bottom);
        double nextR2 = TryToSwap(top, bottom - 1);

        if (nextR1 >= DesiredR) {
          if (nextR1 <= nextR2 || nextR2 < DesiredR) {
            Swap(top + 1, bottom);

            top += 2;
            bottom -= 1;

            continue;
          }
        }

        if (nextR2 >= DesiredR) {
          if (nextR2 <= nextR1 || nextR1 < DesiredR) {
            Swap(top, bottom - 1);

            top += 1;
            bottom -= 2;

            continue;
          }
        }

        if (nextR1 > nextR2)
          top += 1;
        else
          bottom -= 1;
      }
    }

    private void CoreUpdate() {
      double sumX = 0.0;
      double sumX2 = 0.0;
      double sumY = 0.0;
      double sumY2 = 0.0;
      double sumXY = 0.0;

      int N = m_Items.Count;

      for (int i = 0; i < N; ++i) {
        double x = X[i];
        double y = Y[i];

        sumX += x;
        sumX2 += x * x;
        sumY += y;
        sumY2 += y * y;
        sumXY += x * y;
      }

      m_XY = sumXY;

      MeanX = sumX / N;
      MeanY = sumY / N;

      VarianceX = sumX2 / N - MeanX * MeanX;
      VarianceY = sumY2 / N - MeanY * MeanY;

      ActualR = (m_XY / N - MeanX * MeanY) / Math.Sqrt(VarianceX) / Math.Sqrt(VarianceY);

      AdjustR();
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="source">Source (independent sequence)</param>
    /// <param name="dependent">Dependent sequence to shuffle</param>
    /// <param name="desiredR">Desired correlation</param>
    public ShuffledCorrelation(IEnumerable<double> source, IEnumerable<double> dependent, double desiredR) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == dependent)
        throw new ArgumentNullException(nameof(dependent));
      else if (desiredR < -1 || desiredR > 1)
        throw new ArgumentOutOfRangeException(nameof(desiredR));

      DesiredR = desiredR;

      X = source
        .Where(x => !double.IsNaN(x))
        .OrderBy(x => x)
        .ToList();

      m_Items = dependent
        .Where(y => !double.IsNaN(y))
        .OrderBy(y => y)
        .ToList();

      if (X.Count <= 0)
        throw new ArgumentException("No valid items in source", nameof(source));
      else if (X.Count != m_Items.Count)
        throw new ArgumentException("Both (source and dependent) sequencies must of the same length", nameof(dependent));

      CoreUpdate();
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Desired Correlation (R)
    /// </summary>
    public double DesiredR { get; }

    /// <summary>
    /// Source (independent source)
    /// </summary>
    public IReadOnlyList<double> X { get; }

    /// <summary>
    /// Dependent (correlated) source
    /// </summary>
    public IReadOnlyList<double> Y => m_Items;

    /// <summary>
    /// Actual Correlation (R) 
    /// </summary>
    public double ActualR { get; private set; }

    /// <summary>
    /// Mean X
    /// </summary>
    public double MeanX { get; private set; }

    /// <summary>
    /// Mean Y
    /// </summary>
    public double MeanY { get; private set; }

    /// <summary>
    /// Variance (sigma squared) X
    /// </summary>
    public double VarianceX { get; private set; }

    /// <summary>
    /// Variance (sigma squared) Y
    /// </summary>
    public double VarianceY { get; private set; }

    /// <summary>
    /// To String (debug)
    /// </summary>
    public override string ToString() =>
      $"{X.Count} items with R = {ActualR} correlation";

    /// <summary>
    /// To String
    /// </summary>
    public string ToReport() {
      StringBuilder sb = new StringBuilder();

      sb.AppendLine($"{X.Count} items with R = {ActualR} correlation");

      sb.AppendLine();
      sb.AppendLine($"     ## :                    X :                    Y");
      sb.Append($"-----------------------------------------------------");

      for (int i = 0; i < m_Items.Count; ++i) {
        sb.AppendLine();
        sb.Append($"{i,7} : {X[i],20} : {Y[i],20}");
      }

      return sb.ToString();
    }

    #endregion Public

    #region IReadOnlyList<(double x, double y)>

    /// <summary>
    /// Count
    /// </summary>
    public int Count => m_Items.Count;

    /// <summary>
    /// Indexer
    /// </summary>
    public (double x, double y) this[int index] {
      get {
        return index >= 0 && index < m_Items.Count
          ? (X[index], Y[index])
          : throw new ArgumentOutOfRangeException(nameof(index));
      }
    }

    /// <summary>
    /// Enumerator 
    /// </summary>
    public IEnumerator<(double x, double y)> GetEnumerator() =>
      X.Zip(Y, (x, y) => (x, y)).GetEnumerator();

    /// <summary>
    /// Enumerator
    /// </summary>
    IEnumerator IEnumerable.GetEnumerator() => this.GetEnumerator();

    #endregion IReadOnlyList<(double x, double y)>
  }

}
