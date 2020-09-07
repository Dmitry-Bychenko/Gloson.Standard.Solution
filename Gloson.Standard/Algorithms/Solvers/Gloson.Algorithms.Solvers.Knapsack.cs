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

  public sealed class DynamicKnapsackSolver {
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
        for (int s = 1; s <= Weight; ++s)
          if (s >= m_Items[k - 1].weight)
            A[k][s] = Math.Max(A[k - 1][s], A[k - 1][s - m_Items[k - 1].weight] + m_Items[k - 1].value);
          else
            A[k][s] = A[k - 1][s];

      // Backtrack
      int kk = m_Items.Count;
      int ss = Weight;

      m_Indexes.Clear();

      while (A[kk][ss] != 0) {
        if (A[kk - 1][ss] == A[kk][ss])
          kk -= 1;
        else {
          m_Indexes.Add(kk - 1);

          kk -= 1;
          ss -= m_Items[kk].weight;
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
      if (null == items)
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
  }

}
