using System;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Algorithms.Solvers {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Dynamic Backpack Solver 
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public sealed class DynamicKnapsackSolver : IEquatable<DynamicKnapsackSolver> {
    #region Private Data

    // Items
    private readonly List<(int weight, int value)> m_Items;

    // Indexes
    private readonly List<int> m_Indexes = new List<int>();

    #endregion Private Data

    #region Algorithm

    private void CoreSolve() {
      int[][] A = Enumerable
        .Range(0, m_Items.Count + 1)
        .Select(yy => new int[Weight + 1])
        .ToArray();

      for (int k = 1; k <= m_Items.Count; ++k)
        for (int w = 1; w <= Weight; ++w)
          if (w >= m_Items[k - 1].weight)
            A[k][w] = Math.Max(A[k - 1][w], A[k - 1][w - m_Items[k - 1].weight] + m_Items[k - 1].value);
          else
            A[k][w] = A[k - 1][w];

      // Backtrack
      int kk = m_Items.Count;
      int ww = Weight;

      m_Indexes.Clear();

      while (A[kk][ww] != 0) {
        if (A[kk - 1][ww] == A[kk][ww])
          kk -= 1;
        else {
          m_Indexes.Add(kk - 1);

          kk -= 1;
          ww -= m_Items[kk].weight;
        }
      }

      m_Indexes.Sort();
    }

    #endregion Algorithm

    #region Create

    /// <summary>
    /// Standard construtor
    /// </summary>
    public DynamicKnapsackSolver(int weight, IEnumerable<(int weight, int value)> items) {
      if (items is null)
        throw new ArgumentNullException(nameof(items));

      m_Items = items.ToList();

      Weight = weight;

      CoreSolve();

      foreach (int index in m_Indexes) {
        ActualWeight += m_Items[index].weight;
        ActualValue += m_Items[index].value;
      }
    }

    #endregion Create

    #region Public

    /// <summary>
    /// Weight
    /// </summary>
    public int Weight { get; }

    /// <summary>
    /// Actual Weight
    /// </summary>
    public int ActualWeight { get; }

    /// <summary>
    /// Actual Value
    /// </summary>
    public int ActualValue { get; }

    /// <summary>
    /// Items
    /// </summary>
    public IReadOnlyList<(int weight, int value)> Items => m_Items;

    /// <summary>
    /// Solution indexes
    /// </summary>
    public IReadOnlyList<int> Indexes => m_Indexes;

    /// <summary>
    /// To String
    /// </summary>
    public override string ToString() =>
      $"{m_Indexes.Count} items [{string.Join(", ", m_Indexes)}] with {ActualValue} total value and {ActualWeight} total weight";

    #endregion Public

    #region Operators

    /// <summary>
    /// Equal 
    /// </summary>
    public static bool operator ==(DynamicKnapsackSolver left, DynamicKnapsackSolver right) {
      if (ReferenceEquals(left, right))
        return true;
      else if (left is null || right is null)
        return false;
      else
        return left.Equals(right);
    }

    /// <summary>
    /// Not Equal
    /// </summary>
    public static bool operator !=(DynamicKnapsackSolver left, DynamicKnapsackSolver right) {
      if (ReferenceEquals(left, right))
        return false;
      else if (left is null || right is null)
        return true;
      else
        return !left.Equals(right);
    }

    #endregion Operators

    #region IEquatable<DynamicKnapsackSolver>

    /// <summary>
    /// Equals
    /// </summary>
    public bool Equals(DynamicKnapsackSolver other) {
      if (other is null)
        return false;

      if (Weight != other.Weight)
        return false;
      else if (m_Items.Count != other.m_Items.Count)
        return false;

      return m_Items
        .OrderBy(x => x.weight)
        .ThenBy(x => x.value)
        .SequenceEqual(other
           .m_Items
           .OrderBy(x => x.weight)
           .ThenBy(x => x.value));
    }

    /// <summary>
    /// Equals
    /// </summary>
    public override bool Equals(object obj) => obj is DynamicKnapsackSolver other && Equals(other);

    /// <summary>
    /// Hash Code
    /// </summary>
    public override int GetHashCode() => unchecked((Weight << 10) ^ m_Items.Count);

    #endregion IEquatable<DynamicKnapsackSolver>
  }

}
