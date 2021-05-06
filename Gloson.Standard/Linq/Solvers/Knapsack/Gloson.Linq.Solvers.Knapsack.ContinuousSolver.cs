using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Gloson.Linq.Solvers.Knapsack {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Continuous Knapsack solver
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class KnapsackSolver {
    #region Internal classes

    /// <summary>
    /// Knapsack continuous solution
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public sealed class KnapsackContinuousSolution<T>
      : IReadOnlyList<T> {

      #region Private Data

      private readonly List<T> m_Items = new();

      private readonly List<int> m_Indexes = new();

      private readonly List<double> m_Parts = new();

      #endregion Private Data

      #region Algorithm

      internal void Add(T item, int index, double part, double weight, double value) {
        m_Items.Add(item);
        m_Indexes.Add(index);
        m_Parts.Add(part);

        Value += part * value;
        Weight += part * weight;
      }

      #endregion Algorithm

      #region Create

      internal KnapsackContinuousSolution(double capacity) {
        Capacity = capacity;
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Initial capacity
      /// </summary>
      public double Capacity { get; }

      /// <summary>
      /// Total Value of the solution
      /// </summary>
      public double Value { get; private set; }

      /// <summary>
      /// Total Weight of the solution
      /// </summary>
      public double Weight { get; private set; }

      /// <summary>
      /// Indexes of the original items to select
      /// </summary>
      public IReadOnlyList<int> Indexes => m_Indexes;

      /// <summary>
      /// Selected items
      /// </summary>
      public IReadOnlyList<T> Items => m_Items;

      /// <summary>
      /// Parts of items (0..1]
      /// </summary>
      public IReadOnlyList<double> Parts => m_Parts;

      /// <summary>
      /// Debug Information
      /// </summary>
      public override string ToString() {
        if (Items.Count <= 0)
          return $"Capacity {Capacity} : Empty Solution";

        return $"Capacity {Capacity} : {Items.Count} items with {Value} value and {Weight} weight ([{string.Join(", ", Indexes)}] indexes).";
      }

      #endregion Public

      #region IReadOnlyList<T>

      /// <summary>
      /// Count
      /// </summary>
      public int Count => Items.Count;

      /// <summary>
      /// Indexer
      /// </summary>
      public T this[int index] => Items[index];

      /// <summary>
      /// Enumerator
      /// </summary>
      public IEnumerator<T> GetEnumerator() => Items.GetEnumerator();

      /// <summary>
      /// Enumerator
      /// </summary>
      IEnumerator IEnumerable.GetEnumerator() => Items.GetEnumerator();

      #endregion IReadOnlyList<T>
    }

    #endregion Internal classes

    #region Public

    /// <summary>
    /// Knapsack Continous Solution
    /// </summary>
    /// <param name="source">Possible items to take</param>
    /// <param name="capacity">Knapsack capacity</param>
    /// <param name="weight">Item's weight</param>
    /// <param name="value">Item's value</param>
    /// <returns></returns>
    public static KnapsackContinuousSolution<T> KnapsackSolveForContinuous<T>(
      this IEnumerable<T> source,
           double capacity,
           Func<T, double> weight,
           Func<T, double> value) {

      if (source is null)
        throw new ArgumentNullException(nameof(source));
      else if (weight is null)
        throw new ArgumentNullException(nameof(weight));
      else if (value is null)
        throw new ArgumentNullException(nameof(value));

      var data = source
        .Select((item, idx) => (
           item,
           weight: weight(item),
           value: value(item),
           index: idx))
        .Where(item => item.weight <= capacity)
        .Where(item => item.value > 0 || item.weight < 0)
        .OrderBy(item => item.weight >= 0)
        .ThenByDescending(item => item.value / item.weight)
        .ToList();

      KnapsackContinuousSolution<T> solution = new(capacity);

      for (int i = 0; i < data.Count; ++i) {
        var item = data[i];

        if (capacity - item.weight >= 0) {
          // completely
          capacity -= item.weight;

          solution.Add(item.item, item.index, 1.0, item.weight, item.value);
        }
        else if (capacity > 0) {
          // partially
          double part = item.weight / capacity;

          solution.Add(item.item, item.index, part, item.weight, item.value);

          break;
        }
      }

      return solution;
    }

    #endregion Public
  }
}
