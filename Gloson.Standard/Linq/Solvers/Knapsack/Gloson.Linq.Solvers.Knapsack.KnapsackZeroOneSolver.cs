using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq.Solvers.Knapsack {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Knapsack Solver
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static partial class KnapsackSolver {
    #region Inner Classes

    /// <summary>
    /// Knapsack Zero Or One Solution
    /// </summary>
    public sealed class KnapsackZeroOneSolution<T>
      : IReadOnlyList<T> {

      #region Create 

      internal KnapsackZeroOneSolution(
        double capacity,
        double value,
        double weight,
        IEnumerable<int> indexes,
        IEnumerable<T> items) {

        Value = value;
        Weight = weight;
        Capacity = capacity;

        Indexes = indexes.ToList();
        Items = items.ToList();
      }

      internal KnapsackZeroOneSolution(double capacity) {
        Capacity = capacity;
        Value = 0.0;
        Weight = 0.0;

        Indexes = new List<int>();
        Items = new List<T>();
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Total Value of the solution
      /// </summary>
      public double Value { get; }

      /// <summary>
      /// Total Weight of the solution
      /// </summary>
      public double Weight { get; }

      /// <summary>
      /// Initial knapsack Capacity
      /// </summary>
      public double Capacity { get; }

      /// <summary>
      /// Indexes of the original items to select
      /// </summary>
      public IReadOnlyList<int> Indexes { get; }

      /// <summary>
      /// Selected items
      /// </summary>
      public IReadOnlyList<T> Items { get; }

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

    #endregion Inner Classes

    #region Public

    /// <summary>
    /// Knapsack Zero Or One Solution
    /// </summary>
    /// <param name="source">Possible items to take</param>
    /// <param name="capacity">Knapsack capacity</param>
    /// <param name="weight">Item's weight</param>
    /// <param name="value">Item's value</param>
    /// <returns></returns>
    public static KnapsackZeroOneSolution<T> KnapsackSolveForZeroOne<T>(
      this IEnumerable<T> source,
           double capacity,
           Func<T, double> weight,
           Func<T, double> value) {

      if (ReferenceEquals(null, source))
        throw new ArgumentNullException(nameof(source));
      else if (ReferenceEquals(null, weight))
        throw new ArgumentNullException(nameof(weight));
      else if (ReferenceEquals(null, value))
        throw new ArgumentNullException(nameof(value));

      var data = source
        .Select((item, idx) => (
           item   : item, 
           weight : weight(item), 
           value  : value(item),
           index  : idx))
        .Where(item => item.weight <= capacity)
        .Where(item => item.value > 0 || item.weight < 0)
        .OrderBy(item => item.weight >= 0)
        .ThenByDescending(item => item.weight)
        .ToList();

      // Specal Cases :

      // Empty :

      if (data.Count <= 0)
        return new KnapsackZeroOneSolution<T>(capacity);

      // All :
      
      double maxCapacity = data
        .Where(item => item.value > 0)
        .Sum(item => item.weight);

      if (capacity >= maxCapacity) {
        var positives = data
          .Where(item => item.value > 0)
          .ToArray();

        return new KnapsackZeroOneSolution<T>(
          capacity,
          positives.Sum(item => item.value),
          positives.Sum(item => item.weight),
          positives.Select(item => item.index),
          positives.Select(item => item.item)
        );
      }

      // General case :

      double[] takeAll = new double[data.Count];

      for (int i = data.Count - 1; i >= 0; --i) {
        double prior = i >= data.Count - 1 
          ? 0.0 
          : takeAll[i + 1];

        takeAll[i] = data[i].weight + prior;
      }

      // Cache (memoization) :

      Dictionary<Tuple<int, double>,
                 Tuple<double, bool>> cache =
        new Dictionary<Tuple<int, double>, Tuple<double, bool>>();

      Func<int, double, double> solver = null;

      solver = (i, w) => {
        if (i < 0 || w < 0)
          return 0.0;

        double result;

        if (cache.TryGetValue(Tuple.Create(i, w), out var cachedResult))
          return cachedResult.Item1;

        if (data[i].weight > w) {
          // Skip :
          return solver(i - 1, w);
        }
        else if (data[i].weight <= 0 && data[i].value > 0) {
          // Take :
          result = solver(i - 1, w - data[i].weight) + data[i].value;

          cache.Add(Tuple.Create(i, w), Tuple.Create(result, true));

          return result;
        }
        else if (data[i].weight <= takeAll[i] && data[i].value > 0) {
          // Take :
          result = solver(i - 1, w - data[i].weight) + data[i].value;

          cache.Add(Tuple.Create(i, w), Tuple.Create(result, true));

          return result;
        }

        // General Case : 

        var skip = solver(i - 1, w);
        var take = solver(i - 1, w - data[i].weight) + data[i].value;

        if (skip > take) {
          result = skip;

          cache.Add(Tuple.Create(i, w), Tuple.Create(result, false));
        }
        else {
          result = take;

          cache.Add(Tuple.Create(i, w), Tuple.Create(result, true));
        }

        return result;
      };

      // Entire task solving :

      double solution = solver(data.Count - 1, capacity);

      // Solved, backtrack : 

      int index = data.Count - 1;
      double bestWeight = capacity;

      List<int> sequence = new List<int>();

      while (index >= 0) {
        if (cache.TryGetValue(Tuple.Create(index, bestWeight), out var step)) {
          if (step.Item2) {
            sequence.Add(index);
            double ww = data[index].weight;

            bestWeight -= ww;
          }

          index -= 1;
        }
        else
          index -= 1;
      }

      sequence.Sort();

      return new KnapsackZeroOneSolution<T>(
        capacity,
        solution,
        sequence.Sum(i => data[i].weight),
        sequence.Select(i => data[i].index),
        sequence.Select(i => data[i].item)
      );
    }

    #endregion Public
  }
}
