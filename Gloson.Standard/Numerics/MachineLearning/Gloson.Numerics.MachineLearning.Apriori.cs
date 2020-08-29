using Gloson.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Numerics.MachineLearning {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Lift, Confidence and Support
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------
  public sealed class SupportConfidenceLift<T> {
    #region Private Data

    private readonly List<T> m_Left;
    private readonly List<T> m_Right;
    private readonly List<T> m_LeftAndRight;

    #endregion Private Data

    #region Create

    /// <summary>
    /// Standard Constructor
    /// </summary>
    /// <param name="transactions">Transactions to process</param>
    /// <param name="left">Left (cause)</param>
    /// <param name="right">Right (outcome)</param>
    /// <param name="comparer">Equality Comparer to use</param>
    public SupportConfidenceLift(
      IEnumerable<IEnumerable<T>> transactions,
      IEnumerable<T> left,
      IEnumerable<T> right,
      IEqualityComparer<T> comparer = null) {

      if (null == transactions)
        throw new ArgumentNullException(nameof(transactions));

      comparer ??= EqualityComparer<T>.Default;

      m_Left = left.ToList();
      m_Right = right.ToList();

      m_LeftAndRight = m_Left
        .UnionWithDuplicates(m_Right)
        .ToList();

      var leftDict = m_Left
        ?.GroupBy(item => item, comparer)
        ?.ToDictionary(group => group.Key, group => group.Count())
        ?? throw new ArgumentNullException(nameof(left));

      var rightDict = m_Right
        ?.GroupBy(item => item, comparer)
        ?.ToDictionary(group => group.Key, group => group.Count())
        ?? throw new ArgumentNullException(nameof(left));

      foreach (IEnumerable<T> record in transactions) {
        CountAll += 1;

        if (null == record)
          continue;

        var dict = record
          .GroupBy(item => item, comparer)
          .ToDictionary(group => group.Key, group => group.Count());

        bool leftFound = (leftDict.All(pair => dict.TryGetValue(pair.Key, out int v) && v >= pair.Value));
        bool rightFound = (rightDict.All(pair => dict.TryGetValue(pair.Key, out int v) && v >= pair.Value));

        if (leftFound) {
          CountLeft += 1;

          if (rightFound) {
            CountRight += 1;
            CountLeftAndRight += 1;
          }
        }
        else if (rightFound)
          CountRight += 1;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Count All
    /// </summary>
    public int CountAll { get; }

    /// <summary>
    /// Count Left
    /// </summary>
    public int CountLeft { get; }

    /// <summary>
    /// Count Right
    /// </summary>
    public int CountRight { get; }

    /// <summary>
    /// Count Left and Right
    /// </summary>
    public int CountLeftAndRight { get; }

    /// <summary>
    /// Left Support
    /// </summary>
    public double LeftSupport => CountAll == 0 ? 1 : ((double)CountLeft) / CountAll;

    /// <summary>
    /// Right Support
    /// </summary>
    public double RightSupport => CountAll == 0 ? 1 : ((double)CountRight) / CountAll;

    /// <summary>
    /// Left And Right Support
    /// </summary>
    public double LeftAndRightSupport => CountAll == 0 ? 1 : ((double)CountLeftAndRight) / CountAll;

    /// <summary>
    /// Left Confidence
    /// </summary>
    public double LeftConfidence => LeftAndRightSupport / LeftSupport;

    /// <summary>
    /// Right Confidence
    /// </summary>
    public double RightConfidence => LeftAndRightSupport / RightSupport;

    /// <summary>
    /// Lift
    /// </summary>
    public double Lift => LeftSupport == 0 || RightSupport == 0
      ? 1
      : LeftAndRightSupport / LeftSupport / RightSupport;

    /// <summary>
    /// Left items
    /// </summary>
    public IReadOnlyList<T> Left => m_Left;

    /// <summary>
    /// Right items
    /// </summary>
    public IReadOnlyList<T> Right => m_Right;

    /// <summary>
    /// Left And Right items
    /// </summary>
    public IReadOnlyList<T> LeftAndRight => m_LeftAndRight;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"[{string.Join(", ", Left)}] ({LeftSupport * 100:G4}%) -> [{string.Join(", ", Right)}] ({RightSupport * 100:G4}%) with lift {Lift:G4}";

    /// <summary>
    /// To Report
    /// </summary>
    public string ToReport() {
      return string.Join(Environment.NewLine,
        $"Transactions:",
        $"  Total:      {CountAll}",
        $"Left (cause):",
        $"  {{{string.Join(", ", Left)}}} ({Left.Count} items)",
        $"  Total:      {CountLeft}",
        $"  Support:    {LeftSupport * 100:G4}%",
        $"  Confidence: {LeftConfidence * 100:G4}%",
        $"Right (outcome):",
        $"  {{{string.Join(", ", Right)}}} ({Right.Count} items)",
        $"  Total:      {CountRight}",
        $"  Support:    {RightSupport * 100:G4}%",
        $"  Confidence: {RightConfidence * 100:G4}%",
        $"Left And Right:",
        $"  {{{string.Join(", ", LeftAndRight)}}} ({LeftAndRight.Count} items)",
        $"  Total:      {CountLeftAndRight}",
        $"  Support:    {LeftAndRightSupport * 100:G4}%",
        $"  Lift:       {Lift:G4}"
      );
    }

    #endregion Public
  }
}
