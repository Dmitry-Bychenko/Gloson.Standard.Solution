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

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Apriori Rule
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class AprioriRule<T> {
    #region Create

    /// <summary>
    /// Standard constructor
    /// </summary>
    /// <param name="left">Left subset (cause)</param>
    /// <param name="right">Right subset (outcome)</param>
    /// <param name="leftSupport">Left Support</param>
    /// <param name="rightSupport">Right Support</param>
    /// <param name="leftAndRightSupport">Left and Right Support</param>
    public AprioriRule(
      IEnumerable<T> left,
      IEnumerable<T> right,
      double leftSupport,
      double rightSupport,
      double leftAndRightSupport) {

      Left = left?.ToList() ?? throw new ArgumentNullException(nameof(left));
      Right = right?.ToList() ?? throw new ArgumentNullException(nameof(right));

      LeftSupport = leftSupport >= 0 && leftSupport <= 1
        ? leftSupport
        : throw new ArgumentOutOfRangeException(nameof(leftSupport));

      RightSupport = rightSupport >= 0 && rightSupport <= 1
        ? rightSupport
        : throw new ArgumentOutOfRangeException(nameof(rightSupport));

      LeftAndRightSupport = leftAndRightSupport >= 0 && leftAndRightSupport <= Math.Min(leftSupport, rightSupport)
        ? leftAndRightSupport
        : throw new ArgumentOutOfRangeException(nameof(leftAndRightSupport));
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Left subset (cause)
    /// </summary>
    public IReadOnlyList<T> Left { get; }

    /// <summary>
    /// Right subset (outcome)
    /// </summary>
    public IReadOnlyList<T> Right { get; }

    /// <summary>
    /// Left Support
    /// </summary>
    public double LeftSupport { get; }

    /// <summary>
    /// Right Support
    /// </summary>
    public double RightSupport { get; }

    /// <summary>
    /// Left And Right Support
    /// </summary>
    public double LeftAndRightSupport { get; }

    /// <summary>
    /// Confidence
    /// </summary>
    public double Confidence => LeftSupport == 0
      ? 0.0
      : LeftAndRightSupport / LeftSupport;

    /// <summary>
    /// Lift
    /// </summary>
    public double Lift => LeftSupport == 0 || RightSupport == 0
      ? 1.0
      : LeftAndRightSupport / LeftSupport / RightSupport;

    /// <summary>
    /// To String (debug)
    /// </summary>
    public override string ToString() {
      return string.Concat(
        $"{{{string.Join(", ", Left)}}} (Support = {LeftSupport * 100:G4}%)",
        $" -> ",
        $"{{{string.Join(", ", Right)}}} (Support = {RightSupport * 100:G4}%)",
        $" With",
        $" Support = {LeftAndRightSupport * 100:G4}%",
        $" Confidence = {Confidence * 100:G4}%",
        $" Lift = {Lift:G4}"
      );
    }

    #endregion Public
  }

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Apriori
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class Apriori {
    #region Public

    /// <summary>
    /// Frequent Subsets 
    /// </summary>
    /// <param name="transactions">Transactions to analyze</param>
    /// <param name="minSupport">Minimum Support</param>
    /// <param name="maxSubsetSize">Maximum Subset size (-1 for all subsets)</param>
    /// <param name="comparer">Comparer</param>
    /// <returns>(subset, support) pairs</returns>
    public static IEnumerable<(HashSet<T> subset, double support)> FrequentSubsets<T>(
      this IEnumerable<IEnumerable<T>> transactions,
      double minSupport,
      int maxSubsetSize = -1,
      IEqualityComparer<T> comparer = null) {

      if (null == transactions)
        throw new ArgumentNullException(nameof(transactions));
      else if (minSupport < 0 || minSupport > 1)
        throw new ArgumentOutOfRangeException(nameof(minSupport));

      if (maxSubsetSize == 0)
        yield break;

      comparer ??= EqualityComparer<T>.Default;

      List<HashSet<T>> data = transactions
        .Select(transaction => new HashSet<T>(transaction ?? new T[0], comparer))
        .ToList();

      double Compute(IEnumerable<T> subset) {
        double count = data.Count(transaction => subset.All(s => transaction.Contains(s)));

        return count / data.Count;
      };

      var list = data
        .SelectMany(transaction => transaction)
        .Distinct(comparer)
        .Select(item => new HashSet<T>(comparer) { item })
        .Select(items => (subset: items, support: Compute(items)))
        .Where(item => item.support >= minSupport)
        .ToList();

      List<List<(HashSet<T> subset, double support)>> frequent =
        new List<List<(HashSet<T> subset, double support)>>() { list };

      while (true) {
        var prior = frequent[frequent.Count - 1];

        if (prior.Count <= 0)
          break;

        foreach (var record in prior)
          yield return record;

        if (maxSubsetSize > 0 && prior.Count >= maxSubsetSize)
          break;

        List<(HashSet<T> subset, double support)> next = new List<(HashSet<T> subset, double support)>();

        frequent.Add(next);

        for (int i = 0; i < prior.Count; ++i) {
          for (int j = i + 1; j < prior.Count; ++j) {
            HashSet<T> hs = new HashSet<T>(prior[i].subset, comparer);

            hs.UnionWith(prior[j].subset);

            if (hs.Count != prior[i].subset.Count + 1)
              continue;

            if (next.Any(z => !z.subset.Except(hs).Any()))
              continue;

            double s = Compute(hs);

            if (s < minSupport)
              continue;

            next.Add((hs, s));
          }
        }
      }
    }

    /// <summary>
    /// Apriori Rules
    /// </summary>
    /// <param name="transactions">Transactions to analyze</param>
    /// <param name="minSupport">Minimum Support</param>
    /// <param name="minConfidence">Minimum Confidence</param>
    /// <param name="minLift">Min Lift (1 by default)</param>
    /// <param name="maxLevel">Maximum Subset size (-1 for all subsets)</param>
    /// <param name="comparer">Comparer</param>
    /// <returns></returns>
    public static IEnumerable<AprioriRule<T>> AprioriRules<T>(
      this IEnumerable<IEnumerable<T>> transactions,
      double minSupport,
      double minConfidence,
      double minLift = 1,
      int maxLevel = -1,
      IEqualityComparer<T> comparer = null) {

      var data = FrequentSubsets(transactions, minSupport, maxLevel, comparer)
        .GroupBy(item => item.subset.Count)
        .ToDictionary(group => group.Key, group => group);

      double Support(HashSet<T> value) {
        if (null == value)
          return 0;

        if (!data.TryGetValue(value.Count, out var records))
          return 0;

        foreach (var (subset, support) in records)
          if (subset.SetEquals(value))
            return support;

        return 0;
      }

      foreach (var pair in data) {
        if (pair.Key <= 1)
          continue;

        foreach (var subset in pair.Value) {
          for (int i = 1; i < subset.subset.Count; ++i) {
            HashSet<T> left = new HashSet<T>(subset.subset.Take(i), comparer);
            HashSet<T> right = new HashSet<T>(subset.subset.Skip(i), comparer);

            double leftAndRightSupport = subset.support;
            double leftSupport = Support(left);
            double rightSupport = Support(right);

            double confidence = leftAndRightSupport / leftSupport;
            double lift = confidence / rightSupport;

            if (confidence < minConfidence || lift < minLift)
              continue;

            yield return new AprioriRule<T>(left, right, leftSupport, rightSupport, leftAndRightSupport);
          }
        }
      }
    }

    #endregion Public
  }

}
