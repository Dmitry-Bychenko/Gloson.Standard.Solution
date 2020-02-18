using System;
using System.Collections.Generic;
using System.Globalization;

namespace Gloson.Numerics.MachineLearning {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Confusion Matrix
  /// </summary>
  // https://en.wikipedia.org/wiki/Precision_and_recall
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class ConfusionMatrix<T> 
    : IEquatable<ConfusionMatrix<T>>,
      IFormattable {

    #region Internal classes

    private class ConfusionMatrixF1Comparer : IComparer<ConfusionMatrix<T>> {
      /// <summary>
      /// Compare
      /// </summary>
      public int Compare(ConfusionMatrix<T> x, ConfusionMatrix<T> y) {
        if (ReferenceEquals(x, y))
          return 0;
        else if (ReferenceEquals(x, null))
          return -1;
        else if (ReferenceEquals(null, y))
          return 1;
        else
          return x.F1Score.CompareTo(y.F1Score);
      }
    }

    #endregion Internal classes

    #region Private Data

    // Prediction
    private readonly Func<T, bool> m_PredictedMap;

    // Ground truth
    private readonly Func<T, bool> m_ActualMap;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="predictedMap">Predicted value from given item</param>
    /// <param name="actualMap">Actual value (grpund truth) from item</param>
    public ConfusionMatrix(Func<T, bool> predictedMap, Func<T, bool> actualMap) {
      if (null == predictedMap)
        throw new ArgumentNullException(nameof(predictedMap));
      else if (null == predictedMap)
        throw new ArgumentNullException(nameof(actualMap));

      m_PredictedMap = predictedMap;
      m_ActualMap = actualMap;
    }

    #endregion Create 

    #region Public

    #region General

    /// <summary>
    /// F1 Comparer
    /// </summary>
    public static IComparer<ConfusionMatrix<T>> F1Comparer {
      get;
    } = new ConfusionMatrixF1Comparer();

    #endregion General

    #region Standard

    /// <summary>
    /// Add
    /// </summary>
    public long Add(T value) {
      bool predicted = m_PredictedMap(value);
      bool actual = m_ActualMap(value);

      if (predicted)
        if (actual)
          TruePositive += 1;
        else
          FalsePositive += 1;
      else if (actual)
        FalseNegative += 1;
      else
        TrueNegative += 1;

      return Count;
    }

    /// <summary>
    /// Add Range
    /// </summary>
    public long AddRange(IEnumerable<T> source) {
      if (null == source)
        throw new ArgumentNullException(nameof(source));

      foreach (var value in source) {
        bool predicted = m_PredictedMap(value);
        bool actual = m_ActualMap(value);

        if (predicted)
          if (actual)
            TruePositive += 1;
          else
            FalsePositive += 1;
        else if (actual)
          FalseNegative += 1;
        else
          TrueNegative += 1;
      }

      return Count;
    }

    /// <summary>
    /// True Positive
    /// </summary>
    public long TruePositive {
      get;
      private set;
    }

    /// <summary>
    /// True Negative
    /// </summary>
    public long TrueNegative {
      get;
      private set;
    }

    /// <summary>
    /// False Positive
    /// </summary>
    public long FalsePositive {
      get;
      private set;
    }

    /// <summary>
    /// False Negative
    /// </summary>
    public long FalseNegative {
      get;
      private set;
    }

    /// <summary>
    /// Count
    /// </summary>
    public long Count => TruePositive + TrueNegative + FalsePositive + FalseNegative;

    #endregion Standard

    #region Extended

    /// <summary>
    /// Positive
    /// </summary>
    public long Positive => TruePositive + FalseNegative;

    /// <summary>
    /// Negative
    /// </summary>
    public long Negative => TrueNegative + FalsePositive;

    /// <summary>
    /// Positive
    /// </summary>
    public long PositivePredicted => TruePositive + FalsePositive;

    /// <summary>
    /// Negative
    /// </summary>
    public long NegativePredicted => TrueNegative + FalseNegative;

    /// <summary>
    /// True
    /// </summary>
    public long True => TruePositive + TrueNegative;

    /// <summary>
    /// False
    /// </summary>
    public long False => FalsePositive + FalseNegative;

    /// <summary>
    /// Precision (PPR - Positive Predicted Value)
    /// </summary>
    public double Precision => (TruePositive + FalsePositive) == 0
      ? 1.0
      : (double)TruePositive / (TruePositive + FalsePositive);

    /// <summary>
    /// Recall (TPR - True Positive Rate)
    /// </summary>
    public double Recall => (TruePositive + FalseNegative) == 0
      ? 1.0
      : (double)TruePositive / (TruePositive + FalseNegative);

    /// <summary>
    /// Selectivity (TNR - True Negative Rate)
    /// </summary>
    public double Selectivity => (TrueNegative + FalsePositive) == 0
      ? 1.0
      : (double)TrueNegative / (TrueNegative + FalsePositive);

    /// <summary>
    /// Accuracy
    /// </summary>
    public double Accuracy => Count == 0
      ? 1.0
      : (double)True / Count;

    /// <summary>
    /// Accuracy Balanced
    /// </summary>
    public double AccuracyBalanced => (Recall + Selectivity) / 2.0;

    /// <summary>
    /// F1 score
    /// </summary>
    public double F1Score => (TruePositive + FalsePositive + FalseNegative) == 0
      ? 1.0
      : 2.0 * TruePositive / (2.0 * TruePositive + FalsePositive + FalseNegative);

    #endregion Extended

    #endregion Public 

    #region Operators

    /// <summary>
    /// Equal
    /// </summary>
    public static bool operator ==(ConfusionMatrix<T> left, ConfusionMatrix<T> right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (ReferenceEquals(null, right) || ReferenceEquals(left, null))
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(ConfusionMatrix<T> left, ConfusionMatrix<T> right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (ReferenceEquals(null, right) || ReferenceEquals(left, null))
        return true;
      else
        return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<ConfusionMatrix<T>>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(ConfusionMatrix<T> other) {
      if (ReferenceEquals(this, other))
        return true;
      else if (ReferenceEquals(null, other))
        return false;

      return TruePositive == other.TruePositive &
             TrueNegative == other.TrueNegative &
             FalsePositive == other.FalsePositive &
             FalseNegative == other.FalseNegative;
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) {
      return base.Equals(obj as ConfusionMatrix<T>);
    }

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() {
      unchecked {
        return (int)(TruePositive * 17 +
                     TrueNegative * 23 +
                     FalsePositive * 31 +
                     FalseNegative * 47);
      }
    }

    #endregion IEquatable<ConfusionMatrix<T>> 

    #region IFormatable

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format, IFormatProvider formatProvider) {
      if (string.IsNullOrWhiteSpace(format))
        format = "F3";

      if (null == formatProvider)
        formatProvider = CultureInfo.InvariantCulture;

      return string.Concat(
        "Precision: ",
         Precision.ToString(format, formatProvider),
        "; Recall: ",
         Recall.ToString(format, formatProvider),
        "; F1 score: ",
         F1Score.ToString(format, formatProvider));
    }

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(string format) => ToString(format, CultureInfo.InvariantCulture);

    /// <summary>
    /// To String
    /// </summary>
    public string ToString(IFormatProvider formatProvider) => ToString(null, formatProvider);

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() => ToString(null, CultureInfo.InvariantCulture);

    #endregion IFormatable
  }

}
