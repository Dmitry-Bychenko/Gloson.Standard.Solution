using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Gloson.Linq.Solvers.Generic {

  //-------------------------------------------------------------------------------------------------------------------
  //
  /// <summary>
  /// Maximum Weight Independent Set
  /// </summary>
  //
  //-------------------------------------------------------------------------------------------------------------------

  public static class MaximumWeightIndependentSetSolver {
    #region Internal Classes

    /// <summary>
    /// Maximum Weight Independent Set Solution
    /// </summary>
    public sealed class MaximumWeightIndependentSetSolution<T>
      : IReadOnlyList<(T value, long weight, bool isTaken)> {

      #region Private Data

      private readonly List<(T value, long weight, bool isTaken)> m_Items;

      #endregion Private Data

      #region Create

      internal MaximumWeightIndependentSetSolution(long value, Dictionary<int, (T value, long total, bool taken, long weight)> path) {
        Value = value;

        var raw = path
          .OrderBy(pair => pair.Key)
          .Select(pair => pair.Value);

        m_Items = new List<(T value, long weight, bool isTaken)>(path.Count);

        bool priorTaken = false;

        foreach (var item in raw) {
          priorTaken = (!priorTaken) && item.taken;

          m_Items.Add((item.value, item.weight, priorTaken));
        }

        TakenCount = m_Items.Count(item => item.isTaken);
      }

      #endregion Create

      #region Public

      /// <summary>
      /// Solution (Weight)
      /// </summary>
      public long Value { get; }

      /// <summary>
      /// Set (Taken) Count
      /// </summary>
      public int TakenCount { get; }

      /// <summary>
      /// Solution as a set
      /// </summary>
      public IReadOnlyList<(T value, long weight, bool isTaken)> Items => m_Items;

      /// <summary>
      /// To String
      /// </summary>
      /// <returns></returns>
      public override string ToString() =>
        $"Value: {Value} Taken: {TakenCount} From: {m_Items.Count}";


      #endregion Public

      #region IReadOnlyList<(T value, long weight, bool isTaken)>

      /// <summary>
      /// Count
      /// </summary>
      public int Count => m_Items.Count;

      /// <summary>
      /// Indexer
      /// </summary>
      public (T value, long weight, bool isTaken) this[int index] => m_Items[index];

      /// <summary>
      /// Enumerator (typed)
      /// </summary>
      public IEnumerator<(T value, long weight, bool isTaken)> GetEnumerator() => m_Items.GetEnumerator();

      /// <summary>
      /// Enumerator (untyped)
      /// </summary>
      System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator() => GetEnumerator();

      #endregion IReadOnlyList<(T value, long weight, bool isTaken)>
    }

    #endregion Internal Classes

    #region Algorithm 

    private static long CoreSolve<T>(T[] items, int at, Func<T, long> map, Dictionary<int, (T value, long total, bool taken, long weigth)> cache) {
      if (null == items || items.Length <= 0)
        return 0;
      else if (at < 0)
        return CoreSolve(items, 0, map, cache);
      else if (at >= items.Length)
        return 0;

      if (cache.TryGetValue(at, out var solution))
        return solution.total;

      long value = map(items[at]);

      long leave = CoreSolve(items, at + 1, map, cache);

      if (value <= 0) {
        cache.Add(at, (items[at], leave, false, value));

        return leave;
      }

      long take = value + CoreSolve(items, at + 2, map, cache);

      if (take > leave) {
        cache.Add(at, (items[at], take, true, value));

        return take;
      }

      cache.Add(at, (items[at], leave, false, value));

      return leave;
    }

    #endregion Algorithm

    #region Public

    /// <summary>
    /// Solve for Maximum Weight Independent Set
    /// </summary>
    /// <param name="source">Source Items</param>
    /// <param name="weight">Map function: item to its value (weight)</param>
    /// <returns></returns>
    public static MaximumWeightIndependentSetSolution<T> MaximumWeightIndependentSet<T>(
      this IEnumerable<T> source,
      Func<T, long> weight) {

      if (null == source)
        throw new ArgumentNullException(nameof(source));
      else if (null == weight)
        weight = x => (long)Convert.ChangeType(x, typeof(long));

      T[] items = source.ToArray();

      Dictionary<int, (T value, long total, bool taken, long weight)> cache =
        new Dictionary<int, (T value, long total, bool taken, long weight)>();

      long value = CoreSolve(items, 0, weight, cache);

      return new MaximumWeightIndependentSetSolution<T>(value, cache);
    }

    #endregion Public
  }

}
